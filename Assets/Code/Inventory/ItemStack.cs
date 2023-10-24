using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemStack
{
    private ushort id;
    private int amount;

    public ushort ID {
        get { return id; }
        set { ID = value; }
    }

    public int Amount {
        get { return amount; }
        set { amount = value; }
    }

    public ItemStack(ushort id, int amount) {
        this.id = id;
        this.amount = amount;
    }

    public bool Add(int count, int maxStackSize) {
        int totalAmount = amount + count;
        if(totalAmount > maxStackSize) return false;

        amount += count;
        return true;
    }

    public void Take(int count) {
        amount -= count;
    }
}
