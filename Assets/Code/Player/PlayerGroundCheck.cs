using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    [SerializeField] private Transform[] groundChecks;

    [SerializeField] private float checkDistance = 1.0f;
    [SerializeField] private LayerMask groundLayerMask;

    private PlayerEventSystem playerEventSystem;

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;
    }

    private void FixedUpdate() {
        playerEventSystem.InvokeGroundCheck(IsGroundedViaRaycast());
        DrawDebugRays();
    }

    private bool IsGroundedViaRaycast() {
        bool voxelCheck = false;

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
 