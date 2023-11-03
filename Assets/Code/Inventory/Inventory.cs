using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [SerializeField] private UiItemSlot[] slots;
    [SerializeField] private int maxStackSize = 5;

    public UiItemSlot[] Slots {
        get { return slots; }
    }

    public int MaxStackSize {
        get { return maxStackSize; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        InitItemSlots();
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

    public void AddStack(ItemStack stack) {
        int leftOverStackSize = stack.Amount;

        for(int i = 0; i < slots.Length; i++) {
            UiItemSlot uiItemSlot = slots[i];
            ItemSlot itemSlot = uiItemSlot.ItemSlot;
            ItemStack currentStack = itemSlot.Stack;

            if(currentStack.ID != stack.ID && currentStack.Amount > 0) continue;
            if(currentStack.Amount <= 0) SetStack(i, new ItemStack(stack.ID, 0));

            leftOverStackSize = itemSlot.Give(leftOverStackSize, maxStackSize);

            if(leftOverStackSize != 0) {
                slots[i].UpdateSlot(true);
                continue;
            }

            slots[i].UpdateSlot(true);
            break;
        }
    }

    public void SetStack(int index, ItemStack stack) {
        slots[index].ItemSlot.Stack = stack;
        slots[index].UpdateSlot(true);
    }
}
