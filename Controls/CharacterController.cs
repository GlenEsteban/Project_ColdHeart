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
    [RequireComponent(typeof(FollowerAIController))]
    [RequireComponent(typeof(EnemyAIController))]
    public class CharacterController : MonoBehaviour
    {
        bool isControlledByPlayer;
        bool isControlStateUpdated;
        bool isAbleToSwitchFollowTarget = true;
        [HideInInspector] public float mouseScrollY;
        CharacterManager characterManager;
        PlayerInput playerInput;
        PlayerControls playerControls;
        Movement movement;
        Vector2 moveInput;
        Combat combat;
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            playerInput = GetComponent<PlayerInput>();
            movement = GetComponent<Movement>();
            combat = GetComponent<Combat>();
        }
        void OnEnable() {
            RegisterCharacter();

            characterManager.onSwitchCharacterAction += UpdateControlStatus;
            playerControls = new PlayerControls();
            playerControls.Player.SwitchCharacter.performed += x => mouseScrollY = x.ReadValue<float>();
            playerControls.Enable();
        }
        void OnDisable() {
            if (tag == "Player") {
                characterManager.UnregisterPlayerCharacter(gameObject);
            }
            else if (tag == "Enemy") {
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

            if (tag == "Player") {
                characterManager.RegisterPlayerCharacter(gameObject, isControlledByPlayer);
            }
            else if (tag == "Enemy") {
                characterManager.RegisterEnemyCharacter(gameObject);
            }
            else {
                print("Character is untagged!!!");
            }
        }
        void UpdateControlStatus() {
            if (tag == "Player") {
                GetComponent<CharacterController>().enabled = true;
                GetComponent<FollowerAIController>().enabled = true;
                GetComponent<EnemyAIController>().enabled = false;
            }
            else if (tag == "Enemy") {
                GetComponent<CharacterController>().enabled = false;
                GetComponent<FollowerAIController>().enabled = false;
                GetComponent<EnemyAIController>().enabled = true;
            }
            else {
                GetComponent<CharacterController>().enabled = false;
                GetComponent<FollowerAIController>().enabled = false;
                GetComponent<EnemyAIController>().enabled = false;
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
                GetComponent<FollowerAIController>().SwitchTargetState();
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