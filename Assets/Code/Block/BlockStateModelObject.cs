using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block State Model", menuName = "Voxel Engine/Block State Model")]
public class BlockStateModelObject : ScriptableObject
{
    [Header("Basic Information")]
    public byte id;

    [Header("Model Data")]
    public List<BlockFace> faces = new List<BlockFace>();
}
