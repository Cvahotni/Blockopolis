using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class WorldInventorySaveLoad
{
    private static readonly int inventoryEntryByteSize = 2;
    private static readonly int inventorySlotByteSize = (inventoryEntryByteSize * 2) + 1;

    public static void SaveWorldInventory(World world, Inventory inventory) {
        string path = WorldSaveLoad.CheckWorldSaveFile(world, WorldStorageProperties.inventoryFileName);
        byte[] inventoryBytes = new byte[InventoryProperties.slotCount * inventorySlotByteSize];

        for(int i = 0; i < inventory.Slots.Length; i++) {
            UIItemSlot uiSlot = inventory.Slots[i];
            ItemSlot itemSlot = uiSlot.ItemSlot;
            ItemStack stack = itemSlot.Stack;

            int inventoryBytesIndex = i * inventorySlotByteSize;
            byte[] typeBytes = BitConverter.GetBytes(stack.ID);
            byte[] amountBytes = BitConverter.GetBytes(stack.Amount);

            inventoryBytes[inventoryBytesIndex] = (byte) i;

            inventoryBytes[inventoryBytesIndex + 1] = typeBytes[0];
            inventoryBytes[inventoryBytesIndex + 2] = typeBytes[1];

            inventoryBytes[inventoryBytesIndex + 3] = amountBytes[0];
            inventoryBytes[inventoryBytesIndex + 4] = amountBytes[1];
        }

        using (var stream = new FileStream(path, FileMode.Append)) {
            stream.Write(inventoryBytes, 0, inventoryBytes.Length);
        }
    }

    public static void LoadWorldInventory(World world, Inventory inventory) {
        string loadPath = WorldSaveLoad.CheckWorldLoadFile(world.Name, WorldStorageProperties.inventoryFileName);
        byte[] buffer = new byte[InventoryProperties.slotCount * 5];

        using (FileStream fileStream = new FileStream(loadPath, FileMode.Open, FileAccess.Read)) {
            fileStream.Read(buffer, 0, (int) fileStream.Length);
        }

        for(int i = 0; i < buffer.Length; i += inventorySlotByteSize) {
            byte[] typeBytes = new byte[inventoryEntryByteSize];
            byte[] amountBytes = new byte[inventoryEntryByteSize];

            typeBytes[0] = buffer[i + 1];
            typeBytes[1] = buffer[i + 2];

            amountBytes[0] = buffer[i + 3];
            amountBytes[1] = buffer[i + 4];

            ushort id = BitConverter.ToUInt16(typeBytes, 0);
            ushort amount = BitConverter.ToUInt16(amountBytes, 0);

            inventory.SetStack(i / inventorySlotByteSize, new ItemStack(id, amount));
        }
    }
}
