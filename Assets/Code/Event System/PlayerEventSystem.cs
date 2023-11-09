using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(300)]
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

    private UnityEvent<bool> groundCheckEvent = new UnityEvent<bool>();
    private UnityEvent<BlockBreakData> blockBreakEvent = new UnityEvent<BlockBreakData>();
    private UnityEvent blockPlaceEvent = new UnityEvent();
    private UnityEvent playerSpawnEvent = new UnityEvent();

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerBlockBreakEffect = PlayerBlockBreakEffect.Instance;
        droppedItemFactory = DroppedItemFactory.Instance;
        playerMove = PlayerMove.Instance;
        playerHand = PlayerHand.Instance;
        hotbar = Hotbar.Instance;

        if(playerMove == null || playerHand == null) {
            Debug.LogError("The Player object must be enabled for the game to start.");
            return;
        }

        AddGroundCheckListeners();
        AddBlockBreakListeners();
        AddBlockPlaceListeners();
        AddPlayerSpawnListeners();
    } 

    private void AddGroundCheckListeners() {
        groundCheckEvent.AddListener(playerMove.UpdateIsGrounded);
    }

    private void AddBlockBreakListeners() {
        blockBreakEvent.AddListener(playerBlockBreakEffect.PlayBlockBreakParticle);
        blockBreakEvent.AddListener(droppedItemFactory.DropItemFromBlock);
    }

    private void AddBlockPlaceListeners() {
        blockPlaceEvent.AddListener(hotbar.TakeFromCurrentSlot);
    }

    private void AddPlayerSpawnListeners() {
        playerSpawnEvent.AddListener(playerMove.TeleportToSpawn);
        playerSpawnEvent.AddListener(hotbar.UpdateHeldItem);
    }

    public void InvokeGroundCheck(bool value) {
        groundCheckEvent.Invoke(value);
    }

    public void InvokeBlockBreak(BlockBreakData data) {
        blockBreakEvent.Invoke(data);
    }

    public void InvokeBlockPlace() {
        blockPlaceEvent.Invoke();
    }

    public void InvokePlayerSpawn() {
        playerSpawnEvent.Invoke();
    }
}
