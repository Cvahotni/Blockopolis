using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBuild : MonoBehaviour
{
    public static PlayerBuild Instance { get; private set; }

    [SerializeField] private float reach = 5.0f;
    [SerializeField] private float checkIncrement = 0.5f;
    [SerializeField] private float minCheckIncrement = 0.0001f;
    [SerializeField] private float checkIncrementDivisionAmount = 8;
    [SerializeField] private float breakDelay = 0.1f;
    [SerializeField] private float progressDelay = 0.25f;

    [SerializeField] private GameObject blockOutline;
    [SerializeField] private GameObject blockCrackOutline;

    [SerializeField] private LayerMask chunkMask;
    [SerializeField] private LayerMask playerMask;

    private PlayerEventSystem playerEventSystem;
    private ItemRegistry itemRegistry;

    private Animator blockCrackAnimator;
    private Camera playerCamera;

    private Vector3Int targetPos;
    private Vector3Int highlightPos;

    private WaitForSeconds mineDelayWaitForSeconds;

    private float currentBlockBreakProgress;
    private float maxBlockBreakProgress = 1.0f;

    private BlockID targetRaycastBlock;
    private ushort targetBlockID;

    private bool isMining = false;
    private bool isEnabled = true;
    private bool canMine = true;

    private Vector3 previousTargetPos;
    private BlockModifyData blockBreakStartData;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;

        playerCamera = Camera.main;
        blockCrackAnimator = blockCrackOutline.GetComponent<Animator>();
        itemRegistry = ItemRegistry.Instance;

        mineDelayWaitForSeconds = new WaitForSeconds(breakDelay);
        InvokeRepeating("PlayMiningSound", 0.0f, progressDelay);
    }

    private void Update() {
        if(!isEnabled) return;

        UpdateBlockOutline();
        UpdateBlockCrackOutline();
        AdjustBlockCrackAnimatorSpeed();
        GetPlaceInputs();
    }

    private void FixedUpdate() {
        if(!isEnabled) return;

        RaycastIntoWorld();
        GetMineInputs();
    }

    public void Enable(object sender, EventArgs e) {
        isEnabled = true;
        blockCrackAnimator.enabled = true;
    }

    public void Disable(object sender, EventArgs e) {
        isEnabled = false;
        blockCrackAnimator.enabled = false;
    }

    private void GetMineInputs() {
        isMining = Input.GetButton("Click");

        if(isMining) MineCurrentBlock();
        else ResetMiningProgress(true);
    }

    private void GetPlaceInputs() {
        if(Input.GetButtonDown("Click2")) PlaceTargetBlock();
    }

    private void PlayMiningSound() {
        if(isMining) playerEventSystem.InvokeBlockBreakProgress(blockBreakStartData);
    }

    private void MineCurrentBlock() {
        Vector3 offsetTargetPos = GetOffsetTargetPos();

        if(!CanModifyAt(offsetTargetPos)) {
            ResetMiningProgress(true);
            return;
        }

        if(!blockCrackOutline.activeSelf) return;
        if(!canMine) return;

        BlockID block = WorldModifier.GetBlockAt(targetPos.x, targetPos.y, targetPos.z);

        blockBreakStartData = new BlockModifyData(
            offsetTargetPos.x, offsetTargetPos.y, offsetTargetPos.z,
        block, 1);

        if(currentBlockBreakProgress == 0.0f) {
            blockCrackAnimator.enabled = true;

            blockCrackAnimator.Rebind();
            blockCrackAnimator.Update(0.0f);

            blockCrackAnimator.Play("blockcrack");
            playerEventSystem.InvokeBlockBreakStart(blockBreakStartData);
        }

        currentBlockBreakProgress += Time.deltaTime * Time.timeScale * GetBlockBreakSpeed();

        if(currentBlockBreakProgress >= maxBlockBreakProgress) {
            currentBlockBreakProgress = 0.0f;
            DestroyTargetBlock();
        }
    }

    private void ResetMiningProgress(bool endAnimation) {
        currentBlockBreakProgress = 0.0f;
        blockCrackAnimator.enabled = false;

        if(endAnimation) playerEventSystem.InvokeBlockBreakEnd();
    }

    private IEnumerator MineDelayCoroutine() {
        yield return mineDelayWaitForSeconds;
        canMine = true;
    }

    private void AdjustBlockCrackAnimatorSpeed() {
        blockCrackAnimator.speed = GetBlockBreakSpeed() * 0.9f;
    }
    
    private float GetBlockBreakSpeed() {
        BlockState state = BlockRegistry.BlockStateDictionary[targetRaycastBlock.Pack()];

        if(itemRegistry == null) return 1.0f / state.hardness;
        return itemRegistry.GetItemMineMultiplier(targetBlockID, targetRaycastBlock) / state.hardness;
    }

    public void ModifyTargetBlock(object sender, ItemPickupData data) {
        ModifyTargetBlock(data.itemStack.ID);
    }

    public void ModifyTargetBlock(ushort id) {
        targetBlockID = id;
    }

    public void ModifyTargetBlock(object sender, ushort id) {
        ModifyTargetBlock(id);
    }

    private void DestroyTargetBlock() {
        Vector3 offsetTargetPos = GetOffsetTargetPos();

        if(!CanModifyAt(offsetTargetPos)) return;
        BlockID block = WorldModifier.GetBlockAt(targetPos.x, targetPos.y, targetPos.z);

        WorldModifier.ModifyBlocks(new List<VoxelModification>() {
            new VoxelModification(targetPos.x, targetPos.y, targetPos.z, new BlockID(0))
        });

        Vector3 blockBreakParticlePosition = GetOffsetTargetPos();
        
        BlockModifyData blockBreakData = new BlockModifyData(
            blockBreakParticlePosition.x, blockBreakParticlePosition.y, blockBreakParticlePosition.z,
            block, 1);

        playerEventSystem.InvokeBlockBreak(blockBreakData);

        canMine = false;
        StartCoroutine(MineDelayCoroutine());
    }

    private void PlaceTargetBlock() {
        Vector3 offsetHighlightPos = GetOffsetHighlightPos();

        if(!CanModifyAt(offsetHighlightPos)) return;
        if(targetBlockID == 0) return;

        BlockID targetBlock = BlockID.FromUShort(targetBlockID);

        if(itemRegistry != null) {
            if(!itemRegistry.IsItemBlockItem(targetBlock.id)) return;
        }

        WorldModifier.ModifyBlocks(new List<VoxelModification>() {
            new VoxelModification(highlightPos.x, highlightPos.y, highlightPos.z, targetBlock)
        });

        BlockModifyData blockPlaceData = new BlockModifyData(
            highlightPos.x, highlightPos.y, highlightPos.z,
            targetBlock, 1);

        playerEventSystem.InvokeBlockPlace(blockPlaceData);
    }

    private void RaycastIntoWorld() {
        if(previousTargetPos != null) previousTargetPos = targetPos;

        float stepAmount = checkIncrement;
        float step = stepAmount;

        Vector3 playerPosition = playerCamera.transform.position;

        int positionX = Mathf.FloorToInt(playerPosition.x);
        int positionY = Mathf.FloorToInt(playerPosition.y);
        int positionZ = Mathf.FloorToInt(playerPosition.z);

        Vector3Int playerPositionFloored = new Vector3Int(positionX, positionY, positionZ);
        Vector3 lastPos = new Vector3();

        while(step < reach) {
            Vector3 pos = playerCamera.transform.position + (playerCamera.transform.forward * step);
            Vector3Int posFlooredAsInt = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            targetRaycastBlock = WorldModifier.GetBlockAt(posFlooredAsInt.x, posFlooredAsInt.y, posFlooredAsInt.z);

            if(!targetRaycastBlock.IsAir()) {
                if(stepAmount > minCheckIncrement) {
                    step -= stepAmount;
                    stepAmount /= checkIncrementDivisionAmount;
                }

                else {
                    lastPos = pos - (playerCamera.transform.forward * checkIncrement);
                    Vector3Int lastPosFloored = new Vector3Int(Mathf.FloorToInt(lastPos.x), Mathf.FloorToInt(lastPos.y), Mathf.FloorToInt(lastPos.z));

                    highlightPos = lastPosFloored;
                    targetPos = posFlooredAsInt;
            
                    return;
                }
            }

            step += stepAmount;
            lastPos = posFlooredAsInt;
        }

        targetPos = playerPositionFloored;
        highlightPos = playerPositionFloored;
    }

    private void ResetRaycastPositions() {
        Vector3 playerPosition = playerCamera.transform.position;

        int positionX = Mathf.FloorToInt(playerPosition.x);
        int positionY = Mathf.FloorToInt(playerPosition.y);
        int positionZ = Mathf.FloorToInt(playerPosition.z);

        Vector3Int playerPositionFloored = new Vector3Int(positionX, positionY, positionZ);

        targetPos = playerPositionFloored;
        highlightPos = playerPositionFloored;
    }

    private bool CanModifyAt(Vector3 position) {
        bool boxCheck = !Physics.CheckBox(position, Vector3.one * 0.5f, Quaternion.identity, playerMask, QueryTriggerInteraction.Ignore);
        bool skyCheck = position.y < VoxelProperties.chunkHeight;
        bool voidCheck = position.y >= 0;

        return boxCheck && skyCheck && voidCheck;
    }

    private void UpdateBlockOutline() {
        Vector3 blockOutlinePosition = GetOffsetTargetPos();

        blockOutline.SetActive(CanModifyAt(blockOutlinePosition) && !IsAir(blockOutlinePosition));
        blockOutline.transform.position = blockOutlinePosition;
    }

    private void UpdateBlockCrackOutline() {
        Vector3 blockOutlinePosition = GetOffsetTargetPos();
        bool shouldCrackOutlineBeActive = CanModifyAt(blockOutlinePosition) && isMining && !IsAir(blockOutlinePosition);

        blockCrackOutline.SetActive(shouldCrackOutlineBeActive);
        blockCrackOutline.transform.position = blockOutlinePosition;
    }

    private bool IsAir(Vector3 position) {
        return WorldModifier.GetBlockAt(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z)).IsAir();
    }

    private Vector3 GetOffsetTargetPos() {
        return new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, targetPos.z + 0.5f);
    }

    private Vector3 GetOffsetHighlightPos() {
        return new Vector3(highlightPos.x + 0.5f, highlightPos.y + 0.5f, highlightPos.z + 0.5f);
    }
}
