using UnityEngine;
using System;

[Serializable]
public struct ItemStack
{
    [SerializeField] private ushort id;
    [SerializeField] private ushort amount;

    public ushort ID {
        get { return id; }
        set { id = value; }
    }

    public ushort Amount {
        get { return amount; }
        set { amount = value; }
    }

    public ItemStack(ushort id, ushort amount) {
        this.id = id;
        this.amount = amount;
    }

    public bool Add(ushort count) {
        int totalAmount = amount + count;
        if(totalAmount > InventoryProperties.maxStackSize) return false;

        amount += count;
        return true;
    }

    public void Take(ushort count) {
        amount -= count;
    }

    public bool Equals(ItemStack other) {
        return other.ID == id && other.Amount == amount;
    }
}
