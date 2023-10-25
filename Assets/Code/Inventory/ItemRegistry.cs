using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemRegistry : MonoBehaviour
{
    public static ItemRegistry Instance { get; private set; }

    [SerializeField]
    private List<ItemType> itemTypes = new List<ItemType>();

    [SerializeField]
    private Sprite emptyItemSprite;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public Material GetMaterialForID(ushort id) {
        foreach(ItemType itemType in itemTypes) if(itemType.ID == id) return itemType.DroppedItemMaterial;
        throw new NullReferenceException("Material not found for item ID: " + id);
    }

    public Material GetBreakMaterialForID(ushort id) {
        foreach(ItemType itemType in itemTypes) if(itemType.ID == id) return itemType.BrokenMaterial;
        throw new NullReferenceException("Break material not found for item ID: " + id);
    }
    
    public Sprite GetSpriteForID(ushort id) {
        foreach(ItemType itemType in itemTypes) if(itemType.ID == id) return itemType.Sprite;
        return emptyItemSprite;
    }

    public Sprite GetBreakSpriteForID(ushort id) {
        foreach(ItemType itemType in itemTypes) if(itemType.ID == id) return itemType.BrokenSprite;
        return emptyItemSprite;
    }

    public bool IsItemForm(ushort id, ItemForm form) {
        foreach(ItemType itemType in itemTypes) {
            if(itemType.ID == id && itemType.ItemForm == form) return true;
        }
        
        return false;
    }

    public GameObject GetItemDroppedPrefab(ushort id) {
        foreach(ItemType itemType in itemTypes) {
            if(itemType.ID == id) return itemType.DroppedItemPrefab;
        }

        throw new NullReferenceException("Dropped item prefab not found for item ID: " + id);
    }
}
