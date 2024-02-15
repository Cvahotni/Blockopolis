using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WorldInventorySaveLoad
{
    public static void SaveWorldInventory(World world, Inventory inventory) {
        WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.inventoryFileName);
        string path = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.inventoryFileName);
        
        InventoryData inventoryData = new InventoryData(inventory);
        string json = JsonUtility.ToJson(inventoryData);

        File.WriteAllText(path, json);
    }

    public static void LoadWorldInventory(World world, Inventory inventory) {
        WorldSaveLoad.CheckWorldLoadFile(world.Name, WorldStorageProperties.inventoryFileName);

        string loadPath = WorldSaveLoad.GetWorldFilePath(world, WorldStorageProperties.inventoryFileName);
        string json = File.ReadAllText(loadPath);
    
        InventoryData inventoryData = JsonUtility.FromJson<InventoryData>(json);

        for(int i = 0; i < inventoryData.Stacks.Length; i++) {
            inventory.SetStack(i, inventoryData.Stacks[i]);
        }
    }
}
