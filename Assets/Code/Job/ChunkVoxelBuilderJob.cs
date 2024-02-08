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

        for(int x = 0; x < VoxelProperties.chunkWidth; x++) {
            for(int z = 0; z < VoxelProperties.chunkWidth; z++) {
                float worldX = GetWorldX(x);
                float worldZ = GetWorldZ(z);

                float noiseLevel = Noise.Get2DNoise(worldX, worldZ, noiseOffset[0], noiseOffset[1], frequencies, amplitudes) + yOffset;
                int yLevel = (int) (noiseLevel);

                for(int y = 0; y < VoxelProperties.chunkHeight; y++) {
                    PutVoxel(x, y, z, 0, 0);

                    if(y == yLevel && yLevel > seaLevel) PutVoxel(x, y, z, 1, 0);
                    else if(y == yLevel && WorldUtil.IsBelowBeachLevel(yLevel)) PutVoxel(x, y, z, 5, 0);

                    else if(WorldUtil.IsAtSeaLevel(y) && y > yLevel) PutVoxel(x, y, z, 10, 0);
                    else if(WorldUtil.IsBelowSeaLevel(y) && y > yLevel) PutVoxel(x, y, z, 10, 1);

                    else if(y == 0) PutVoxel(x, y, z, 4, 0);

                    else if(y < yLevel && y >= yLevel - soilLevel && yLevel > seaLevel) PutVoxel(x, y, z, 2, 0);
                    else if(y < yLevel && y >= yLevel - soilLevel && yLevel <= seaLevel) PutVoxel(x, y, z, 6, 0);

                    else if(y < yLevel - soilLevel) PutVoxel(x, y, z, 3, 0);
                }
            }
        }
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
