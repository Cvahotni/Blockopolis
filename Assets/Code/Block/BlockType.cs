using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block Type", menuName = "Voxel Engine/Block Type")]
public class BlockType : ScriptableObject
{
    [Header("Basic Information")]
    public byte id;
    public bool replaceable;
    public bool isLiquid;

    [Header("Material Information")]
    public BlockMaterial material;
    public float hardness;
}
