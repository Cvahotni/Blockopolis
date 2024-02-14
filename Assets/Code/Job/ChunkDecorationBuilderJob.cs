using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;

[BurstCompile]
public struct ChunkDecorationBuilderJob : IJob
{
    public NativeArray<long> coord;
    public NativeArray<ushort> voxelMap;

    public NativeList<float> frequencies;
    public NativeList<float> amplitudes;

    public NativeArray<float> noiseOffset;

    public void Execute() {
        AddDecorations();
    }

    private void AddDecorations() {
        int soilLevel = 6;
        int seaLevel = 64;

        float yOffset = -16.0f;

        for(int x = 0; x < VoxelProperties.chunkWidth; x++) {
            for(int z = 0; z < VoxelProperties.chunkWidth; z++) {
                float worldX = GetWorldX(x);
                float worldZ = GetWorldZ(z);

                float noiseLevel = Noise.Get2DNoise(worldX, worldZ, noiseOffset[0], noiseOffset[1], frequencies, amplitudes) + yOffset;
                float randomNoiseLevel = Noise.Get2DNoiseAt(noiseOffset[0], noiseOffset[1], worldX, worldZ, 1.0f, 16.0f);

                int yLevel = (int) (noiseLevel);

                for(int y = 0; y < VoxelProperties.chunkHeight; y++) {
                    if(y == yLevel + 1 && yLevel > seaLevel && randomNoiseLevel >= 0.7f) {
                        ushort voxel = GetVoxel(x, y, z);
                        byte id = BlockIDHelper.ID(voxel);

                        if(id == 0) PutVoxel(x, y, z, 24, 0);
                    }
                }
            }
        }
    }

    private ushort GetVoxel(int x, int y, int z) {
        int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
        return voxelMap[voxelMapArrayIndex];
    }

    private void PutVoxel(int x, int y, int z, byte value, byte variant) {
        int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
        voxelMap[voxelMapArrayIndex] = BlockIDHelper.Pack(value, variant, 0);
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
