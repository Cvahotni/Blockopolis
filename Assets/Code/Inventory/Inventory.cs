using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    private InventoryEventSystem inventoryEventSystem;

    [SerializeField] private UiItemSlot[] slots;

    public UiItemSlot[] Slots {
        get { return slots; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        InitItemSlots();
        inventoryEventSystem = InventoryEventSystem.Instance;

        WorldHandler.LoadWorldInventory();
    }

    private void InitItemSlots() {
        ushort index = 1;

        for(int i = 0; i < slots.Length; i++) {
            ItemStack itemStack = new ItemStack(0, 0);
            ItemSlot itemSlot = new ItemSlot(slots[index - 1], itemStack);

            index++;
        }
    }

    public void AddStack(object sender, ItemStack stack) {
        ushort leftOverStackSize = stack.Amount;

        for(int i = 0; i < slots.Length; i++) {
            UiItemSlot uiItemSlot = slots[i];
            ItemSlot itemSlot = uiItemSlot.ItemSlot;
            ItemStack currentStack = itemSlot.Stack;

            int stackAmount = currentStack.Amount;

            if(currentStack.ID != stack.ID && currentStack.Amount > 0) continue;
            if(currentStack.Amount <= 0) SetStack(i, new ItemStack(stack.ID, 0));

            leftOverStackSize = itemSlot.Give(leftOverStackSize, InventoryProperties.maxStackSize);

            if(leftOverStackSize != 0) {
                slots[i].UpdateSlot(true);
                continue;
            }

            ItemSlotIndex itemSlotIndex = new ItemSlotIndex(i, stackAmount);
            inventoryEventSystem.InvokeHotbarUpdate(itemSlotIndex);

            slots[i].UpdateSlot(true);
            break;
        }
    }

    public void SetStack(int index, ItemStack stack) {
        slots[index].ItemSlot.Stack = stack;
        slots[index].UpdateSlot(true);
    }
}
