using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance { get; private set; }

    [SerializeField] private RectTransform hightlight;

    [SerializeField] private int hotbarLength = 9;

    private bool hotbarEnabled = false;

    private Inventory inventory;

    public bool HotbarEnabled {
        get { return hotbarEnabled; }
        set { hotbarEnabled = value; }
    }

    private int slotIndex = 0;

    private PlayerBuild playerBuild;
    private ItemRegistry itemRegistry;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        inventory = Inventory.Instance;
        itemRegistry = ItemRegistry.Instance;
    }

    private void Update() {
        if(!hotbarEnabled) return;
        
        if(playerBuild == null && hotbarEnabled) {
            playerBuild = PlayerBuild.Instance;
            return;
        }

        UpdateSlotIndex();
        UpdateHighlightPosition();
        UpdatePlayerTargetBlock();
    }

    private void UpdateSlotIndex() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll == 0) return;

        if(scroll > 0) slotIndex--;
        else slotIndex++;

        if(slotIndex > hotbarLength - 1) slotIndex = 0;
        if(slotIndex < 0) slotIndex = hotbarLength - 1;
    }

    private void UpdateHighlightPosition() {
        hightlight.position = inventory.Slots[slotIndex].SlotIcon.transform.position;
    }

    private void UpdatePlayerTargetBlock() {
        ItemSlot currentSlot = inventory.Slots[slotIndex].ItemSlot;

        if(!currentSlot.HasItem) {
            playerBuild.TargetBlock = 0;
            return;
        }

        ushort id = currentSlot.Stack.ID;

        if(!itemRegistry.IsItemForm(id, ItemForm.BlockItem)) return;
        playerBuild.TargetBlock = id;
    }

    public void TakeFromCurrentSlot() {
        inventory.Slots[slotIndex].ItemSlot.Take(1);
    }
}
