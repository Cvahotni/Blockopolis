using UnityEngine;
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
    private WorldAudioPlayer worldAudioPlayer;

    private event EventHandler<bool> groundCheckEvent;
    private event EventHandler<BlockModifyData> blockBreakStartEvent;
    private event EventHandler<BlockModifyData> blockBreakProgressEvent;
    private event EventHandler blockBreakEndEvent;
    private event EventHandler<BlockModifyData> blockBreakEvent;
    private event EventHandler<BlockModifyData> blockPlaceEvent;
    private event EventHandler playerSpawnEvent;
    private event EventHandler playerEnterWaterEvent;
    private event EventHandler playerExitWaterEvent;

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
        worldAudioPlayer = WorldAudioPlayer.Instance;

        if(playerMove == null || playerHand == null) {
            Debug.LogError("The Player object must be enabled for the game to start.");
            return;
        }

        AddGroundCheckListeners();
        AddBlockBreakStartListeners();
        AddBlockBreakProgressListeners();
        AddBlockBreakEndListeners();
        AddBlockBreakListeners();
        AddBlockPlaceListeners();
        AddPlayerSpawnListeners();
        AddPlayerEnterWaterListeners();
        AddPlayerExitWaterListeners();
    } 

    private void AddGroundCheckListeners() {
        groundCheckEvent += playerMove.UpdateIsGrounded;
    }

    private void AddBlockBreakStartListeners() {
        blockBreakStartEvent += playerHand.SwingHeldItemRepeating;
    }

    private void AddBlockBreakEndListeners() {
        blockBreakEndEvent += playerHand.ResetHandSwing;
        blockBreakEndEvent += worldAudioPlayer.RemoveInterruptableAudioSources;
    }

    private void AddBlockBreakProgressListeners() {
        blockBreakProgressEvent += worldAudioPlayer.PlayBlockMining;
    }

    private void AddBlockBreakListeners() {
        blockBreakEvent += playerHand.ResetLayers;
        blockBreakEvent += playerBlockBreakEffect.PlayBlockBreakParticle;
        blockBreakEvent += droppedItemFactory.DropItemFromBlock;
        blockBreakEvent += worldAudioPlayer.PlayBlockBreak;
    }

    private void AddBlockPlaceListeners() {
        blockPlaceEvent += hotbar.TakeFromCurrentSlot;
        blockPlaceEvent += worldAudioPlayer.PlayBlockPlace;
    }

    private void AddPlayerSpawnListeners() {
        playerSpawnEvent += mouseLook.LockCursor;
        playerSpawnEvent += playerMove.TeleportToSpawn;
        playerSpawnEvent += hotbar.UpdateHeldItem;
    }

    private void AddPlayerEnterWaterListeners() {
        playerEnterWaterEvent += playerMove.EnterWater;
    }

    private void AddPlayerExitWaterListeners() {
        playerExitWaterEvent += playerMove.ExitWater;
    }

    public void InvokeGroundCheck(bool value) {
        groundCheckEvent.Invoke(this, value);
    }

    public void InvokeBlockBreakStart(BlockModifyData data) {
        blockBreakStartEvent.Invoke(this, data);
    }

    public void InvokeBlockBreakProgress(BlockModifyData data) {
        blockBreakProgressEvent.Invoke(this, data);
    }

    public void InvokeBlockBreakEnd() {
        blockBreakEndEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokeBlockBreak(BlockModifyData data) {
        blockBreakEvent.Invoke(this, data);
    }

    public void InvokeBlockPlace(BlockModifyData data) {
        blockPlaceEvent.Invoke(this, data);
    }

    public void InvokePlayerSpawn() {
        playerSpawnEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerEnterWater() {
        playerEnterWaterEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokePlayerExitWater() {
        playerExitWaterEvent.Invoke(this, EventArgs.Empty);
    }
}
