using UnityEngine;
using System.IO;

public class WorldPlayerSaveLoad
{
    public static void SaveWorldPlayer(World world, GameObject player) {
        WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.playerFileName);
        string path = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);
        
        PlayerData playerData = new PlayerData(player);
        string json = JsonUtility.ToJson(playerData);

        File.WriteAllText(path, json);
    }

    private static PlayerData LoadPlayerData(World world) {
        WorldSaveLoad.CheckWorldLoadFile(world.Name, WorldStorageProperties.playerFileName);

        string loadPath = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);
        string json = File.ReadAllText(loadPath);
    
        return JsonUtility.FromJson<PlayerData>(json);
    }

    public static void LoadWorldPlayer(World world, GameObject player) {
        PlayerData playerData = LoadPlayerData(world);
        WorldSpawner.CachedSpawnPos = new Vector3(playerData.Position.x, playerData.Position.y, playerData.Position.z);

        Hotbar hotbar = Hotbar.Instance;
        if(hotbar != null) hotbar.SlotIndex = playerData.HeldSlotIndex;
    }

    public static void LoadWorldPlayerRotation(World world) {
        PlayerData playerData = LoadPlayerData(world);

        PlayerStorage.XRotation = playerData.XRotation;
        PlayerStorage.YRotation = playerData.YRotation;
    }
}
