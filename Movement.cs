using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace coldheart_movement {
    public class Movement : MonoBehaviour {
        [SerializeField] float moveSpeed = 10f;
        Rigidbody rb;
        Vector3 playerVelocity;
        Ray screenPointRay;
        RaycastHit hit;
        GameObject objectHit;
        void Start() {
            rb = GetComponent<Rigidbody>();
        }
        public void MoveCharacter(Vector2 moveInput) {
            playerVelocity = new Vector3(moveInput.x, 0f, moveInput.y);
            rb.velocity = playerVelocity * moveSpeed;
        }
        public void LookAtCursor() {
            screenPointRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hasHit = Physics.Raycast(screenPointRay, out hit);
            objectHit = hit.transform.gameObject;
            if (hasHit && objectHit != gameObject) {
                transform.LookAt(hit.point);
            }
        }
    }
}
