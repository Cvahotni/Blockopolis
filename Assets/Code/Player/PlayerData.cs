using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public class PlayerData
{
    [SerializeField] private float3 position;
    public float3 Position { get { return position; }}

    public PlayerData(GameObject playerObject) {
        position = new float3(playerObject.transform.position.x, 
                            playerObject.transform.position.y, 
                            playerObject.transform.position.z);
    }
}
