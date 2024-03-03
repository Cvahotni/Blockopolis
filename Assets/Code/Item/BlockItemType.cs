using UnityEngine;

[CreateAssetMenu(fileName = "New Block Item Type", menuName = "Voxel Engine/Block Item Type")]
public class BlockItemType : ItemType
{
    [Header("Block Item Information")]
    [SerializeField] private Sprite brokenSprite;
    [SerializeField] private Material brokenMaterial;

    public Sprite BrokenSprite {
        get { return brokenSprite; }
    }

    public Material BrokenMaterial {
        get { return brokenMaterial; }
    }
}
