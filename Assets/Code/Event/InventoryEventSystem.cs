using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(200)]
public class InventoryEventSystem : MonoBehaviour
{
    public static InventoryEventSystem Instance {
        get {
            if(_instance == null) {
                Debug.LogError("The InventoryEventSystem must be present in the scene at all times.");
            }

            return _instance;
        }

        set {
            _instance = value;
        }
    }

    private static InventoryEventSystem _instance;

    private PlayerBuild playerBuild;
    private PlayerHand playerHand;
    private Inventory inventory;
    private Hotbar hotbar;

    private UnityEvent<ushort> targetSlotUpdateEvent = new UnityEvent<ushort>();
    private UnityEvent<SwitchedItemStack> modifyHeldSlotEvent = new UnityEvent<SwitchedItemStack>();
    private UnityEvent<ItemStack> itemPickupEvent = new UnityEvent<ItemStack>();
    private UnityEvent<ItemSlotIndex> hotbarUpdateEvent = new UnityEvent<ItemSlotIndex>();

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerBuild = PlayerBuild.Instance;
        playerHand = PlayerHand.Instance;
        inventory = Inventory.Instance;
        hotbar = Hotbar.Instance;

        AddTargetSlotUpdateListeners();
        AddModifyHeldSlotListeners();
        AddItemPickupListeners();
        AddHotbarUpdateListeners();
    }

    private void AddTargetSlotUpdateListeners() {
        targetSlotUpdateEvent.AddListener(playerBuild.ModifyTargetBlock);
    }

    private void AddModifyHeldSlotListeners() {
        modifyHeldSlotEvent.AddListener(playerHand.SwitchHeldItem);
    }

    private void AddItemPickupListeners() {
        itemPickupEvent.AddListener(inventory.AddStack);
        itemPickupEvent.AddListener(playerBuild.ModifyTargetBlock);
    }

    private void AddHotbarUpdateListeners() {
        hotbarUpdateEvent.AddListener(hotbar.UpdateHeldItem);
    }

    public void InvokeTargetSlotUpdate(ushort id) {
        targetSlotUpdateEvent.Invoke(id);
    }

    public void InvokeModifyHeldSlot(SwitchedItemStack stack) {
        modifyHeldSlotEvent.Invoke(stack);
    }

    public void InvokeItemPickup(ItemStack stack) {
        itemPickupEvent.Invoke(stack);
    }

    public void InvokeHotbarUpdate(ItemSlotIndex data) {
        hotbarUpdateEvent.Invoke(data);
    }
}
