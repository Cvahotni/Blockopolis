using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class WorldInfoSaveLoad
{
    public static void SaveWorldInfo(World world) {
        WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.worldInfoFileName);
        string path = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.worldInfoFileName);
        
        WorldInfoData worldInfoData = new WorldInfoData(world);
        string json = JsonUtility.ToJson(worldInfoData);

        File.WriteAllText(path, json);
    }

    public static World LoadWorldInfo(string path) {
        UnityEngine.Debug.Log(path);
        WorldSaveLoad.CheckWorldLoadFile(path, WorldStorageProperties.worldInfoFileName);

        string loadPath = WorldSaveLoad.GetWorldFilePath(path, WorldStorageProperties.worldInfoFileName);
        string json = File.ReadAllText(loadPath);
    
        WorldInfoData worldInfoData = JsonUtility.FromJson<WorldInfoData>(json);
        return new World(worldInfoData.Name, worldInfoData.Seed);
    }
}