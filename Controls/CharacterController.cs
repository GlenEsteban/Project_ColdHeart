using UnityEngine;
using UnityEngine.InputSystem;
using coldheart_core;
using coldheart_movement;
using coldheart_combat;

namespace coldheart_controls
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(Movement))]
    [RequireComponent(typeof(Combat))]
    [RequireComponent(typeof(FollowerAIController))]
    [RequireComponent(typeof(EnemyAIController))]
    public class CharacterController : MonoBehaviour
    {
        bool isControlledByPlayer;
        bool isAbleToSwitchFollowTarget = true;
        [HideInInspector] public float mouseScrollY;
        CharacterManager characterManager;
        PlayerInput playerInput;
        PlayerInputActions playerInputActions;
        FollowerAIController followerAIController;
        EnemyAIController enemyAIController;
        Movement movement;
        Vector2 moveInput;
        Combat combat;
        void Awake() {
            characterManager = FindObjectOfType<CharacterManager>();
            playerInput = GetComponent<PlayerInput>();
            followerAIController = GetComponent<FollowerAIController>();
            enemyAIController =GetComponent<EnemyAIController>();
            movement = GetComponent<Movement>();
            combat = GetComponent<Combat>();
        }
        void OnEnable() {
            RegisterCharacter();

            characterManager.onSwitchCharacterAction += UpdateControlStatus;
            playerInputActions = new PlayerInputActions();
            playerInputActions.Player.SwitchCharacter.performed += x => mouseScrollY = x.ReadValue<float>();
            playerInputActions.Player.ChargeUpAbility.started += x => combat.SetIsChargingUpAbility(true);
            playerInputActions.Player.ChargeUpAbility.canceled += x => combat.SetIsChargingUpAbility(false);

            playerInputActions.Enable();
        }
        void OnDisable() {
            UnregisterCharacter();

            characterManager.onSwitchCharacterAction -= UpdateControlStatus;
            playerInputActions.Player.SwitchCharacter.performed -= x => mouseScrollY = x.ReadValue<float>();
            playerInputActions.Player.ChargeUpAbility.started -= x => combat.SetIsChargingUpAbility(true);
            playerInputActions.Player.ChargeUpAbility.canceled -= x => combat.SetIsChargingUpAbility(false);
            
            playerInputActions.Disable();
        }
        void Update() {
            UpdateControlStatus();
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
        void UnregisterCharacter() {
            if (tag == "Player") {
                characterManager.UnregisterPlayerCharacter(gameObject);
            }
            else if (tag == "Enemy") {
                characterManager.UnregisterEnemyCharacter(gameObject);
            }
        }
        void UpdateControlStatus() {
            isControlledByPlayer = characterManager.GetCurrentPlayerCharacter() == gameObject;
            if (isControlledByPlayer) {
                playerInput.enabled = true;
                playerInputActions.Enable();
            }
            else {
                playerInput.enabled = false;
                playerInputActions.Disable();
            }
            
            if (tag == "Player" ) {
                enemyAIController.enabled = false;
                followerAIController.enabled = true;
                this.enabled = true;
            }
            else if (tag == "Enemy") {
                followerAIController.enabled = false;
                enemyAIController.enabled = true;
                this.enabled = false;
            }
            else {
                followerAIController.enabled = false;
                enemyAIController.enabled = false;
                this.enabled = false;
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
                followerAIController.SwitchTargetState();
            }
            else{
                isAbleToSwitchFollowTarget = true;
            }
        }
        void OnFollowMe() {
            characterManager.AllPlayerCharactersFollowCurrentPlayer();
            isAbleToSwitchFollowTarget = false;
            print ("Everyone is following " + gameObject.name);
        }
        void OnInstantAbility() {
            combat.CallThrottledInstantAbility();
        }

        // OnChargeUpAbility is handled via subscribing to its 
        // input action events, Started and Canceled, and then updating a bool
        // to toggle a timer.
    }
}