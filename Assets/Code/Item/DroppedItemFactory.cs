using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedItemFactory : MonoBehaviour
{
    public static DroppedItemFactory Instance { get; private set; }

    [SerializeField] private GameObject droppedBlockPrefab;
    [SerializeField] private GameObject droppedItemPrefab;

    [SerializeField] private float randomForceAmount = 1.0f;

    private ItemRegistry itemRegistry;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        itemRegistry = ItemRegistry.Instance;
    }

    public void DropItem(Vector3 position, ushort id, ushort amount) {
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
