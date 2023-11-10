using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance { get; private set; }

    [SerializeField] private RectTransform hightlight;

    [SerializeField] private int hotbarLength = 9;

    private bool hotbarEnabled = false;

    public bool HotbarEnabled {
        get { return hotbarEnabled; }
        set { hotbarEnabled = value; }
    }

    private int slotIndex = 0;

    private InventoryEventSystem inventoryEventSystem;
    private Inventory inventory;
    private ItemRegistry itemRegistry;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        inventoryEventSystem = InventoryEventSystem.Instance;
        inventory = Inventory.Instance;
        itemRegistry = ItemRegistry.Instance;
    }

    private void Update() {
        if(!hotbarEnabled) return;

        UpdateSlotIndex();
        UpdateHighlightPosition();
        UpdatePlayerTargetBlock();
    }

    private void UpdateSlotIndex() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll == 0) return;

        ItemSlot previousSlot = inventory.Slots[slotIndex].ItemSlot;

        if(scroll > 0) slotIndex--;
        else slotIndex++;

        if(slotIndex > hotbarLength - 1) slotIndex = 0;
        if(slotIndex < 0) slotIndex = hotbarLength - 1;

        ItemSlot currentSlot = inventory.Slots[slotIndex].ItemSlot;
        if(!CheckIfSlotsAreSimilar(previousSlot, currentSlot)) UpdateHeldItem(scroll);
    }

    public void UpdateHeldItem(ItemStack stack) {
        UpdateHeldItem(1.0f / 20f);
    }

    public void UpdateHeldItem() {
        UpdateHeldItem(1.0f / 20f);
    }

    private bool CheckIfSlotsAreSimilar(ItemSlot firstSlot, ItemSlot secondSlot) {
        ItemStack firstStack = firstSlot.Stack;
        ItemStack secondStack = secondSlot.Stack;

        return firstStack.ID == secondStack.ID;
    }

    public void UpdateHeldItem(float switchTime) {
        if(inventory == null) {
            Debug.LogError("The Inventory script must be present in the scene in order to update the held item.");
            return;
        }

        ItemSlot currentSlot = inventory.Slots[slotIndex].ItemSlot;

        SwitchedItemStack switchedItemStack = new SwitchedItemStack(currentSlot.Stack, switchTime);
        inventoryEventSystem.InvokeModifyHeldSlot(switchedItemStack);
    }

    private void UpdateHighlightPosition() {
        if(inventory == null) return;
        hightlight.position = inventory.Slots[slotIndex].SlotIcon.transform.position;
    }

    private void UpdatePlayerTargetBlock() {
        if(itemRegistry == null) {
            Debug.LogError("The ItemRegistry script must be present in the scene in order to update the target block.");
            return;
        }

        if(inventory == null) {
            Debug.LogError("The Inventory script must be present in the scene in order to update the target block.");
            return;
        }

        ItemSlot currentSlot = inventory.Slots[slotIndex].ItemSlot;

        if(!currentSlot.HasItem) {
            inventoryEventSystem.InvokeTargetSlotUpdate(0);
            return;
        }

        ushort id = currentSlot.Stack.ID;

        if(!itemRegistry.IsItemForm(id, ItemForm.BlockItem)) return;
        inventoryEventSystem.InvokeTargetSlotUpdate(id);
    }

    public void SetStatus(bool status) {
        hotbarEnabled = status;
    }

    public void TakeFromCurrentSlot() {
        if(inventory == null) {
            Debug.LogError("The Inventory script must be present in the scene in order to take from the current slot.");
            return;
        }

        inventory.Slots[slotIndex].ItemSlot.Take(1);
        RefreshHeldSlot();
    }

    public void RefreshHeldSlot() {
        ItemSlot itemSlot = inventory.Slots[slotIndex].ItemSlot;
        inventoryEventSystem.InvokeModifyHeldSlot(new SwitchedItemStack(itemSlot.Stack, 0.0f));
    }
}
