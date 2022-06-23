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
        bool isControlledByPlayer;
        PlayerInput playerInput;
        CharacterManager characterManager;
        Movement movement;
        Vector2 moveInput;
        Combat combat;
        bool isGuarding;
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
        }
        void Update()
        {
            UpdateCharacterControllerState();

            if (!isAPlayerCharacter || !isControlledByPlayer)
            {
                playerInput.enabled = false;
                return;
            }
            else
            {
                playerInput.enabled = true;
            }
            
            movement.MoveCharacter(moveInput);
            movement.LookAtCursor();
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
        void UpdateCharacterControllerState()
        {
            if (characterManager.GetIsAPlayerCharacter(gameObject)) {
                isAPlayerCharacter = true;

                if (characterManager.GetCurrentPlayerCharacter() == gameObject) {
                    isControlledByPlayer = true;
                }
                else {
                    isControlledByPlayer = false;
                }
            }
            else {
                isAPlayerCharacter = false;
            }
        }
        void OnMove(InputValue value) {
            moveInput = value.Get<Vector2>();
        }
        void OnGuard() {
            isGuarding = !isGuarding;
            combat.SetIsGuarding(isGuarding);
        }
        void OnSwitch() {
            characterManager.SwitchCurrentPlayerCharacter();
        }
    }
}