using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Item Type", menuName = "Voxel Engine/Tool Item Type")]
public class ToolItemType : ItemType
{
    [Header("Tool Information")]
    [SerializeField] private List<BlockMaterial> mineableMaterials = new List<BlockMaterial>();
    [SerializeField] private float speedMultiplier;

    public List<BlockMaterial> MineableMaterials {
        get { return mineableMaterials; }
    }

    public float SpeedMultiplier {
        get { return speedMultiplier; }
    }
}
