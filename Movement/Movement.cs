using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace coldheart_movement {
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Movement : MonoBehaviour {
        [SerializeField] float moveSpeed = 10f;
        Rigidbody rb;
        NavMeshAgent navMeshAgent;
        Vector3 playerVelocity;
        Ray screenPointRay;
        RaycastHit hit;
        GameObject objectHit;
        void Start() {
            rb = GetComponent<Rigidbody>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }
        public void MoveCharacter(Vector2 moveInput) {
            playerVelocity = new Vector3(moveInput.x, 0f, moveInput.y);
            rb.velocity = playerVelocity * moveSpeed;
        }
        public void LookAtCursor() {
            screenPointRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hasHit = Physics.Raycast(screenPointRay, out hit);

            if (hasHit) {
                objectHit = hit.transform.gameObject;
                if (objectHit != gameObject) {
                    transform.LookAt(new Vector3(hit.point.x, 0f, hit.point.z));
                }
            }
        }
        public void FollowTarget(GameObject target) {
            if (navMeshAgent != null) {
                navMeshAgent.destination = target.transform.position;
            }
        }
    }
}
