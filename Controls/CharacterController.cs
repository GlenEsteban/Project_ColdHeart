using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using coldheart_core;
using coldheart_movement;
using coldheart_combat;

namespace coldheart_controls {
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Combat))]
    public class CharacterController : MonoBehaviour
    {
        public bool isAPlayerCharacter;
        public bool isControlledByPlayer;
        bool isControlStateUpdated;
        PlayerInput playerInput;
        CharacterManager characterManager;
        Movement movement;
        Vector2 moveInput;
        Combat combat;
        public bool GetIsPlayerCharacter() {
            return isAPlayerCharacter;
        }
        public bool GetIsControlledByPlayer() {
            return isControlledByPlayer;
        }
        void Start() {
            playerInput = GetComponent<PlayerInput>();
            characterManager = FindObjectOfType<CharacterManager>();
            movement = GetComponent<Movement>();
            combat = GetComponent<Combat>();

            RegisterCharacter();
            characterManager.onSwitchCharacterAction += UpdateControlStatus;
        }
        void Update()
        {
            if (!isControlStateUpdated) {
                UpdateControlStatus();
            }
            
            if (isControlledByPlayer) {
                movement.MoveCharacter(moveInput);
                movement.LookAtCursor();
            }
        }
        void RegisterCharacter() {
            // Main Character is registered on the Character Manager for scene setup reasons
            if (characterManager.GetMainPlayerCharacter() == gameObject) {return;}

            if (isAPlayerCharacter) {
                characterManager.RegisterPlayerCharacter(gameObject, isControlledByPlayer);
            }
            else {
                characterManager.RegisterEnemyCharacter(gameObject);
            }
        }
        void UpdateControlStatus() {
            isControlledByPlayer = characterManager.GetCurrentPlayerCharacter() == gameObject;
            if (isControlledByPlayer) {
                playerInput.enabled = true;
            }
            else {
                playerInput.enabled = false;
            }
            if (characterManager.GetCurrentPlayerCharacter() != null) {
                isControlStateUpdated = true;
            }
        }
        void OnMove(InputValue value) {
            moveInput = value.Get<Vector2>();
        }
        void OnSwitch() {
            characterManager.SwitchCurrentPlayerCharacter();
        }
        void OnGuard() {
            movement.SetIsAbleToMove(!movement.GetIsAbleToMove());
        }
    }
}