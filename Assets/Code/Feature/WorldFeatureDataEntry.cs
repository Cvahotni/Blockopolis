using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public class WorldFeatureDataEntry
{
    [SerializeField] private int3 pos;
    [SerializeField] private BlockID block;

    public int3 Pos { get { return pos; }}
    public BlockID Block { get { return block; }}

    public WorldFeatureDataEntry(int3 pos, BlockID block) {
        this.pos = pos;
        this.block = block;
    }
}
