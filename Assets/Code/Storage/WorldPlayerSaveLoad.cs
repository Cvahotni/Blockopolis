using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

    public static void LoadWorldPlayer(World world, GameObject player) {
        WorldSaveLoad.CheckWorldLoadFile(world.Name, WorldStorageProperties.playerFileName);

        string loadPath = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);
        string json = File.ReadAllText(loadPath);
    
        PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
        WorldSpawner.CachedSpawnPos = new Vector3(playerData.Position.x, playerData.Position.y, playerData.Position.z);

        Hotbar hotbar = Hotbar.Instance;
        if(hotbar != null) hotbar.SlotIndex = playerData.HeldSlotIndex;
    }
}
