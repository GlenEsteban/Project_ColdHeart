using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using coldheart_core;
using coldheart_movement;
using System;
using coldheart_combat;

namespace coldheart_controls {
    public enum TargetState {
        FollowTargetCharacter,
        FollowCurrentCharacter,
        FollowNoOne
    }
    public class FollowerAIController : MonoBehaviour {
        bool isControlledByPlayer;
        bool isAIControlStateUpdated;
        CharacterManager characterManager;
        GameObject currentCharacter;
        Movement movement;
        NavMeshAgent navMeshAgent;
        public TargetState targetState;
        public Transform followTarget;
        Combat combat;
        float timeSinceLastCheckForNearbyEnemies;
        GameObject attackTarget;
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            movement = GetComponent<Movement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            combat = GetComponent<Combat>();
        }
        void OnEnable() {  
            targetState = TargetState.FollowTargetCharacter;

            characterManager.onSwitchCharacterAction += UpdateAIControlStatus;
            characterManager.onAllPlayerCharactersFollowCurrentPlayer += UpdateTargetStateOnFollowMe;
        }
        void OnDisable() {
            if (characterManager.CheckIfCharacterIsAPlayerCharacter(gameObject)) {
                characterManager.onSwitchCharacterAction -= UpdateAIControlStatus;
                characterManager.onAllPlayerCharactersFollowCurrentPlayer -= UpdateTargetStateOnFollowMe;
            }
        }
        void Update()
        {
            if (!isAIControlStateUpdated)
            {
                UpdateAIControlStatus();
            }

            if (isControlledByPlayer) { return; }

            timeSinceLastCheckForNearbyEnemies += Time.deltaTime;
            if (timeSinceLastCheckForNearbyEnemies > 1f) {
                attackTarget = CheckForNearbyEnemies();
                print("Checking for enemies...");
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
            if (targetState == TargetState.FollowTargetCharacter)
            {
                if (followTarget == null)
                {
                    followTarget = characterManager.GetMainPlayerCharacter().transform;
                }
                else
                {
                    movement.FollowTarget(followTarget);
                }
            }
            else if (targetState == TargetState.FollowCurrentCharacter)
            {
                GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
                movement.FollowTarget(currentCharacter.transform);
            }
        }
        void AttackBehavior(GameObject enemyTarget) {
            print("Attacking " + enemyTarget.name);
            movement.FollowTarget(enemyTarget.transform);
            // If is in weapon range, face target and attack else resume pursuit.
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
            GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
            followTarget = characterManager.GetCurrentPlayerCharacter().transform;
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
            
            if (targetState == TargetState.FollowTargetCharacter) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, followTarget.position);
            }
            else if (targetState == TargetState.FollowCurrentCharacter) {
                Gizmos.color = Color.red;
                GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
                Gizmos.DrawLine(transform.position, currentCharacter.transform.position);
            }
        }
    }
}
