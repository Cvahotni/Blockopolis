using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemRegistry : MonoBehaviour
{
    public static ItemRegistry Instance { get; private set; }

    private List<ItemType> itemTypes = new List<ItemType>();

    [SerializeField]
    private Sprite emptyItemSprite;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        LoadItemTypesFromFolder();
    }

    private void LoadItemTypesFromFolder() {
        System.Object[] objects = Resources.LoadAll("Item Types");
        foreach(System.Object currentObject in objects) itemTypes.Add((ItemType) currentObject);
    }

    public Material GetMaterialForID(ushort id) {
        foreach(ItemType itemType in itemTypes) if(itemType.ID == id) return itemType.DroppedItemMaterial;
        throw new NullReferenceException("Material not found for item ID: " + id);
    }

    public Material GetMaterialForIDWithCount(ushort id, ushort count) {
        if(count == 0) return GetMaterialForID(0);

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

    public float GetItemMineMultiplier(ushort id, ushort blockId) {
        BlockMaterial blockMaterial = BlockRegistry.BlockTypeDictionary[blockId].material;
        BlockMaterial mineMaterial = BlockMaterial.Unknown;

        float mineSpeed = 1.0f;
        
        foreach(ItemType itemType in itemTypes) {
            if(itemType.ID == id) {
                mineMaterial = itemType.MineableMaterial;
                mineSpeed = itemType.SpeedMultiplier;
            }
        }

        if(blockMaterial == mineMaterial) return mineSpeed;
        return 1.0f;
    }

    public GameObject GetItemDroppedPrefab(ushort id) {
        foreach(ItemType itemType in itemTypes) {
            if(itemType.ID == id) return itemType.DroppedItemPrefab;
        }

        throw new NullReferenceException("Dropped item prefab not found for item ID: " + id);
    }

    public GameObject GetItemHeldPrefabWithCount(ushort id, ushort count) {
        if(count == 0) return GetItemHeldPrefabWithCount(0, 1);

        foreach(ItemType itemType in itemTypes) {
            if(itemType.ID == id) return itemType.HeldItemPrefab;
        }

        throw new NullReferenceException("Held item prefab not found for item ID: " + id);
    }
}
