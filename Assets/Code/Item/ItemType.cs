using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Item Type", menuName = "Voxel Engine/Item Type")]
public class ItemType : ScriptableObject
{
    [Header("Basic Information")]
    [SerializeField] private ushort id;

    [Header("Display Information")]
    [SerializeField] private Sprite sprite;
    [SerializeField] private Material droppedItemMaterial;

    [Header("Prefab Information")]
    [SerializeField] private GameObject heldItemPrefab;
    [SerializeField] private GameObject droppedItemPrefab;

    public ushort ID {
        get { return id; }
    }

    public Sprite Sprite {
        get { return sprite; }
    }

    public Material DroppedItemMaterial {
        get { return droppedItemMaterial; }
    }

    public GameObject HeldItemPrefab {
        get { return heldItemPrefab; }
    }

    public GameObject DroppedItemPrefab {
        get { return droppedItemPrefab; }
    }
}
