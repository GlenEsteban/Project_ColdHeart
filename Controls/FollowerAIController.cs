using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using coldheart_core;
using coldheart_movement;
using coldheart_combat;

namespace coldheart_controls
{
    public class FollowerAIController : MonoBehaviour {
        CharacterManager characterManager;
        Movement movement;
        Combat combat;
        NavMeshAgent navMeshAgent;
        TargetState targetState;
        GameObject followTarget;
        GameObject attackTarget;
        bool isControlledByPlayer;
        bool isAIControlStateUpdated;
        float timeSinceLastCheckForNearbyEnemies;
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            movement = GetComponent<Movement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            combat = GetComponent<Combat>();
        }
        void OnEnable() {  
            targetState = TargetState.FollowTargetCharacter;

            characterManager.onSwitchCharacterAction += UpdateAIControlStatus;
            characterManager.onFollowMeAction += UpdateTargetStateOnFollowMe;
        }
        void OnDisable() {
            characterManager.onSwitchCharacterAction -= UpdateAIControlStatus;
            characterManager.onFollowMeAction -= UpdateTargetStateOnFollowMe;
        }
        void Update()
        { 
            if (!isAIControlStateUpdated) {
                UpdateAIControlStatus();
            }

            if (isControlledByPlayer) { return; }

            timeSinceLastCheckForNearbyEnemies += Time.deltaTime;
            if (timeSinceLastCheckForNearbyEnemies > 1f) {
                attackTarget = CheckForNearbyEnemies();
                timeSinceLastCheckForNearbyEnemies = 0;
            }

            if (attackTarget == null) {
                FollowBehavior();
            }
            else {
                AttackBehavior(attackTarget);
            }
        }
        GameObject CheckForNearbyEnemies() {
            List<GameObject> enemies = characterManager.GetEnemyCharacters();
            float chaseRange = 10f;
            bool isInRange;
            foreach (GameObject enemy in enemies) {
                Vector3 pointA = enemy.transform.position;
                Vector3 pointB = this.gameObject.transform.position;
                isInRange = (pointA - pointB).sqrMagnitude < (chaseRange * chaseRange);
                if (isInRange) {
                    return enemy;
                }
                else {
                    return null;
                }
            }
            return null;
        }
        void FollowBehavior() {
            if (targetState == TargetState.FollowTargetCharacter) {
                if (followTarget == null) {
                    followTarget = characterManager.GetMainPlayerCharacter();
                }
                else {
                    movement.FollowTarget(followTarget.transform);
                }
            }
            else if (targetState == TargetState.FollowCurrentCharacter) {
                GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
                movement.FollowTarget(currentCharacter.transform);
            }
        }
        void AttackBehavior(GameObject enemyTarget) {
            movement.FollowTarget(enemyTarget.transform);
            combat.CallThrottledInstantAbility();
        }
        void UpdateAIControlStatus() {
            isControlledByPlayer = characterManager.GetCurrentPlayerCharacter() == gameObject;
            if (isControlledByPlayer) {
                navMeshAgent.enabled = false;
            }
            else {
                navMeshAgent.enabled = true;
            }

            // For some reason GetCurrentPlayerCharacter() returns null at Start and on the 
            // ... first Update(), so here's a quick fix. 
            GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
            if (currentCharacter != null) {
                isAIControlStateUpdated = true;
            }
        }
        void UpdateTargetStateOnFollowMe() {
            targetState = TargetState.FollowTargetCharacter;
            followTarget = characterManager.GetCurrentPlayerCharacter();
        }
        public void SwitchTargetState() {
            if (targetState == TargetState.FollowTargetCharacter) {
                targetState = TargetState.FollowCurrentCharacter;
                print(gameObject.name + " is following current character!");
            }
            else if (targetState == TargetState.FollowCurrentCharacter) {
                targetState = TargetState.FollowNoOne;
                print(gameObject.name + " is following no one!");
            }            
            else if (targetState == TargetState.FollowNoOne) {
                targetState = TargetState.FollowTargetCharacter;
                print(gameObject.name + " is following its target!");
            }
        }
        void OnDrawGizmos() {
            if (followTarget == null) return;
            
            if (attackTarget != null) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, attackTarget.transform.position);
            }
            else if (targetState == TargetState.FollowTargetCharacter) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, followTarget.transform.position);
            }
            else if (targetState == TargetState.FollowCurrentCharacter) {
                Gizmos.color = Color.green;
                GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
                Gizmos.DrawLine(transform.position, currentCharacter.transform.position);
            }
        }
    }
    public enum TargetState {
        FollowTargetCharacter,
        FollowCurrentCharacter,
        FollowNoOne
    }
}