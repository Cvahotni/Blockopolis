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
        int yLevel = WorldUtil.GetMaxYLevelAt(spawnX, spawnZ);
        return new Vector3(spawnX + 0.5f, yLevel + 2.0f, spawnZ + 0.5f);
    }
}
