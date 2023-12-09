using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkVoxelBuilderJob : IJob
{
    public NativeArray<long> coord;
    public NativeArray<ushort> voxelMap;

    public NativeList<float> frequencies;
    public NativeList<float> amplitudes;

    public NativeArray<float> noiseOffset;

    public void Execute() {
        PopulateVoxelMap();
    }

    private void PopulateVoxelMap() {
        int soilLevel = 6;
        int seaLevel = 64;

        float yOffset = -16.0f;

        for(int x = -1; x <= VoxelProperties.chunkWidth; x++) {
            for(int z = -1; z <= VoxelProperties.chunkWidth; z++) {
                float worldX = GetWorldX(x);
                float worldZ = GetWorldZ(z);

                float noiseLevel = Noise.Get2DNoise(worldX, worldZ, noiseOffset[0], noiseOffset[1], frequencies, amplitudes) + yOffset;
                int yLevel = (int) (noiseLevel);

                for(int y = 0; y < VoxelProperties.chunkHeight; y++) {
                    int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);

                    if(y == yLevel && yLevel > seaLevel) voxelMap[voxelMapArrayIndex] = 1;
                    else if(y == yLevel && WorldUtil.IsBelowSeaLevel(yLevel)) voxelMap[voxelMapArrayIndex] = 5;

                    else if(y == 0) voxelMap[voxelMapArrayIndex] = 4;

                    else if(y < yLevel && y >= yLevel - soilLevel && yLevel > seaLevel) voxelMap[voxelMapArrayIndex] = 2;
                    else if(y < yLevel && y >= yLevel - soilLevel && yLevel <= seaLevel) voxelMap[voxelMapArrayIndex] = 6;

                    else if(y < yLevel - soilLevel ) voxelMap[voxelMapArrayIndex] = 3;
                }
            }
        }
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
