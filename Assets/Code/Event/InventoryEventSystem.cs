using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-200)]
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
    private WorldAudioPlayer worldAudioPlayer;

    private event EventHandler<ushort> targetSlotUpdateEvent;
    private event EventHandler<SwitchedItemStack> modifyHeldSlotEvent;
    private event EventHandler<ItemPickupData> itemPickupEvent;
    private event EventHandler<ItemSlotIndex> hotbarUpdateEvent;

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerBuild = PlayerBuild.Instance;
        playerHand = PlayerHand.Instance;
        inventory = Inventory.Instance;
        hotbar = Hotbar.Instance;
        worldAudioPlayer = WorldAudioPlayer.Instance;

        AddTargetSlotUpdateListeners();
        AddModifyHeldSlotListeners();
        AddItemPickupListeners();
        AddHotbarUpdateListeners();
    }

    private void AddTargetSlotUpdateListeners() {
        targetSlotUpdateEvent += playerBuild.ModifyTargetBlock;
    }

    private void AddModifyHeldSlotListeners() {
        modifyHeldSlotEvent += playerHand.SwitchHeldItem;
    }

    private void AddItemPickupListeners() {
        itemPickupEvent += inventory.AddStack;
        itemPickupEvent += playerBuild.ModifyTargetBlock;
        itemPickupEvent += worldAudioPlayer.PlayItemPickup;
    }

    private void AddHotbarUpdateListeners() {
        hotbarUpdateEvent += hotbar.UpdateHeldItem;
    }

    public void InvokeTargetSlotUpdate(ushort id) {
        targetSlotUpdateEvent.Invoke(this, id);
    }

    public void InvokeModifyHeldSlot(SwitchedItemStack stack) {
        modifyHeldSlotEvent.Invoke(this, stack);
    }

    public void InvokeItemPickup(ItemPickupData data) {
        itemPickupEvent.Invoke(this, data);
    }

    public void InvokeHotbarUpdate(ItemSlotIndex data) {
        hotbarUpdateEvent.Invoke(this, data);
    }
}
