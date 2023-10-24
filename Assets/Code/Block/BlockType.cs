using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[System.Serializable]
public struct BlockType
{
    public ushort id;
    public bool solid;

    public float hardness;
    
    public float2 backTexture;
    public float2 frontTexture;
    public float2 upTexture;
    public float2 downTexture;
    public float2 leftTexture;
    public float2 rightTexture;
}
