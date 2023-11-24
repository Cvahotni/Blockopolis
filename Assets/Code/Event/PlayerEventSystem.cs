using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

[DefaultExecutionOrder(-300)]
public class PlayerEventSystem : MonoBehaviour
{
    public static PlayerEventSystem Instance {
        get {
            if(_instance == null) {
                Debug.LogError("The PlayerEventSystem must be present in the scene at all times.");
            }

            return _instance;
        }

        set {
            _instance = value;
        }
    }

    private static PlayerEventSystem _instance;

    private PlayerBlockBreakEffect playerBlockBreakEffect;
    private DroppedItemFactory droppedItemFactory;
    private PlayerMove playerMove;
    private PlayerHand playerHand;
    private Hotbar hotbar;
    private MouseLook mouseLook;

    private event EventHandler<bool> groundCheckEvent;
    private event EventHandler blockBreakStartEvent;
    private event EventHandler blockBreakEndEvent;
    private event EventHandler<BlockBreakData> blockBreakEvent;
    private event EventHandler blockPlaceEvent;
    private event EventHandler playerSpawnEvent;

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerBlockBreakEffect = PlayerBlockBreakEffect.Instance;
        droppedItemFactory = DroppedItemFactory.Instance;
        playerMove = PlayerMove.Instance;
        playerHand = PlayerHand.Instance;
        hotbar = Hotbar.Instance;
        mouseLook = MouseLook.Instance;

        if(playerMove == null || playerHand == null) {
            Debug.LogError("The Player object must be enabled for the game to start.");
            return;
        }

        AddGroundCheckListeners();
        AddBlockBreakStartListeners();
        AddBlockBreakEndListeners();
        AddBlockBreakListeners();
        AddBlockPlaceListeners();
        AddPlayerSpawnListeners();
    } 

    private void AddGroundCheckListeners() {
        groundCheckEvent += playerMove.UpdateIsGrounded;
    }

    private void AddBlockBreakStartListeners() {
        blockBreakStartEvent += playerHand.SwingHeldItemRepeating;
    }

    private void AddBlockBreakEndListeners() {
        blockBreakEndEvent += playerHand.ResetHandSwing;
    }

    private void AddBlockBreakListeners() {
        blockBreakEvent += playerHand.ResetLayers;
        blockBreakEvent += playerBlockBreakEffect.PlayBlockBreakParticle;
        blockBreakEvent += droppedItemFactory.DropItemFromBlock;
    }

    private void AddBlockPlaceListeners() {
        blockPlaceEvent += hotbar.TakeFromCurrentSlot;
    }

    private void AddPlayerSpawnListeners() {
        playerSpawnEvent += mouseLook.LockCursor;
        playerSpawnEvent += playerMove.TeleportToSpawn;
        playerSpawnEvent += hotbar.UpdateHeldItem;
    }

    public void InvokeGroundCheck(bool value) {
        groundCheckEvent.Invoke(this, value);
    }

    public void InvokeBlockBreakStart() {
        blockBreakStartEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokeBlockBreakEnd() {
        blockBreakEndEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokeBlockBreak(BlockBreakData data) {
        blockBreakEvent.Invoke(this, data);
    }

    public void InvokeBlockPlace() {
        blockPlaceEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerSpawn() {
        playerSpawnEvent.Invoke(this, EventArgs.Empty);
    }
}
