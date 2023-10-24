using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionClamp : MonoBehaviour
{
    [SerializeField] private int positiveMargin = 4;
    [SerializeField] private int negativeMargin = -12;

    private void FixedUpdate() {
        ClampPosition();
    }

    private void ClampPosition() {
        float x = Mathf.Clamp(transform.position.x, -VoxelProperties.worldSizeHalved - negativeMargin, VoxelProperties.worldSizeHalved + positiveMargin);
        float z = Mathf.Clamp(transform.position.z, -VoxelProperties.worldSizeHalved - negativeMargin, VoxelProperties.worldSizeHalved + positiveMargin);
        float y = Mathf.Clamp(transform.position.y, VoxelProperties.worldMinY, VoxelProperties.worldMaxY);

        transform.position = new Vector3(x, y, z);
    }
}
