using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot
{
    private ItemStack stack;
    private UiItemSlot uiItemSlot;

    private bool empty;

    public ItemSlot(UiItemSlot uiItemSlot) {
        this.uiItemSlot = uiItemSlot;
    }

    public ItemSlot(UiItemSlot uiItemSlot, ItemStack stack) {
        this.stack = stack;
        this.uiItemSlot = uiItemSlot;

        uiItemSlot.Link(this);
        uiItemSlot.UpdateSlot(true);
    }

    public void UpdateEmptyStatus() {
        CheckIfEmpty();
        uiItemSlot.UpdateSlot(empty);
    }

    public void CheckIfEmpty() {
        empty = stack.Amount <= 0 || stack.ID == 0;
    }

    public int Take(int amount) {
        if(stack.Amount <= 1) {
            stack.Amount = 0;
            uiItemSlot.UpdateSlot(false);
            UpdateEmptyStatus();

            return 0;
        }

        if(amount <= stack.Amount) {
            stack.Take(amount);
            uiItemSlot.UpdateSlot(false);

            if(stack.Amount <= 0) UpdateEmptyStatus();
            return stack.Amount;
        }

        return stack.Amount;
    }

    public int Give(int amount, int maxSize) {
        int totalAmount = stack.Amount + amount;

        if(totalAmount > maxSize) {
            stack.Amount = maxSize;
            return Mathf.Abs(maxSize - totalAmount);
        }

        stack.Amount += amount;
        return 0;
    }

    public ItemStack Stack {
        get { return stack; }
        set { stack = value; }
    }

    public bool HasItem {
        get { return !empty; }
    }
}
