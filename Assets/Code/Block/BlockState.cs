using Unity.Mathematics;

[System.Serializable]
public struct BlockState
{
    public byte id;
    public byte variant;

    public bool solid;
    public bool transparent;
    public bool cutout;

    public byte model;
    public byte transparency;

    public float2 backTexture;
    public float2 frontTexture;
    public float2 upTexture;
    public float2 downTexture;
    public float2 leftTexture;
    public float2 rightTexture;
}
