using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    private ItemStack stack;

    private UIItemSlot slot;
    private UIItemSlot hotbarSlot;

    private bool empty;
    public bool Empty { get { return empty; }}

    public ItemSlot(UIItemSlot UIItemSlot) {
        this.slot = UIItemSlot;
    }

    public ItemSlot(UIItemSlot slot, UIItemSlot hotbarSlot, ItemStack stack) {
        this.stack = stack;

        this.slot = slot;
        this.hotbarSlot = hotbarSlot;

        slot.Link(this);
        LinkHotbarSlot();
    }

    public void UpdateEmptyStatus() {
        CheckIfEmpty();
        UpdateSlot(true);
    }

    public void CheckIfEmpty() {
        empty = stack.Amount <= 0 || stack.ID == 0;
    }

    public void EmptySlot() {
        stack.ID = 0;
        stack.Amount = 0;

        UpdateSlot(false);
        UpdateEmptyStatus();
    }

    public ItemStack TakeAll() {
        ItemStack handOver = new ItemStack(stack.ID, stack.Amount);
        EmptySlot();

        return handOver;
    }

    public void InsertStack(ItemStack newStack) {
        stack = newStack;

        UpdateSlot(true);
        UpdateEmptyStatus();
    }

    public int Take(ushort amount) {
        if(stack.Amount <= 1) {
            stack.Amount = 0;
            UpdateSlot(false);

            UpdateEmptyStatus();
            return 0;
        }

        if(amount <= stack.Amount) {
            stack.Take(amount);
            UpdateSlot(false);

            if(stack.Amount <= 0) UpdateEmptyStatus();
            return stack.Amount;
        }

        return stack.Amount;
    }

    public ushort Give(ushort amount) {
        ushort totalAmount = (ushort) (stack.Amount + amount);

        if(totalAmount > InventoryProperties.maxStackSize) {
            stack.Amount = (ushort) InventoryProperties.maxStackSize;
            return (ushort) Mathf.Abs(InventoryProperties.maxStackSize - totalAmount);
        }

        stack.Amount += amount;
        return 0;
    }

    public void UpdateSlot(bool value) {
        slot.UpdateSlot(value);
        UpdateHotbarSlot(value);
    }

    private void UpdateHotbarSlot(bool value) {
        if(hotbarSlot == null) return;
        hotbarSlot.UpdateSlot(value);
    }

    private void LinkHotbarSlot() {
        if(hotbarSlot == null) return;
        hotbarSlot.Link(this);
    }

    public ItemStack Stack {
        get { return stack; }
        set { stack = value; }
    }

    public bool HasItem {
        get { return !empty; }
    }
}
