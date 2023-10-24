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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Start() {
        string worldName = "test";

        if(DoesWorldExist(worldName)) currentWorld = LoadWorld(worldName);
        else currentWorld = CreateNewWorld(worldName, "test");

        //Remove this later when adding UI
    }

    public static World CreateNewWorld(string name, string seed) {
        Debug.Log("Creating world: " + name);

        int currentSeed = HashStringSeed(seed);
        return new World(name, currentSeed);
    }

    public static World LoadWorld(string name) {
        Debug.Log("Loading world: " + name);
        return WorldSaveLoad.LoadWorldInfo(name);
    }

    public static void SaveCurrentWorld() {
        WorldSaveLoad.SaveWorldInfo(currentWorld);
        WorldStorage.SaveRegions(currentWorld);

        Debug.Log("Saved world: " + currentWorld.Name);
    }

    public static bool DoesWorldExist(string name) {
        return Directory.Exists(name);
    }

    private static int HashStringSeed(string seed) {
        return seed.GetHashCode();
    }
}
