using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "New Block Type", menuName = "Voxel Engine/Block Type")]
public class BlockTypeObject : ScriptableObject
{
    [Header("Basic Information")]
    public ushort id;
    public bool solid;

    [Header("Material Information")]
    public BlockMaterial material;
    public float hardness;
    
    [Header("Face Textures")]
    public Vector2 backTexture;
    public Vector2 frontTexture;
    public Vector2 upTexture;
    public Vector2 downTexture;
    public Vector2 leftTexture;
    public Vector2 rightTexture;

    [Header("Model Data")]
    public List<BlockFace> faces = new List<BlockFace>();
}
