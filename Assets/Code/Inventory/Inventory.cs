using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private InventoryEventSystem inventoryEventSystem;
    private bool inUI = false;

    [SerializeField] private UIItemSlot[] slots;
    [SerializeField] private UIItemSlot[] hotbarSlots;

    [SerializeField] private GameObject inventoryScreen;

    public bool InUI { get { return inUI; }}

    public UIItemSlot[] Slots {
        get { return slots; }
    }

    public UIItemSlot[] HotbarSlots {
        get { return hotbarSlots; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        InitItemSlots();
        inventoryEventSystem = InventoryEventSystem.Instance;

        WorldHandler.LoadWorldInventory();
        UpdateInventoryScreen();

        SetStack(0, new ItemStack(1005, 1));
        SetStack(1, new ItemStack(1010, 1));
        SetStack(2, new ItemStack(1015, 1));
    }

    private void InitItemSlots() {
        ItemStack itemStack = new ItemStack(0, 0);
        UIItemSlot hotbarSlot = null;

        for(int i = 0; i < slots.Length; i++) {
            if(i < hotbarSlots.Length) hotbarSlot = hotbarSlots[i];
            else hotbarSlot = null;

            ItemSlot itemSlot = new ItemSlot(slots[i], hotbarSlot, itemStack);
        }

    }

    public void AddStack(object sender, ItemPickupData data) {
        AddStack(data.itemStack);
    }

    private void AddStack(ItemStack stack) {
        ushort leftOverStackSize = stack.Amount;

        for(int i = 0; i < slots.Length; i++) {
            UIItemSlot UIItemSlot = slots[i];
            ItemSlot itemSlot = UIItemSlot.ItemSlot;
            ItemStack currentStack = itemSlot.Stack;

            int stackAmount = currentStack.Amount;

            if(currentStack.ID != stack.ID && currentStack.Amount > 0) continue;
            if(currentStack.Amount <= 0) SetStack(i, new ItemStack(stack.ID, 0));

            leftOverStackSize = itemSlot.Give(leftOverStackSize);

            if(leftOverStackSize != 0) {
                slots[i].UpdateItemSlot(true);
                continue;
            }

            ItemSlotIndex itemSlotIndex = new ItemSlotIndex(i, stackAmount);
            inventoryEventSystem.InvokeHotbarUpdate(itemSlotIndex);

            slots[i].UpdateItemSlot(true);
            break;
        }
    }

    public void SetStack(int index, ItemStack stack) {
        if(index >= slots.Length) return;

        slots[index].ItemSlot.Stack = stack;
        slots[index].ItemSlot.UpdateSlot(true);
    }

    public void ActivateInUI(object sender, EventArgs e) {
        inUI = true;
        UpdateInventoryScreen();
    }

    public void DeactivateInUI(object sender, EventArgs e) {
        inUI = false;
        UpdateInventoryScreen();
    }

    public void ToggleInUI(object sender, EventArgs e) {
        inUI = !InUI;
        UpdateInventoryScreen();
    }

    private void UpdateInventoryScreen() {
        inventoryScreen.SetActive(inUI);
    }
}
