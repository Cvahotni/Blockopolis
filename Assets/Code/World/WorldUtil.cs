using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtil
{
    public static int YOffset { get { return -16; }}

    public static int GetMaxYLevelAt(int worldX, int worldZ) {
        for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
            BlockID currentBlockID = WorldAccess.GetBlockAt(worldX, y, worldZ);
            if(!currentBlockID.IsAir()) return y;
        }

        return VoxelProperties.chunkHeight;
    }

    public static bool IsBelowBeachLevel(int y) {
        return y < 66;
    }

    public static bool IsAtSeaLevel(int y) {
        return y == 63;
    }

    public static bool IsBelowSeaLevel(int y) {
        return y <= 62;
    }
    
    public static float GetRealWorldX(int x) {
        return x + (VoxelProperties.worldSize / 2);
    }

    public static float GetRealWorldZ(int z) {
        return z + (VoxelProperties.worldSize / 2);
    }
}
