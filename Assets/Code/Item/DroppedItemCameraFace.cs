using UnityEngine;

public class DroppedItemCameraFace : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;

    private void Update() {
        RotateItemTowardsCamera();
    }

    private void RotateItemTowardsCamera() {
        Vector3 cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0.0f;

        if(cameraDirection == Vector3.zero) return;
        itemTransform.rotation = Quaternion.LookRotation(cameraDirection);
    }
}
