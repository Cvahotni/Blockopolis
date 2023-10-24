using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private Transform[] groundChecks;

    [SerializeField] private float checkDistance = 1.0f;
    [SerializeField] private LayerMask groundLayerMask;

    private PlayerMove playerMove;

    private void Start() {
        playerMove = PlayerMove.Instance;
    }

    private void FixedUpdate() {
        playerMove.IsGrounded = IsGroundedViaRaycast();
        DrawDebugRays();
    }

    private bool IsGroundedViaRaycast() {
        foreach(Transform transform in groundChecks) {
            RaycastHit hit;

            if(Physics.Raycast(transform.position, 
                                Vector3.down,
                                out hit,
                                checkDistance,
                                groundLayerMask)) {

                return true;
            }
        }

        return false;
    }

    private void DrawDebugRays() {
        foreach(Transform transform in groundChecks) {
            Debug.DrawRay(transform.position, Vector3.down * checkDistance, Color.yellow);
        }
    }
}
 