using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemType
{
    [SerializeField]
    private ushort id;

    [SerializeField]
    private Sprite sprite;

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

    public Material DroppedItemMaterial {
        get { return droppedItemMaterial; }
    }

    public ItemForm ItemForm {
        get { return itemForm; }
    }

    public GameObject DroppedItemPrefab {
        get { return droppedItemPrefab; }
    }
}
