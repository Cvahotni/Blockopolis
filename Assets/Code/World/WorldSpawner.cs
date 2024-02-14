using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpawner
{
    private static int spawnX = 0;
    private static int spawnZ = 0;

    public static int SpawnX { get { return spawnX; }}
    public static int SpawnZ { get { return spawnZ; }}

    public static Vector3 GetPlayerSpawnLocation() {
        int centralYLevel = WorldUtil.GetMaxYLevelAt(spawnX, spawnZ);
        return new Vector3(spawnX + 0.5f, centralYLevel + 2.0f, spawnZ + 0.5f);
    }
}
