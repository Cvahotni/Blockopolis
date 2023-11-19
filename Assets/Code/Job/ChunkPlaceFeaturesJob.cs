using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkPlaceFeaturesJob : IJob
{
    public NativeArray<long> coord;
    public NativeArray<ushort> voxelMap;

    public void Execute() {
        PlaceFeatures();
    }

    private void PlaceFeatures() {
        for(int x = 0; x < VoxelProperties.chunkWidth; x++) {
            for(int z = 0; z < VoxelProperties.chunkWidth; z++) {
                float worldX = GetWorldX(x);
                float worldZ = GetWorldZ(z);

                int y = GetHighestY(x, z);  
            }
        }
    }

    private int GetHighestY(int x, int z) {
        for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
            if(voxelMap[voxelMapArrayIndex] != 0) return y;
        }

        return VoxelProperties.chunkHeight;
    }

    private float GetWorldX(int x) {
        return GetWorldXOriginal(x) + (VoxelProperties.worldSize / 2);
    }

    private float GetWorldZ(int z) {
        return GetWorldZOriginal(z) + (VoxelProperties.worldSize / 2);
    }

    private float GetWorldXOriginal(int x) {
        return ChunkPositionHelper.GetChunkPosWX(coord[0]) + x;
    }

    private float GetWorldZOriginal(int z) {
        return ChunkPositionHelper.GetChunkPosWZ(coord[0]) + z;
    }
}
