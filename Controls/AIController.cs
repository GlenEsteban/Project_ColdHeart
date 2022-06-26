using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using coldheart_core;
using coldheart_movement;
using System;


namespace coldheart_controls {
    public class AIController : MonoBehaviour
    {
        public bool isControlledByPlayer;
        bool isAIControlStateUpdated;
        CharacterManager characterManager;
        Movement movement;
        NavMeshAgent navMeshAgent;
        public Transform followTarget;
        public void SetFollowTarget(Transform targetTransform) {
            followTarget = targetTransform;
        }
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            movement = GetComponent<Movement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void OnEnable() {
            characterManager.onSwitchCharacterAction += UpdateAIControlStatus;
        }
        void OnDisable() {
            characterManager.onSwitchCharacterAction -= UpdateAIControlStatus;
        }
        void Update() {
            if (!isAIControlStateUpdated) {
                UpdateAIControlStatus();
                return;
            }

            if (tag == "Player") {
                if (isControlledByPlayer) {return;}

                if (followTarget == null) {
                    SetFollowTarget(characterManager.GetCurrentPlayerCharacter().transform);
                }            
                movement.FollowTarget(followTarget);
            }
            else if (tag == "Enemy") {
                if (followTarget == null) {
                    SetFollowTarget(transform);
                }
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
    }
}
