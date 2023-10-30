using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UiItemSlot : MonoBehaviour
{
    [SerializeField] private Image slotIcon;

    [SerializeField] private TMP_Text slotAmount;
    [SerializeField] private TMP_Text slotAmountShadow;

    private ItemSlot itemSlot;

    public bool HasItem {
        get { return itemSlot == null ? false : true; }
    }

    public Image SlotIcon {
        get { return slotIcon; }
    }

    public ItemSlot ItemSlot {
        get { return itemSlot; }
    }

    public void Link(ItemSlot itemSlot) {
        this.itemSlot = itemSlot;
    }

    public void UpdateSlot(bool check) {
        if(check) itemSlot.CheckIfEmpty();
        ItemStack stack = itemSlot.Stack;

        if(itemSlot.HasItem) {
            slotIcon.sprite = ItemRegistry.Instance.GetSpriteForID(stack.ID);
           
            slotAmount.text = stack.Amount.ToString();
            slotAmountShadow.text = stack.Amount.ToString();

            slotIcon.enabled = true;

            slotAmount.enabled = true;
            slotAmountShadow.enabled = true;
        }

        else {
            slotIcon.sprite = null;

            slotAmount.text = "";
            slotAmountShadow.text = "";
        
            slotIcon.enabled = false;

            slotAmount.enabled = false;
            slotAmountShadow.enabled = false;
        }

        if(stack.Amount == 1) {
            slotAmount.text = "";
            slotAmountShadow.text = "";
        }
    }
}