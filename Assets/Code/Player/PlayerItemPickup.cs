using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    [SerializeField] private string droppedItemTagName;

    private Inventory inventory;
    private Hotbar hotbar;

    private void Start() {
        inventory = Inventory.Instance;
        hotbar = Hotbar.Instance;
    }

    private void OnTriggerEnter(Collider other) {
        GameObject currentObject = other.gameObject;
        
        if(currentObject.tag != droppedItemTagName) return;
        DroppedItem droppedItem = currentObject.GetComponent<DroppedItem>();

        if(droppedItem.Destroyed) return;
        droppedItem.Destroyed = true;

        inventory.AddStack(droppedItem.ItemStack);
        hotbar.RefreshHeldSlot();

        Destroy(currentObject, 0.1f);
    }
}
