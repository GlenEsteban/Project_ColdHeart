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
        bool isAPlayerCharacter;
        bool isControlledByPlayer;
        CharacterManager characterManager;
        CharacterController characterController;
        Movement movement;
        NavMeshAgent navMeshAgent;
        GameObject followTarget;

        void Start() {
            characterManager = FindObjectOfType<CharacterManager>();
            characterController = GetComponent<CharacterController>();
            movement = GetComponent<Movement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        void Update() {
            UpdateAIControllerState();

            if (isAPlayerCharacter) {
                if (isControlledByPlayer) {return;}
                followTarget = characterManager.GetPlayerCharacterTarget();
                movement.FollowTarget(followTarget);
            }
            else if (characterManager.GetIsAnEnemyCharacter(gameObject)) {
                RunEnemyGuardBehavior();
            }
        }
        void UpdateAIControllerState()
        {
            isAPlayerCharacter = characterController.GetIsPlayerCharacter();
            isControlledByPlayer = characterController.GetIsControlledByPlayer();
            if (isControlledByPlayer) {
                navMeshAgent.enabled = false;
            }
            else {
                navMeshAgent.enabled = true;
            }
        }
        void RunEnemyGuardBehavior()
        {
            
        }
    }
}
