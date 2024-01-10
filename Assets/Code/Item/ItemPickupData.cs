using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public struct ItemPickupData
{
    public ItemStack itemStack;
    public Vector3 position;

    public ItemPickupData(ItemStack itemStack, Vector3 position) {
        this.itemStack = itemStack;
        this.position = position;
    }
}
