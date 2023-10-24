using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner
{
    private static int spawnX = 0;
    private static int spawnZ = 0;

    public static int SpawnX { get { return spawnX; }}
    public static int SpawnZ { get { return spawnX; }}

    public static Vector3 GetPlayerSpawnLocation() {
        int yLevel = GetMaxYLevelAt(spawnX, spawnZ);
        return new Vector3(spawnX + 0.5f, yLevel + 2.0f, spawnZ + 0.5f);
    }

    private static int GetMaxYLevelAt(int worldX, int worldZ) {
        for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
            ushort currentBlock = WorldModifier.GetBlockAt(spawnX, y, spawnZ);
            if(currentBlock != 0) return y;
        }

        return VoxelProperties.chunkHeight;
    }
}
