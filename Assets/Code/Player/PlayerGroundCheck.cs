using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private float checkDistance = 1.0f;
    [SerializeField] private LayerMask groundLayerMask;

    private PlayerEventSystem playerEventSystem;
    private RaycastHit[] results = new RaycastHit[1];

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;
    }

    private void FixedUpdate() {
        playerEventSystem.InvokeGroundCheck(IsGroundedViaRaycast());
        DrawDebugRays();
    }

    private bool IsGroundedViaRaycast() {
        foreach(Transform transform in groundChecks) {
            if(Physics.RaycastNonAlloc(transform.position,
                                Vector3.down,
                                results,
                                checkDistance,
                                groundLayerMask) != 0) {

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
 