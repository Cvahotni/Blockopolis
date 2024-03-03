using UnityEngine;

public class DroppedItemFactory : MonoBehaviour
{
    public static DroppedItemFactory Instance { get; private set; }

    [SerializeField] private GameObject droppedBlockPrefab;
    [SerializeField] private GameObject droppedItemPrefab;

    [SerializeField] private float randomForceAmount = 0.1f;

    private ItemRegistry itemRegistry;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        itemRegistry = ItemRegistry.Instance;
    }

    public void DropItemFromBlock(object sender, BlockModifyData data) {
        DropItem(new Vector3(data.x, data.y, data.z), data.block.id, data.amount);
    }

    public void DropItem(Vector3 position, ushort id, ushort amount) {
        if(itemRegistry == null) {
            Debug.LogError("The ItemRegistry script must be present in the scene in order to drop an item.");
            return;
        }

        GameObject droppedItemObject = GetDroppedPrefab(position, id);
        
        DroppedItem droppedItem = droppedItemObject.GetComponent<DroppedItem>();
        Rigidbody rigidbody = droppedItemObject.GetComponent<Rigidbody>();

        float randomForceX = Random.Range(0, randomForceAmount);
        float randomForceZ = Random.Range(0, randomForceAmount);

        randomForceX = randomForceX - (randomForceAmount / 2.0f);
        randomForceZ = randomForceZ - (randomForceAmount / 2.0f);

        droppedItem.SetItem(itemRegistry, id, amount);
        rigidbody.AddForce(new Vector3(randomForceX, 0.0f, randomForceZ));
    }

    private GameObject GetDroppedPrefab(Vector3 position, ushort id) {
        GameObject prefab = itemRegistry.GetItemDroppedPrefab(id);
        return Instantiate(prefab, position, Quaternion.identity);
    }
}
