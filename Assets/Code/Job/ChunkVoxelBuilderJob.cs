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
        int seaLevel = 64;
        int soilLevel = 6;

        float yOffset = -16.0f;

        for(int x = -1; x <= VoxelProperties.chunkWidth; x++) {
            for(int z = -1; z <= VoxelProperties.chunkWidth; z++) {
                float worldX = GetWorldX(x);
                float worldZ = GetWorldZ(z);

                float noiseLevel = Get2DNoise(worldX, worldZ) + yOffset;
                int yLevel = (int) (noiseLevel);

                for(int y = 0; y < VoxelProperties.chunkHeight; y++) {
                    int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);

                    if(y == yLevel && yLevel > seaLevel) voxelMap[voxelMapArrayIndex] = 1;
                    else if(y == yLevel && yLevel <= seaLevel) voxelMap[voxelMapArrayIndex] = 5;

                    else if(y == 0) voxelMap[voxelMapArrayIndex] = 4;

                    else if(y < yLevel && y >= yLevel - soilLevel && yLevel > seaLevel) voxelMap[voxelMapArrayIndex] = 2;
                    else if(y < yLevel && y >= yLevel - soilLevel && yLevel <= seaLevel) voxelMap[voxelMapArrayIndex] = 6;

                    else if(y < yLevel - soilLevel ) voxelMap[voxelMapArrayIndex] = 3;
                }
            }
        }
    }

    private float Get2DNoise(float worldX, float worldZ) {
        int frequenciesCount = frequencies.Length;
        float totalNoiseValue = 0.0f;

        float terrainNoiseOffsetX = noiseOffset[0];
        float terrainNoiseOffsetZ = noiseOffset[1];

        for(int i = 0; i < frequenciesCount; i++) {
            float currentFrequency = frequencies[i];
            float currentAmplitude = amplitudes[i];
        
            totalNoiseValue += Noise.Get2DNoiseAt(terrainNoiseOffsetX, terrainNoiseOffsetZ, worldX, worldZ, currentAmplitude, currentFrequency);
        }

        return totalNoiseValue;
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
