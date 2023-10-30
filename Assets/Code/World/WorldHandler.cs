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
        Debug.Log("currentWorld: " + currentWorld);
        Debug.Log("currentWorld Name: " + currentWorld.Name);

        WorldSaveLoad.SaveWorldInfo(currentWorld);
        WorldStorage.SaveRegions(currentWorld);

        Debug.Log("Saved world: " + currentWorld.Name);
    }

    public static bool DoesWorldExist(string name) {
        string path = WorldStorageProperties.savesFolderName + name;

        bool directoryExists = Directory.Exists(path);
        bool worldInfoExists = File.Exists(path + Path.DirectorySeparatorChar + "info.txt");

        return directoryExists && worldInfoExists;
    }

    private static int HashStringSeed(string seed) {
        return seed.GetHashCode();
    }
}