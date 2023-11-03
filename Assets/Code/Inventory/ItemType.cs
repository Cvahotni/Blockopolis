using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Type", menuName = "Voxel Engine/Item Type")]
public class ItemType : ScriptableObject
{
    [SerializeField]
    private ushort id;

    [SerializeField]
    private Sprite sprite;

    [SerializeField]
    private Sprite brokenSprite;

    [SerializeField]
    private Material brokenMaterial;

    [SerializeField]
    private Material droppedItemMaterial;

    [SerializeField]
    private ItemForm itemForm;

    [SerializeField]
    private GameObject droppedItemPrefab;

    public ushort ID {
        get { return id; }
    }

    public Sprite Sprite {
        get { return sprite; }
    }

    public Sprite BrokenSprite {
        get { return brokenSprite; }
    }

    public Material DroppedItemMaterial {
        get { return droppedItemMaterial; }
    }

    public Material BrokenMaterial {
        get { return brokenMaterial; }
    }

    public ItemForm ItemForm {
        get { return itemForm; }
    }

    public GameObject DroppedItemPrefab {
        get { return droppedItemPrefab; }
    }
}
