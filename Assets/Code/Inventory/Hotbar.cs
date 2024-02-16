using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance { get; private set; }

    [SerializeField] private RectTransform hightlight;
    [SerializeField] private int hotbarLength = 9;

    private bool hotbarEnabled = true;
    private int slotIndex = 0;

    private InventoryEventSystem inventoryEventSystem;
    private Inventory inventory;

    public ItemSlot CurrentSlot {
        get { return GetSlot(slotIndex); }
    }

    public int SlotIndex {
        get { return slotIndex; }
        set { slotIndex = value; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        inventoryEventSystem = InventoryEventSystem.Instance;
        inventory = Inventory.Instance;
    }

    private void Update() {
        if(!hotbarEnabled) return;

        GetHotbarButtonInputs();
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

    private void GetHotbarButtonInputs() {
        ItemSlot previousSlot = GetSlot(slotIndex);

        if(Input.GetButtonDown("Hotbar0")) slotIndex = 0;
        if(Input.GetButtonDown("Hotbar1")) slotIndex = 1;
        if(Input.GetButtonDown("Hotbar2")) slotIndex = 2;
        if(Input.GetButtonDown("Hotbar3")) slotIndex = 3;
        if(Input.GetButtonDown("Hotbar4")) slotIndex = 4;
        if(Input.GetButtonDown("Hotbar5")) slotIndex = 5;
        if(Input.GetButtonDown("Hotbar6")) slotIndex = 6;
        if(Input.GetButtonDown("Hotbar7")) slotIndex = 7;
        if(Input.GetButtonDown("Hotbar8")) slotIndex = 8;

        ItemSlot currentSlot = GetSlot(slotIndex);

        if(!CheckIfSlotsAreSimilar(previousSlot, currentSlot)) {
            UpdateHeldItem(1.0f / 20f, new ItemSlotIndex(slotIndex, 0));
        }
    }

    public void UpdateHeldItem(ItemStack stack) {
        UpdateHeldItem(1.0f / 20f, new ItemSlotIndex(slotIndex, stack.Amount));
    }

    public void UpdateHeldItem(object sender, EventArgs e) {
        UpdateHeldItem(1.0f / 20f, new ItemSlotIndex(slotIndex, 0));
    }

    public void UpdateHeldItem(float scroll) {
        UpdateHeldItem(scroll, new ItemSlotIndex(slotIndex, 0));
    }

    public void UpdateHeldItem(object sender, ItemSlotIndex data) {
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

        SwitchedItemStack switchedItemStack = new SwitchedItemStack(currentSlot.Stack, switchTime);
        inventoryEventSystem.InvokeModifyHeldSlot(switchedItemStack);
    }

    private void UpdateHighlightPosition() {
        if(inventory == null) return;
        hightlight.position = GetCurrentSlotIcon().transform.position;
    }

    private void UpdatePlayerTargetBlock() {
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
        inventoryEventSystem.InvokeTargetSlotUpdate(id);
    }

    public void SetStatus(object sender, bool status) {
        hotbarEnabled = status;
    }

    public void Enable(object sender, EventArgs e) {
        hotbarEnabled = true;
    }

    public void Disable(object sender, EventArgs e) {
        hotbarEnabled = false;
    }

    public void TakeFromCurrentSlot(object sender, BlockModifyData data) {
        if(inventory == null) {
            Debug.LogError("The Inventory script must be present in the scene in order to take from the current slot.");
            return;
        }

        GetCurrentSlot().Take(1);
        RefreshHeldSlot();
    }

    public void RefreshHeldSlot() {
        ItemSlot itemSlot = GetCurrentSlot();
        inventoryEventSystem.InvokeModifyHeldSlot(new SwitchedItemStack(itemSlot.Stack, 0.0f));
    }

    private ItemSlot GetCurrentSlot() {
        return GetSlot(slotIndex);
    }

    private Image GetCurrentSlotIcon() {
        return GetSlotIcon(slotIndex);
    }

    private ItemSlot GetSlot(int index) {
        return inventory.HotbarSlots[index].ItemSlot;
    } 

    private Image GetSlotIcon(int index) {
        return inventory.HotbarSlots[index].SlotIcon;
    }
}
