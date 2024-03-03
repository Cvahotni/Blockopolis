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

        Load();

        DebugPopulate();
        UpdateInventoryScreen();
    }

    private void DebugPopulate() {
        if(!IsEmpty()) return;

        SetStack(0, new ItemStack(1005, 1));
        SetStack(1, new ItemStack(1010, 1));
        SetStack(2, new ItemStack(1015, 1));
        SetStack(3, new ItemStack(1, 999));
        SetStack(4, new ItemStack(2, 999));
        SetStack(5, new ItemStack(3, 999));
        SetStack(6, new ItemStack(4, 999));
        SetStack(7, new ItemStack(5, 999));
        SetStack(8, new ItemStack(6, 999));
        SetStack(9, new ItemStack(7, 999));
        SetStack(10, new ItemStack(8, 999));
        SetStack(11, new ItemStack(9, 999));
        SetStack(12, new ItemStack(10, 999));
        SetStack(13, new ItemStack(11, 999));
        SetStack(14, new ItemStack(12, 999));
        SetStack(15, new ItemStack(13, 999));
        SetStack(16, new ItemStack(14, 999));
        SetStack(17, new ItemStack(15, 999));
        SetStack(18, new ItemStack(16, 999));
        SetStack(19, new ItemStack(17, 999));
        SetStack(20, new ItemStack(18, 999));
        SetStack(21, new ItemStack(19, 999));
        SetStack(22, new ItemStack(20, 999));
        SetStack(23, new ItemStack(21, 999));
        SetStack(24, new ItemStack(22, 999));
        SetStack(25, new ItemStack(23, 999));
        SetStack(26, new ItemStack(24, 999));
        SetStack(27, new ItemStack(25, 999));
        SetStack(28, new ItemStack(26, 999));
        SetStack(29, new ItemStack(27, 999));
        SetStack(30, new ItemStack(28, 999));
        SetStack(31, new ItemStack(29, 999));
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

    private bool IsEmpty() {
        bool found = false;

        foreach(UIItemSlot slot in slots) {
            ItemSlot itemSlot = slot.ItemSlot;
            ItemStack stack = itemSlot.Stack;

            if(!found && stack.ID != 0) found = true; 
        }

        return !found;
    }

    private void Load() {
        if(!WorldHandler.IsCurrentWorldValid()) {
            Debug.Log("Failed to load inventory: Current world is not valid.");
            return;
        }

        WorldInventorySaveLoad.LoadWorldInventory(WorldHandler.CurrentWorld, this);
    }
}
