using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotbar : MonoBehaviour
{
    public static Hotbar Instance { get; private set; }

    [SerializeField]
    private UiItemSlot[] slots;

    [SerializeField]
    private RectTransform hightlight;

    [SerializeField]
    private int maxStackSize = 5;

    private bool hotbarEnabled = false;

    public bool HotbarEnabled {
        get { return hotbarEnabled; }
        set { hotbarEnabled = value; }
    }

    public int MaxStackSize {
        get { return maxStackSize; }
    }

    private int slotIndex = 0;

    private PlayerBuild playerBuild;
    private ItemRegistry itemRegistry;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        itemRegistry = ItemRegistry.Instance;
        InitItemSlots();
    }

    private void InitItemSlots() {
        ushort index = 1;

        for(int i = 0; i < slots.Length; i++) {
            ItemStack itemStack = new ItemStack(0, 0);
            ItemSlot itemSlot = new ItemSlot(slots[index - 1], itemStack);

            index++;
        }
    }

    private void Update() {
        if(!hotbarEnabled) return;
        
        if(playerBuild == null && hotbarEnabled) {
            playerBuild = PlayerBuild.Instance;
            return;
        }

        UpdateSlotIndex();
        UpdateHighlightPosition();
        UpdatePlayerTargetBlock();
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

    private void SetStack(int index, ItemStack stack) {
        slots[index].ItemSlot.Stack = stack;
        slots[index].UpdateSlot(true);
    }

    private void UpdateSlotIndex() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll == 0) return;

        if(scroll > 0) slotIndex--;
        else slotIndex++;

        if(slotIndex > slots.Length - 1) slotIndex = 0;
        if(slotIndex < 0) slotIndex = slots.Length - 1;
    }

    private void UpdateHighlightPosition() {
        hightlight.position = slots[slotIndex].SlotIcon.transform.position;
    }

    private void UpdatePlayerTargetBlock() {
        ItemSlot currentSlot = slots[slotIndex].ItemSlot;

        if(!currentSlot.HasItem) {
            playerBuild.TargetBlock = 0;
            return;
        }

        ushort id = currentSlot.Stack.ID;

        if(!itemRegistry.IsItemForm(id, ItemForm.BlockItem)) return;
        playerBuild.TargetBlock = id;
    }

    public void TakeFromCurrentSlot() {
        slots[slotIndex].ItemSlot.Take(1);
    }
}
