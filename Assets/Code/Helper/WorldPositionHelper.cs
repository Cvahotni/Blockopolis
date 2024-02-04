using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPositionHelper
{
    public static int GetRelativeChunkX(int chunkX) {
        int regionX = chunkX >> VoxelProperties.regionBitShift;
        return Mathf.Abs(chunkX - (regionX << VoxelProperties.chunkBitShift));
    }

    public static int GetRelativeChunkZ(int chunkZ) {
        int regionZ = chunkZ >> VoxelProperties.regionBitShift;
        return Mathf.Abs(chunkZ - (regionZ << VoxelProperties.chunkBitShift));
    }

    public static int GetRelativeX(int worldX) {
        int chunkX = worldX >> VoxelProperties.chunkBitShift;
        int chunkXMultiplied = chunkX << VoxelProperties.chunkBitShift;

        return Mathf.Abs(worldX - chunkXMultiplied);
    }

    public static int GetRelativeZ(int worldZ) {
        int chunkZ = worldZ >> VoxelProperties.chunkBitShift;
        int chunkZMultiplied = chunkZ << VoxelProperties.chunkBitShift;

        return Mathf.Abs(worldZ - chunkZMultiplied);
    }

    public static bool IsBlockOnChunkEdge(int relativeX, int relativeZ) {
        bool isEdgeX = relativeX == 0 || relativeX == VoxelProperties.chunkWidth - 1;
        bool isEdgeZ = relativeZ == 0 || relativeZ == VoxelProperties.chunkWidth - 1;

        return isEdgeX || isEdgeZ;
    }

    public static bool IsChunkOnRegionEdge(int relativeChunkX, int relativeChunkZ) {
        bool isEdgeX = relativeChunkX == 0 || relativeChunkX == VoxelProperties.regionWidthInChunks - 1;
        bool isEdgeZ = relativeChunkZ == 0 || relativeChunkZ == VoxelProperties.regionWidthInChunks - 1;

        return isEdgeX || isEdgeZ;
    }

    public static long BlockPositionToChunkPos(int worldX, int worldZ) {
        int chunkX = worldX >> VoxelProperties.chunkBitShift;
        int chunkZ = worldZ >> VoxelProperties.chunkBitShift;

        return ChunkPositionHelper.GetChunkPos(chunkX, chunkZ);
    }

    public static Border.BorderDirection GetBorderDirection(int relativeX, int relativeZ, int sectionWidth) {
        bool relativeXIsNotOnEdge = relativeX != 0 && relativeX != sectionWidth - 1;
        bool relativeZIsNotOnEdge = relativeZ != 0 && relativeZ != sectionWidth - 1;

        bool relativeXIsMin = relativeX == 0;
        bool relativeZIsMin = relativeZ == 0;

        int sectionWidthSubtracted = sectionWidth - 1;

        bool relativeXIsMax = relativeX == sectionWidthSubtracted;
        bool relativeZIsMax = relativeZ == sectionWidthSubtracted;

        if(relativeXIsMin && relativeZIsNotOnEdge) return Border.BorderDirection.LEFT;
        if(relativeXIsMax && relativeZIsNotOnEdge) return Border.BorderDirection.RIGHT;

        if(relativeZIsMin && relativeXIsNotOnEdge) return Border.BorderDirection.UP;
        if(relativeZIsMax && relativeXIsNotOnEdge) return Border.BorderDirection.DOWN;

        if(relativeXIsMin && relativeZIsMin) return Border.BorderDirection.UP_LEFT;
        if(relativeXIsMax && relativeZIsMin) return Border.BorderDirection.UP_RIGHT;

        if(relativeXIsMin && relativeZIsMax) return Border.BorderDirection.DOWN_LEFT;
        if(relativeXIsMax && relativeZIsMax) return Border.BorderDirection.DOWN_RIGHT;

        return Border.BorderDirection.UNDEFINED;
    }
}
