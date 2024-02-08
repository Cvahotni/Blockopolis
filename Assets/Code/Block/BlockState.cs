using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[System.Serializable]
public struct BlockState
{
    public byte id;
    public byte variant;

    public bool solid;
    public bool transparent;

    public BlockMaterial material;
    public float hardness;

    public byte model;

    public float2 backTexture;
    public float2 frontTexture;
    public float2 upTexture;
    public float2 downTexture;
    public float2 leftTexture;
    public float2 rightTexture;
}
