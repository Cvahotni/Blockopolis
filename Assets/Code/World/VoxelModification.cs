using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoxelModification
{
    private int x;
    private int y;
    private int z;

    private ushort id;

    public int X { get { return x; }}
    public int Y { get { return y; }}
    public int Z { get { return z; }}

    public ushort ID { get { return id; }}

    public VoxelModification(int x, int y, int z, ushort id) {
        this.x = x;
        this.y = y;
        this.z = z;

        this.id = id;
    }
}
