using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BlockBreakData 
{
    public float x;
    public float y;
    public float z;

    public ushort block;
    public ushort amount;

    public BlockBreakData(float x, float y, float z, ushort block, ushort amount) {
        this.x = x;
        this.y = y;
        this.z = z;

        this.block = block;
        this.amount = amount;
    }
}
