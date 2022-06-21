using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using coldheart_movement;
using UnityEngine.InputSystem;

namespace coldheart_controls {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Movement))]
    public class PlayerController : MonoBehaviour
    {
        Movement movement;
        Rigidbody rb;
        Vector2 moveInput;
        Vector2 mousePosition;
        void Start() {
            rb = GetComponent<Rigidbody>();
            movement = GetComponent<Movement>();
        }
        void Update() {
            movement.MoveCharacter(moveInput);
            movement.LookAtCursor();
        }
        void OnMove(InputValue value) {
            moveInput = value.Get<Vector2>();
        }
    }
}
