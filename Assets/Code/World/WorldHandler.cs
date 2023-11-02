using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldHandler
{
    private static World currentWorld;

    public static World CurrentWorld { 
        get { return currentWorld; }
        set { currentWorld = value; }    
    }

    public static void CreateNewWorld(string name, string seed) {
        Debug.Log("Creating world: " + name);

        int currentSeed = HashStringSeed(seed);
        currentWorld = new World(name, currentSeed);
    }

    public static void LoadWorld(string name) {
        Debug.Log("Loading world: " + name);
        currentWorld = WorldSaveLoad.LoadWorldInfo(name);
    }

    public static void SaveCurrentWorld() {
        WorldSaveLoad.SaveWorldInfo(currentWorld);
        WorldStorage.SaveRegions(currentWorld);

        Debug.Log("Saved world: " + currentWorld.Name);
    }

    public static void RenameCurrentWorld(string newName) {
        if(!IsCurrentWorldValid()) return;

        string currentPath = WorldStorageProperties.savesFolderName + currentWorld.Name;
        string newPath = WorldStorageProperties.savesFolderName + newName;

        currentWorld.Name = newName;
        Directory.Move(currentPath, newPath);
        
        WorldSaveLoad.SaveWorldInfo(currentWorld);
        Debug.Log("Renamed world: " + currentWorld.Name);
    }

    public static void DeleteCurrentWorld() {
        if(!IsCurrentWorldValid()) return;

        Directory.Delete(WorldStorageProperties.savesFolderName + currentWorld.Name, true);
        Debug.Log("Deleted world: " + currentWorld.Name);
    }

    public static bool DoesWorldExist(string name) {
        string path = WorldStorageProperties.savesFolderName + name;

        bool directoryExists = Directory.Exists(path);
        bool worldInfoExists = File.Exists(path + Path.DirectorySeparatorChar + "info.txt");

        return directoryExists && worldInfoExists;
    }

    private static bool IsCurrentWorldValid() {
        return currentWorld.Name != "";
    }

    private static int HashStringSeed(string seed) {
        return seed.GetHashCode();
    }
}
