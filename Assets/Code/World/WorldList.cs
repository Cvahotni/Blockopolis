using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WorldList
{
    private static List<string> worldNames = new List<string>();

    public static void Add(string name) {
        worldNames.Add(name);
    }

    public static int Count() {
        return worldNames.Count;
    }

    public static void Delete(string name) {
        worldNames.Remove(name);
    }

    public static void Rename(string original, string newName) {
        for(int i = 0; i < worldNames.Count; i++) {
            string currentName = worldNames[i];

            if(currentName == original) {
                worldNames[i] = newName;
                break;       
            }
        }
    }

    public static string At(int i) {
        return worldNames[i];
    }

    public static void Populate() {
        #if !UNITY_WEBGL
        PopulateStandard();

        #endif
    }

    private static void PopulateStandard() {
        string currentDirectory = Directory.GetCurrentDirectory();
        string path = currentDirectory + Path.DirectorySeparatorChar + WorldStorageProperties.savesFolderSecondaryName;

        if(!Directory.Exists(WorldStorageProperties.savesFolderSecondaryName)) return;
        string[] subDirectories = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

        foreach(string subDirectory in subDirectories) {
            string[] splitName = subDirectory.Split(Path.DirectorySeparatorChar);
            string currentWorldName = splitName[splitName.Length - 1];

            if(!WorldHandler.DoesWorldExist(currentWorldName)) continue;
            if(worldNames.Contains(currentWorldName)) continue;

            Debug.Log("Adding world to list: " + currentWorldName);
            worldNames.Add(currentWorldName);
        }

        for(int i = worldNames.Count - 1; i >= 0; i--) {
            string worldName = worldNames[i];

            if(!WorldHandler.DoesWorldExist(worldName) || !WorldHandler.IsWorldValid(worldName)) {
                Debug.Log("Removing world from list: " + worldName);
                worldNames.RemoveAt(i);
            }
        }
    }
}
