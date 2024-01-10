using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SwitchedItemStack
{
    public ItemStack itemStack;
    public float switchTime;
    public int slotIndex;

    public SwitchedItemStack(ItemStack itemStack, float switchTime, int slotIndex) {
        this.itemStack = itemStack;
        this.switchTime = switchTime;
        this.slotIndex = slotIndex;
    }
}
