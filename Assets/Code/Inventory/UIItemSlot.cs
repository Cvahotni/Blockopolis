using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIItemSlot : MonoBehaviour
{
    [SerializeField] private Image slotIcon;
    [SerializeField] private Image highlight;
    [SerializeField] private TMP_Text slotAmount;

    private ItemSlot itemSlot;

    private Color fullColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private Color emptyColor = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color highlightColor = new Color(1.0f, 1.0f, 1.0f, 0.2f);

    private bool highlighted = false;

    public bool HasItem {
        get { return !itemSlot.Empty; }
    }

    public Image SlotIcon {
        get { return slotIcon; }
    }

    public ItemSlot ItemSlot {
        get { return itemSlot; }
        set { itemSlot = value; }
    }

    public bool Highlighted {
        get { return highlighted; }
        set { highlighted = value; }
    }

    private void FixedUpdate() {
        highlight.color = highlighted ? highlightColor : emptyColor;
        highlighted = false;
    }

    public void Link(ItemSlot itemSlot) {
        this.itemSlot = itemSlot;
        UpdateSlot(true);
    }

    public void UpdateItemSlot(bool value) {
        itemSlot.UpdateSlot(value);
    }

    public void UpdateSlot(bool check) {
        if(check) itemSlot.CheckIfEmpty();
        ItemStack stack = itemSlot.Stack;

        if(itemSlot.HasItem) {
            slotIcon.sprite = ItemRegistry.Instance.GetSpriteForID(stack.ID);
            slotAmount.text = stack.Amount.ToString();

            slotIcon.color = fullColor;
            slotAmount.enabled = true;
        }

        else {
            slotIcon.sprite = null;
            slotAmount.text = "";
        
            slotIcon.color = emptyColor;
            slotAmount.enabled = false;
        }

        if(stack.Amount == 1) slotAmount.text = "";
    }
}
