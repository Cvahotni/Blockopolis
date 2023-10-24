using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EncodedVoxelMapEntry
{
    public ushort id;
    public ushort count;
    
    public EncodedVoxelMapEntry(ushort id, ushort count) {
        this.id = id;
        this.count = count;
    }
}
