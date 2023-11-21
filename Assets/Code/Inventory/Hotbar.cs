using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance { get; private set; }

    [SerializeField] private RectTransform hightlight;

    [SerializeField] private int hotbarLength = 9;

    private bool hotbarEnabled = true;
    private int slotIndex = 0;

    private InventoryEventSystem inventoryEventSystem;
    private Inventory inventory;
    private ItemRegistry itemRegistry;

    private ItemSlot CurrentSlot {
        get { return GetSlot(slotIndex); }
    }

    private Image CurrentSlotIcon {
        get { return GetSlotIcon(slotIndex); }
    }

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

        ItemSlot previousSlot = GetSlot(slotIndex);

        if(scroll > 0) slotIndex--;
        else slotIndex++;

        if(slotIndex > hotbarLength - 1) slotIndex = 0;
        if(slotIndex < 0) slotIndex = hotbarLength - 1;

        ItemSlot currentSlot = GetSlot(slotIndex);
        if(!CheckIfSlotsAreSimilar(previousSlot, currentSlot)) UpdateHeldItem(scroll);
    }

    public void UpdateHeldItem(ItemStack stack) {
        UpdateHeldItem(1.0f / 20f, new ItemSlotIndex(slotIndex, stack.Amount));
    }

    public void UpdateHeldItem() {
        UpdateHeldItem(1.0f / 20f, new ItemSlotIndex(slotIndex, 0));
    }

    public void UpdateHeldItem(float scroll) {
        UpdateHeldItem(scroll, new ItemSlotIndex(slotIndex, 0));
    }

    public void UpdateHeldItem(ItemSlotIndex data) {
        UpdateHeldItem(1.0f / 20f, data);
    }

    private bool CheckIfSlotsAreSimilar(ItemSlot firstSlot, ItemSlot secondSlot) {
        ItemStack firstStack = firstSlot.Stack;
        ItemStack secondStack = secondSlot.Stack;

        return CheckIfStacksAreSimilar(firstStack, secondStack);
    }

    private bool CheckIfStacksAreSimilar(ItemStack firstStack, ItemStack secondStack) {
        return firstStack.ID == secondStack.ID || (firstStack.Amount == 0 && secondStack.Amount == 0);
    }

    public void UpdateHeldItem(float switchTime, ItemSlotIndex data) {
        if(inventory == null) {
            Debug.LogError("The Inventory script must be present in the scene in order to update the held item.");
            return;
        }

        ItemSlot currentSlot = GetCurrentSlot();
        
        if(data.slotIndex != slotIndex) return;
        if(data.amount != 0) return;

        SwitchedItemStack switchedItemStack = new SwitchedItemStack(currentSlot.Stack, switchTime, slotIndex);
        inventoryEventSystem.InvokeModifyHeldSlot(switchedItemStack);
    }

    private void UpdateHighlightPosition() {
        if(inventory == null) return;
        hightlight.position = GetCurrentSlotIcon().transform.position;
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

        ItemSlot currentSlot = GetCurrentSlot();

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

    public void Enable() {
        SetStatus(true);
    }

    public void Disable() {
        SetStatus(false);
    }

    public void TakeFromCurrentSlot() {
        if(inventory == null) {
            Debug.LogError("The Inventory script must be present in the scene in order to take from the current slot.");
            return;
        }

        GetCurrentSlot().Take(1);
        RefreshHeldSlot();
    }

    public void RefreshHeldSlot() {
        ItemSlot itemSlot = GetCurrentSlot();
        inventoryEventSystem.InvokeModifyHeldSlot(new SwitchedItemStack(itemSlot.Stack, 0.0f, slotIndex));
    }

    private ItemSlot GetCurrentSlot() {
        return GetSlot(slotIndex);
    }

    private Image GetCurrentSlotIcon() {
        return GetSlotIcon(slotIndex);
    }

    private ItemSlot GetSlot(int index) {
        return inventory.Slots[index].ItemSlot;
    } 

    private Image GetSlotIcon(int index) {
        return inventory.Slots[index].SlotIcon;
    }
}
