using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WorldInventorySaveLoad
{
    public static void SaveWorldInventory(World world, Inventory inventory) {
        string path = WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.inventoryFileName);

        using (StreamWriter writer = new StreamWriter(path, append: true)) {
            for(int i = 0; i < inventory.Slots.Length; i++) {
                UiItemSlot uiSlot = inventory.Slots[i];
                ItemSlot itemSlot = uiSlot.ItemSlot;
                ItemStack stack = itemSlot.Stack;

                string line = GetSlotLine(i);

                string idLine = line + ".id." + stack.ID;
                string amountLine = line + ".amount." + stack.Amount;

                writer.WriteLine(idLine);
                writer.WriteLine(amountLine);
            }
        }
    }

    public static void LoadWorldInventory(World world, Inventory inventory) {
        string loadPath = WorldSaveLoad.CheckWorldLoadFile(world.Name, WorldStorageProperties.inventoryFileName);
        string line;

        int inventorySize = inventory.Slots.Length;
        
        bool readID = false;
        bool readAmount = false;

        ushort currentID = 0;
        ushort currentAmount = 0;

        int currentIndex = -1;

        using (StreamReader reader = new StreamReader(loadPath)) while((line = reader.ReadLine()) != null) {
            string[] splitString = line.Split(".");

            int index = Int32.Parse(splitString[1]);
            string typeData = splitString[3];

            if(readID && readAmount) {
                UiItemSlot uiSlot = inventory.Slots[currentIndex];
                ItemSlot itemSlot = uiSlot.ItemSlot;

                itemSlot.Stack = new ItemStack(currentID, currentAmount);
                itemSlot.UpdateEmptyStatus();

                uiSlot.UpdateSlot(true);

                readID = false;
                readAmount = false;
            }

            if(line.Contains("id")) {
                currentID = UInt16.Parse(typeData);
                readID = true;
            }

            if(line.Contains("amount")) {
                currentAmount = UInt16.Parse(typeData);
                readAmount = true;
            }

            currentIndex = index;
        }
    }

    private static string GetSlotLine(int i) {
        return "slot." + i;
    }
}
