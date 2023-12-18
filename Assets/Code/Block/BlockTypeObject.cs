using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block Type", menuName = "Voxel Engine/Block Type")]
public class BlockTypeObject : ScriptableObject
{
    public ushort id;
    public bool solid;

    public BlockMaterial material;
    public float hardness;
    
    public Vector2 backTexture;
    public Vector2 frontTexture;
    public Vector2 upTexture;
    public Vector2 downTexture;
    public Vector2 leftTexture;
    public Vector2 rightTexture;
}
