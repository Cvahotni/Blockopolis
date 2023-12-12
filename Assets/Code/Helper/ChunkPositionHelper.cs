using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPositionHelper
{
    public static long GetChunkPos(int x, int z) {
        int rx = x;
        int rz = z;

        if(IsChunkOutsideOfWorldBounds(x, z)) return int.MaxValue;

        rx += VoxelProperties.worldSizeInChunksHalved;
        rz += VoxelProperties.worldSizeInChunksHalved;

        return rx * VoxelProperties.worldSizeInChunks + rz;
    }

    public static int GetChunkPosX(long pos) {
        if(pos == int.MaxValue) return int.MaxValue;
        return (int) (pos / VoxelProperties.worldSizeInChunks) - VoxelProperties.worldSizeInChunksHalved;
    }

    public static int GetChunkPosZ(long pos) {
        if(pos == int.MaxValue) return int.MaxValue;
        return (int) (pos % VoxelProperties.worldSizeInChunks) - VoxelProperties.worldSizeInChunksHalved;
    }

    public static long ModifyChunkPos(long chunkPos, int modX, int modZ) {
        int chunkPosX = GetChunkPosX(chunkPos) + modX;
        int chunkPosZ = GetChunkPosZ(chunkPos) + modZ;

        return GetChunkPos(chunkPosX, chunkPosZ);
    }

    public static int GetChunkPosWX(long pos) {
        return GetChunkPosX(pos) * VoxelProperties.chunkWidth;
    }

    public static int GetChunkPosWZ(long pos) {
        return GetChunkPosZ(pos) * VoxelProperties.chunkWidth;
    }

    public static string ChunkCoordToString(long coord) {
        return GetChunkPosX(coord) + ", " + GetChunkPosZ(coord);    
    }

    private static bool IsChunkOutsideOfWorldBounds(int x, int z) {
        return x < -VoxelProperties.worldSizeInChunksHalved || x > VoxelProperties.worldSizeInChunksHalved ||
            z < -VoxelProperties.worldSizeInChunksHalved || z > VoxelProperties.worldSizeInChunksHalved;
    }
}
