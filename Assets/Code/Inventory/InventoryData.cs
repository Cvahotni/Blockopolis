using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryData
{
    [SerializeField] private ItemStack[] stacks;
    public ItemStack[] Stacks { get { return stacks; }}

    public InventoryData(Inventory inventory) {
        stacks = new ItemStack[InventoryProperties.slotCount];
    
        for(int i = 0; i < inventory.Slots.Length; i++) {
            UIItemSlot uiItemSlot = inventory.Slots[i];
            ItemSlot itemSlot = uiItemSlot.ItemSlot;

            stacks[i] = itemSlot.Stack;
        }
    }
}
