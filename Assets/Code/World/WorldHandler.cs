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
        currentWorld = WorldInfoSaveLoad.LoadWorldInfo(name);
    }

    public static void LoadWorldInventory() {
        Inventory inventory = Inventory.Instance;

        if(inventory == null) {
            Debug.Log("Can't find a valid inventory, consider adding one to the scene.");
            return;
        }

        WorldInventorySaveLoad.LoadWorldInventory(currentWorld, inventory);
    }

    public static void SaveCurrentWorld() {
        Inventory inventory = Inventory.Instance;

        WorldInfoSaveLoad.SaveWorldInfo(currentWorld);
        
        if(inventory != null) {
            WorldInventorySaveLoad.SaveWorldInventory(currentWorld, inventory);
        }

        else {
            Debug.Log("Can't find a valid inventory, consider adding one to the scene. The inventory will not be saved because of this.");
        }

        WorldStorage.SaveRegions(currentWorld);
        Debug.Log("Saved world: " + currentWorld.Name);
    }

    public static void RenameCurrentWorld(string newName) {
        if(!IsCurrentWorldValid()) return;

        string currentPath = WorldStorageProperties.savesFolderName + currentWorld.Name;
        string newPath = WorldStorageProperties.savesFolderName + newName;

        WorldList.Rename(currentWorld.Name, newName);

        currentWorld.Name = newName;
        Directory.Move(currentPath, newPath);
        
        WorldInfoSaveLoad.SaveWorldInfo(currentWorld);
        Debug.Log("Renamed world: " + currentWorld.Name);
    }

    public static void DeleteCurrentWorld() {
        if(!IsCurrentWorldValid()) return;

        WorldList.Delete(currentWorld.Name);
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
