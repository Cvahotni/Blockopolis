using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WorldPlayerSaveLoad
{
    private static string playerDataJsonBuffer = "";

    public static void SaveWorldPlayer(World world, GameObject player) {
        WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.playerFileName);
        string path = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);
        
        PlayerData playerData = new PlayerData(player);
        string json = JsonUtility.ToJson(playerData);

        File.WriteAllText(path, json);
        playerDataJsonBuffer = "";
    }

    private static PlayerData LoadPlayerData(World world) {
        if(playerDataJsonBuffer == "") {
            WorldSaveLoad.CheckWorldLoadFile(world.Name, WorldStorageProperties.playerFileName);

            string loadPath = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.playerFileName);
            string json = File.ReadAllText(loadPath);
    
            playerDataJsonBuffer = json;
            return JsonUtility.FromJson<PlayerData>(json);
        }

        else return JsonUtility.FromJson<PlayerData>(playerDataJsonBuffer);
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
