using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemSlotIndex
{
    public int slotIndex;
    public int amount;

    public ItemSlotIndex(int slotIndex, int amount) {
        this.slotIndex = slotIndex;
        this.amount = amount;
    }
}
