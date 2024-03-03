using UnityEngine;

public class PlayerItemPickup : MonoBehaviour
{
    [SerializeField] private string droppedItemTagName;

    private InventoryEventSystem inventoryEventSystem;

    private void Start() {
        inventoryEventSystem = InventoryEventSystem.Instance;
    }

    private void OnTriggerEnter(Collider other) {
        GameObject currentObject = other.gameObject;
        
        if(currentObject.tag != droppedItemTagName) return;
        DroppedItem droppedItem = currentObject.GetComponent<DroppedItem>();

        if(droppedItem.Destroyed) return;
        droppedItem.Destroyed = true;

        inventoryEventSystem.InvokeItemPickup(new ItemPickupData(droppedItem.ItemStack, currentObject.transform.position));
        Destroy(currentObject, 0.1f);
    }
}
