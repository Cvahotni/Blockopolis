using UnityEngine;
using System.IO;
using System;

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

    public static bool LoadWorld(string name) {
        Debug.Log("Loading world: " + name);

        if(!IsWorldValid(name)) {
            Debug.Log("Failed to load world: " + name);
            return false;
        }

        currentWorld = WorldInfoSaveLoad.LoadWorldInfo(name);
        return true;
    }

    public static void SaveCurrentWorldQuickly(object sender, EventArgs e) {
        SaveCurrentWorld(true);
    }

    public static void SaveCurrentWorldQuickly(object sender, bool var) {
        SaveCurrentWorld(true);
    }

    public static void SaveCurrentWorld(bool saveQuickly) {
        Inventory inventory = Inventory.Instance;
        WorldInfoSaveLoad.SaveWorldInfo(currentWorld);
        
        if(inventory != null) {
            WorldInventorySaveLoad.SaveWorldInventory(currentWorld, inventory);
        }

        else {
            Debug.Log("Can't find a valid inventory, consider adding one to the scene. The inventory will not be saved because of this.");
        }

        WorldStorage.SaveRegions(currentWorld, saveQuickly);
        Debug.Log("Saving world: " + currentWorld.Name);
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
        bool worldInfoExists = File.Exists(path + WorldStorageProperties.worldInfoFileName);

        return directoryExists && worldInfoExists;
    }

    public static bool IsCurrentWorldValid() {
        return IsWorldValid(currentWorld.Name);
    }

    public static bool IsCurrentWorldInfoValid() {
        return IsWorldInfoValid(currentWorld.Name);
    }

    public static bool IsWorldValid(string name) {
        if(name == null) return false;
        bool directoryInvalid = !Directory.Exists(WorldStorageProperties.savesFolderName + name);
        
        return IsWorldInfoValid(name) && !directoryInvalid;
    }

    public static bool IsWorldInfoValid(string name) {
        if(name == null) return false;

        bool nameInvalid = name == "";
        bool nameTooLong = name.Length > WorldStorageProperties.worldNameLimit;

        return !nameInvalid && !nameTooLong;
    }

    private static int HashStringSeed(string seed) {
        return seed.GetHashCode();
    }
}
