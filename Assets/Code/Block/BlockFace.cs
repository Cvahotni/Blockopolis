using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

[CreateAssetMenu(fileName = "New Block Face", menuName = "Voxel Engine/Block Face")]
public class BlockFace : ScriptableObject
{
    [Header("Basic Information")]
    public BlockFaceDirection direction;
    public bool cullFace;
    public int id;

    [Header("Model Data")]
    public List<float3> faceVerts = new List<float3>();
    public List<int> faceTris = new List<int>();
    public List<float2> faceUVs = new List<float2>();
}
