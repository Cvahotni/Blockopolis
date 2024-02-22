using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChunkCheck : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;

    private void Start() {
        InvokeRepeating("Check", 0.05f, 0.1f);
    }

    private void Check() {
        int playerX = Mathf.FloorToInt(transform.position.x) >> VoxelProperties.chunkBitShift;
        int playerZ = Mathf.FloorToInt(transform.position.z) >> VoxelProperties.chunkBitShift;

        long chunkPos = ChunkPositionHelper.GetChunkPos(playerX, playerZ);
        rigidbody.isKinematic = !(WorldStorage.DoesChunkExist(chunkPos) && ChunkObjectExists(chunkPos));
    }
    
    private bool ChunkObjectExists(long coord) {
        return GameObject.Find("" + coord) != null;
    }
}
