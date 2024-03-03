using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public class PlayerData
{
    [SerializeField] private float3 position;
    [SerializeField] private int heldSlotIndex;
    [SerializeField] private float xRotation;
    [SerializeField] private float yRotation;

    public float3 Position { get { return position; }}
    public int HeldSlotIndex { get { return heldSlotIndex; }}
    public float XRotation { get { return xRotation; }}
    public float YRotation { get { return yRotation; }}

    public PlayerData(GameObject playerObject) {
        position = new float3(playerObject.transform.position.x, 
                            playerObject.transform.position.y, 
                            playerObject.transform.position.z);

        MouseLook mouseLook = MouseLook.Instance;
        Hotbar hotbar = Hotbar.Instance;

        if(hotbar != null) {
            heldSlotIndex = hotbar.SlotIndex;
        }

        if(mouseLook != null) {
            xRotation = mouseLook.XRotation;
            yRotation = mouseLook.YRotation;
        }
    }
}
