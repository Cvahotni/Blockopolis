using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct ChunkRunLengthDecoderJob : IJob
{
    public NativeArray<ushort> decodedVoxelMap;
    public NativeList<EncodedVoxelMapEntry> encodedVoxelMap;

    public void Execute() {
        DecodeVoxelMap();
    }

    private void DecodeVoxelMap() {
        int arrayIndex = 0;

        for(int i = 0; i < encodedVoxelMap.Length; i++) {
            ushort id = encodedVoxelMap[i].id;
            ushort count = encodedVoxelMap[i].count;

            for(ushort j = 0; j < count; j++) {
                decodedVoxelMap[arrayIndex] = id;
                arrayIndex++;
            }
        }
    }
}