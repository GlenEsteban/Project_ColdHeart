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
        AssignedTargetCharacter,
        ThisCharacter
    }
    public class FollowerAIController : MonoBehaviour {
        bool isControlledByPlayer;
        bool isAIControlStateUpdated;
        CharacterManager characterManager;
        Movement movement;
        NavMeshAgent navMeshAgent;
        public TargetState targetState;
        public Transform followTarget;
        public Transform assignedTargetCharacter;
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
            characterManager.onSwitchCharacterAction += UpdateTargetState;
            characterManager.onAllPlayerCharactersFollowTargetPlayer += FollowAssignedTargetCharacter;
        }
        void OnDisable() {
            if (characterManager.CheckIfCharacterIsAPlayerCharacter(gameObject)) {
                characterManager.onSwitchCharacterAction -= UpdateAIControlStatus;
                characterManager.onAllPlayerCharactersFollowTargetPlayer -= FollowCurrentCharacter;
            }
        }
        void Update() {
            // Although having an AI control status creates a state, the state is very
            // simple and only needs it once rather than having it checked every frame.
            if (!isAIControlStateUpdated) {
                UpdateAIControlStatus();
                FollowMainCharacter();
                assignedTargetCharacter = characterManager.GetMainPlayerCharacter().transform;
            }

            if (isControlledByPlayer) {return;}

            if (targetState == TargetState.AssignedTargetCharacter) {
                movement.FollowTarget(assignedTargetCharacter);

            }
            else {
                movement.FollowTarget(followTarget);
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
            if (characterManager.GetCurrentPlayerCharacter() != null) {
                isAIControlStateUpdated = true;
            }
        }
        void FollowMainCharacter() {
            targetState = TargetState.MainCharacter;
            GameObject mainCharacter = characterManager.GetMainPlayerCharacter();
            followTarget = mainCharacter.transform;
        }
        void FollowCurrentCharacter() {
            targetState = TargetState.CurrentCharacter;
            GameObject currentCharacter = characterManager.GetCurrentPlayerCharacter();
            followTarget = currentCharacter.transform;
        }
        void FollowAssignedTargetCharacter() {
            GameObject currentPlayerCharacter = characterManager.GetCurrentPlayerCharacter();
            if (this.gameObject == currentPlayerCharacter) {return;}

            targetState = TargetState.AssignedTargetCharacter;
            assignedTargetCharacter = currentPlayerCharacter.transform; // does not change until FollowMe() is called again
        }
        public void SwitchTargetState() {
            if (targetState == TargetState.MainCharacter) {
                SetTargetState(TargetState.CurrentCharacter);
                FollowCurrentCharacter();
                print(gameObject.name + " is following current character!");
            }
            else if (targetState == TargetState.CurrentCharacter) {
                SetTargetState(TargetState.ThisCharacter);
                followTarget = this.gameObject.transform;
                print(gameObject.name + " is following no one!");
            }            
            else if (targetState == TargetState.ThisCharacter) {
                SetTargetState(TargetState.MainCharacter);
                FollowMainCharacter();
                print(gameObject.name + " is following main character!");
            }
            else if (targetState == TargetState.AssignedTargetCharacter) {
                SetTargetState(TargetState.MainCharacter);
                FollowMainCharacter();
                print(gameObject.name + " is following main character!");
            } 
        }
        public void UpdateTargetState() {
            if (targetState == TargetState.MainCharacter) {
                FollowMainCharacter();
            }
            else if (targetState == TargetState.CurrentCharacter) {
                FollowCurrentCharacter();
            }
        }
        // Visualize follow target connections
        void OnDrawGizmos() {
            if (followTarget == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, followTarget.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, assignedTargetCharacter.position);
        }
    }
}
