using UnityEngine;

[CreateAssetMenu(fileName = "New Block State", menuName = "Voxel Engine/Block State")]
public class BlockStateObject : ScriptableObject
{
    [Header("Basic Information")]
    public byte id;
    public byte variant;

    public bool solid;
    public bool transparent;
    public bool cutout;

    [Header("Material Information")]
    public BlockMaterial material;
    public float hardness;

    [Header("Lighting Information")]
    public byte transparency;

    [Header("Model Information")]
    public BlockStateModelObject model;

    [Header("Face Textures")]
    public Vector2 backTexture;
    public Vector2 frontTexture;
    public Vector2 upTexture;
    public Vector2 downTexture;
    public Vector2 leftTexture;
    public Vector2 rightTexture;
}
