using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public class PlayerData
{
    [SerializeField] private float3 position;
    [SerializeField] private int heldSlotIndex;

    public float3 Position { get { return position; }}
    public int HeldSlotIndex { get { return heldSlotIndex; }}

    public PlayerData(GameObject playerObject) {
        position = new float3(playerObject.transform.position.x, 
                            playerObject.transform.position.y, 
                            playerObject.transform.position.z);

        Hotbar hotbar = Hotbar.Instance;

        if(hotbar != null) {
            heldSlotIndex = hotbar.SlotIndex;
        }
    }
}
