using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using coldheart_core;
using coldheart_movement;
using System;

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
        public Transform currentFollowTarget;
        public Transform assignedFollowTarget;
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            movement = GetComponent<Movement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void OnEnable() {  
            targetState = TargetState.FollowTargetCharacter;

            characterManager.onSwitchCharacterAction += UpdateAIControlStatus;
            characterManager.onSwitchCharacterAction += UpdateTargetState;
            characterManager.onAllPlayerCharactersFollowTargetPlayer += FollowTargetCharacter;
        }
        void OnDisable() {
            if (characterManager.CheckIfCharacterIsAPlayerCharacter(gameObject)) {
                characterManager.onSwitchCharacterAction -= UpdateAIControlStatus;
                characterManager.onSwitchCharacterAction -= UpdateTargetState;
                characterManager.onAllPlayerCharactersFollowTargetPlayer -= FollowTargetCharacter;
            }
        }
        void Update() {
            if (!isAIControlStateUpdated) {
                UpdateAIControlStatus();
            }

            if (isControlledByPlayer) {return;}

            if (targetState == TargetState.FollowTargetCharacter) {
                if (assignedFollowTarget == null) {
                    assignedFollowTarget = characterManager.GetMainPlayerCharacter().transform;
                }
                else {
                    movement.FollowTarget(assignedFollowTarget);
                }
            }
            else if (targetState == TargetState.FollowCurrentCharacter) {
                movement.FollowTarget(currentFollowTarget);
            }
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
        void FollowTargetCharacter() {
            targetState = TargetState.FollowTargetCharacter;
            GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
            assignedFollowTarget = characterManager.GetCurrentPlayerCharacter().transform;
        }
        void FollowCurrentCharacter() {
            targetState = TargetState.FollowCurrentCharacter;
            GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
            currentFollowTarget = currentCharacter.transform;
        }
        public void SwitchTargetState() {
            if (targetState == TargetState.FollowTargetCharacter) {
                FollowCurrentCharacter();
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
        public void UpdateTargetState() {
            if (targetState == TargetState.FollowCurrentCharacter) {
                FollowCurrentCharacter();
            }
        }
        void OnDrawGizmos() {
            if (assignedFollowTarget == null) return;
            
            if (targetState == TargetState.FollowTargetCharacter) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, assignedFollowTarget.position);
            }
            else if (targetState == TargetState.FollowCurrentCharacter) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentFollowTarget.position);
            }
        }
    }
}
