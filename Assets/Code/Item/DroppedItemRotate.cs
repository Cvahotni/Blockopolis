using UnityEngine;

public class DroppedItemRotate : MonoBehaviour
{
    [SerializeField] private Transform rotatingObject;
    [SerializeField] private float rotationSpeed;

    private void Update() {
        rotatingObject.Rotate(0.0f, rotationSpeed * Time.deltaTime * Time.timeScale, 0.0f);
    }
}
