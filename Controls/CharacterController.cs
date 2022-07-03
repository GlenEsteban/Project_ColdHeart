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
        bool isAbleToSwitchFollowTarget = true;
        public float mouseScrollY;
        CharacterManager characterManager;
        PlayerInput playerInput;
        PlayerControls playerControls;
        Movement movement;
        Vector2 moveInput;
        Combat combat;
        public bool GetIsPlayerCharacter() {
            return isAPlayerCharacter;
        }
        public bool GetIsControlledByPlayer() {
            return isControlledByPlayer;
        }
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            playerInput = GetComponent<PlayerInput>();
            movement = GetComponent<Movement>();
            combat = GetComponent<Combat>();

            playerControls = new PlayerControls();
            playerControls.Player.SwitchCharacter.performed += x => mouseScrollY = x.ReadValue<float>();
        }
        void OnEnable() {
            RegisterCharacter();
            
            characterManager.onSwitchCharacterAction += UpdateControlStatus;

            playerControls.Enable();
        }
        void OnDisable() {
            if (isAPlayerCharacter) {
                characterManager.UnregisterPlayerCharacter(gameObject);
            }
            else {
                characterManager.UnregisterEnemyCharacter(gameObject);
            }

            characterManager.onSwitchCharacterAction -= UpdateControlStatus;

            playerControls.Disable();
        }
        void Update() {
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
            if (tag == "Player") {
                isAPlayerCharacter = true;
            }
            else if (tag == "Enemy") {
                isAPlayerCharacter = false;
            }
            else {
                isAPlayerCharacter = false;
                print("Character is untagged!!!");
            }

            isControlledByPlayer = characterManager.GetCurrentPlayerCharacter() == gameObject;
            if (isControlledByPlayer) {
                playerInput.enabled = true;
            }
            else {
                playerInput.enabled = false;
            }
            
            // For some reason GetCurrentPlayerCharacter() returns null at Start and on the 
            // ... first Update(), so here's a quick fix. 
            if (characterManager.GetCurrentPlayerCharacter() != null) {
                isControlStateUpdated = true;
            }
        }
        void OnMove(InputValue value) {
            moveInput = value.Get<Vector2>();
        }
        void OnSwitchCharacter() {
            if (mouseScrollY > 0) {
                characterManager.SwitchToNextCharacter();
            }
            else if (mouseScrollY < 0) {
                characterManager.SwitchToPreviousCharacter();
            }
        }
        void OnSwitchFollowTarget() {
            if (isAbleToSwitchFollowTarget) {
                GetComponent<AIController>().SwitchTargetState();
            }
            else{
                isAbleToSwitchFollowTarget = true;
            }
        }
        void OnFollowMe(InputValue value) {
            characterManager.AllPlayerCharactersFollowCurrentPlayer();
            isAbleToSwitchFollowTarget = false;
        }
    }
}