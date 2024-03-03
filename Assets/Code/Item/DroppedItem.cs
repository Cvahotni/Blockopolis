using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [SerializeField] private Transform itemTransform;

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

    private void SetReferences() {
        gameObject.tag = tagName;
        gameObject.layer = LayerMask.NameToLayer(layerName);

        droppedItemTextureHandler = gameObject.GetComponent<DroppedItemTextureHandler>();
    }

    public void SetItem(ItemRegistry itemRegistry, ushort id, ushort amount) {
        SetReferences();

        if(droppedItemTextureHandler == null) {
            Debug.LogError("The Dropped Item prefab is missing a DroppedItemTextureHandler!");
            return;
        }

        itemStack = new ItemStack(id, amount);
        Material material = itemRegistry.GetMaterialForID(id);

        droppedItemTextureHandler.SetMaterials(material);
    }

    public bool Add(ushort amount) {
        return itemStack.Add(amount);
    }

    private void OnTriggerStay(Collider other) {
        if(destroyed) return;

        GameObject otherGameObject = other.gameObject;
        if(otherGameObject.tag != tagName) return;

        DroppedItem droppedItem = otherGameObject.GetComponent<DroppedItem>();

        if(droppedItem == null) {
            Debug.LogError("The other GameObject is missing a DroppedItem!");
            return;
        }

        if(droppedItem.ItemStack.ID != itemStack.ID) return;
        if(droppedItem.Destroyed) return;

        bool addedToOtherItemStack = droppedItem.Add(itemStack.Amount);
        if(!addedToOtherItemStack) return;
    
        destroyed = true;
        Destroy(gameObject, 0.1f);
    }
}
