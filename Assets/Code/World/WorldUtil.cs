using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtil
{
    public static int GetMaxYLevelAt(int worldX, int worldZ) {
        for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
            ushort currentBlock = WorldModifier.GetBlockAt(worldX, y, worldZ);
            if(currentBlock != 0) return y;
        }

        return VoxelProperties.chunkHeight;
    }

    public static bool IsBelowSeaLevel(int y) {
        return y < 66;
    }
    
    public static float GetRealWorldX(int x) {
        return x + (VoxelProperties.worldSize / 2);
    }

    public static float GetRealWorldZ(int z) {
        return z + (VoxelProperties.worldSize / 2);
    }
}
