using UnityEngine;

public class WorldSpawner
{
    private static int spawnX = 0;
    private static int spawnZ = 0;

    private static bool isCached = false;
    private static Vector3 cachedSpawnPos;

    public static Vector3 CachedSpawnPos { set { 
        cachedSpawnPos = value;
        isCached = true;
    }}

    public static Vector3 GetPlayerSpawnLocation() {
        if(isCached) return cachedSpawnPos;

        int centralYLevel = WorldUtil.GetMaxYLevelAt(spawnX, spawnZ);
        return new Vector3(spawnX + 0.5f, centralYLevel + 2.0f, spawnZ + 0.5f);
    }
}
