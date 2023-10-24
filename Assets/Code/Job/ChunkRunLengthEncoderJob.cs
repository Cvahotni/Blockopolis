using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct ChunkRunLengthEncoderJob : IJob
{
    public NativeArray<ushort> voxelMap;
    public NativeList<EncodedVoxelMapEntry> encodedVoxelMap;

    public void Execute() {
        EncodeVoxelMap();
    }

    private void EncodeVoxelMap() {
        int n = voxelMap.Length;

        for(int i = 0; i < n; i++) {
            ushort count = 1;
            ushort id = voxelMap[i];

            while(i < n - 1 && id == voxelMap[i + 1]) {
                count++;
                i++;
            }

            encodedVoxelMap.Add(new EncodedVoxelMapEntry(id, count));
        }
    }
}