using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;
    [SerializeField] private ItemForm itemForm;

    private Hotbar hotbar;

    private ItemStack itemStack;
    private DroppedItemTextureHandler droppedItemTextureHandler;

    private bool destroyed = false;

    private string tagName = "DroppedItem";
    private string layerName = "DroppedItem";

    public ItemStack ItemStack {
        get { return itemStack; }
    }

    public bool Destroyed {
        get { return destroyed; }
        set { destroyed = value; }
    }

    private void Start() {
        hotbar = Hotbar.Instance;
    }

    private void SetReferences() {
        gameObject.tag = tagName;
        gameObject.layer = LayerMask.NameToLayer(layerName);

        droppedItemTextureHandler = gameObject.GetComponent<DroppedItemTextureHandler>();
    }

    public void SetItem(ItemRegistry itemRegistry, ushort id, int amount) {
        itemStack = new ItemStack(id, amount);

        SetReferences();
        Material material = itemRegistry.GetMaterialForID(id);

        droppedItemTextureHandler.SetMaterials(material);
    }

    public bool Add(int amount, int maxStackSize) {
        return itemStack.Add(amount, maxStackSize);
    }

    private void OnTriggerStay(Collider other) {
        if(destroyed) return;

        GameObject otherGameObject = other.gameObject;
        if(otherGameObject.tag != tagName) return;

        DroppedItem droppedItem = otherGameObject.GetComponent<DroppedItem>();

        if(droppedItem.ItemStack.ID != itemStack.ID) return;
        if(droppedItem.Destroyed) return;

        bool addedToOtherItemStack = droppedItem.Add(itemStack.Amount, hotbar.MaxStackSize);
        if(!addedToOtherItemStack) return;
    
        destroyed = true;
        Destroy(gameObject, 0.1f);
    }
}
