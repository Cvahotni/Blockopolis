using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionPositionHelper
{
    public static long GetRegionPos(int x, int z) {
        int rx = x;
        int rz = z;

        rx += VoxelProperties.worldSizeInRegionsHalved;
        rz += VoxelProperties.worldSizeInRegionsHalved;

        if(IsRegionOutsideOfWorldBounds(x, z)) return int.MaxValue;
        return rx * VoxelProperties.worldSizeInRegions + rz;
    }

    public static int GetRegionPosX(long pos) {
        if(pos == int.MaxValue) return int.MaxValue;
        return (int) (pos / VoxelProperties.worldSizeInRegions) - VoxelProperties.worldSizeInRegionsHalved;
    }

    public static int GetRegionPosZ(long pos) {
        if(pos == int.MaxValue) return int.MaxValue;
        return (int) (pos % VoxelProperties.worldSizeInRegions) - VoxelProperties.worldSizeInRegionsHalved;
    }

    public static long ChunkPosToRegionPos(long chunkPos) {
        int chunkPosWX = ChunkPositionHelper.GetChunkPosWX(chunkPos);
        int chunkPosWZ = ChunkPositionHelper.GetChunkPosWZ(chunkPos);

        int regionPosX = chunkPosWX / VoxelProperties.regionWidth;
        int regionPosZ = chunkPosWZ / VoxelProperties.regionWidth;

        return GetRegionPos(regionPosX, regionPosZ);
    }

    private static bool IsRegionOutsideOfWorldBounds(int x, int z) {
        return x < -VoxelProperties.worldSizeInRegionsHalved || x > VoxelProperties.worldSizeInRegionsHalved ||
            z < -VoxelProperties.worldSizeInRegionsHalved || z > VoxelProperties.worldSizeInRegionsHalved;
    }
}
