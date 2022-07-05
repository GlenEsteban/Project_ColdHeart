using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using coldheart_core;
using coldheart_movement;
using System;

namespace coldheart_controls {
    public enum TargetState {
        MainCharacter,
        CurrentCharacter,
        Self
    }
    public class FollowerAIController : MonoBehaviour {
        bool isControlledByPlayer;
        bool isAIControlStateUpdated;
        CharacterManager characterManager;
        Movement movement;
        NavMeshAgent navMeshAgent;
        TargetState targetState;
        Transform followTarget;
        public void SetTargetState(TargetState targetState) {
            this.targetState = targetState;
        }
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            movement = GetComponent<Movement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void OnEnable() {
            SetTargetState(TargetState.MainCharacter);
            
            characterManager.onSwitchCharacterAction += UpdateAIControlStatus;
            characterManager.onSwitchCharacterAction += UpdateFollowTarget;
            characterManager.onAllPlayerCharactersFollowCurrentPlayer += FollowCurrentCharacter;
        }
        void OnDisable() {
            if (characterManager.CheckIfCharacterIsAPlayerCharacter(gameObject)) {
                characterManager.onSwitchCharacterAction -= UpdateAIControlStatus;
            }
        }
        void Update() {
            // Although having an AI control status creates a state, the state is very
            // simple and only needs it once rather than having it checked every frame.
            if (!isAIControlStateUpdated) {
                UpdateAIControlStatus();
                UpdateFollowTarget();
            }

            if (isControlledByPlayer) {return;}

            movement.FollowTarget(followTarget);
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
            if (characterManager.GetCurrentPlayerCharacter() != null) {
                isAIControlStateUpdated = true;
            }
        }
        void UpdateFollowTarget() {
            if (targetState == TargetState.MainCharacter) {
                GameObject mainCharacter = characterManager.GetMainPlayerCharacter();
                followTarget = mainCharacter.transform;
            }
            else if (targetState == TargetState.CurrentCharacter) {
                GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
                followTarget = currentCharacter.transform;
            }
            else if (targetState == TargetState.Self) {
                GameObject self = gameObject;
                followTarget = self.transform;
            }
        }
        void FollowCurrentCharacter() {
            targetState = TargetState.CurrentCharacter;
            UpdateFollowTarget();
        }
        public void SwitchTargetState() {
            if (targetState == TargetState.Self) {
                SetTargetState(TargetState.MainCharacter);
                print(gameObject.name + " is following main character!");
            }
            else if (targetState == TargetState.MainCharacter) {
                SetTargetState(TargetState.CurrentCharacter);
                print(gameObject.name + " is following current character!");
            }
            else if (targetState == TargetState.CurrentCharacter) {
                SetTargetState(TargetState.Self);
                print(gameObject.name + " is following its guard location");
            }
        }
        // Visualize follow target connections
        void OnDrawGizmos() {
            if (followTarget == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, followTarget.position);
        }
    }
}
