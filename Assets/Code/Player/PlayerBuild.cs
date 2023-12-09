using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBuild : MonoBehaviour
{
    public static PlayerBuild Instance { get; private set; }

    [SerializeField] private float reach = 5.0f;
    [SerializeField] private float checkIncrement = 0.5f;

    [SerializeField] private GameObject blockOutline;
    [SerializeField] private GameObject blockCrackOutline;

    [SerializeField] private LayerMask chunkMask;
    [SerializeField] private LayerMask playerMask;

    private PlayerEventSystem playerEventSystem;

    private Animator blockCrackAnimator;
    private Camera playerCamera;

    private Vector3Int targetPos;
    private Vector3Int previousTargetPos;
    private Vector3Int highlightPos;

    private float currentBlockBreakProgress;
    private float maxBlockBreakProgress = 1.0f;

    private ushort targetRaycastBlock;
    private ushort targetBlock;

    private bool isMining = false;
    private bool isEnabled = true;

    private RaycastHit[] raycastHit;

    private RaycastHit HitData {
        get { return raycastHit[0]; }
        set { raycastHit[0] = value; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;
        raycastHit = new RaycastHit[1];

        playerCamera = Camera.main;
        blockCrackAnimator = blockCrackOutline.GetComponent<Animator>();
    }

    private void Update() {
        if(!isEnabled) return;

        RaycastIntoWorld();
        UpdateBlockOutline();
        UpdateBlockCrackOutline();

        AdjustBlockCrackAnimatorSpeed();
        GetInputs();
    }

    public void Enable(object sender, EventArgs e) {
        isEnabled = true;
        blockCrackAnimator.enabled = true;
    }

    public void Disable(object sender, EventArgs e) {
        isEnabled = false;
        blockCrackAnimator.enabled = false;
    }

    private void GetInputs() {
        isMining = Input.GetButton("Click");

        if(isMining) MineCurrentBlock();
        else ResetMiningProgress(true);

        if(Input.GetButtonDown("Click2")) PlaceTargetBlock();
    }

    private void MineCurrentBlock() {
        Vector3 offsetTargetPos = GetOffsetTargetPos();

        if(!CanModifyAt(offsetTargetPos)) return;
        if(!blockCrackOutline.activeSelf) return;

        if(currentBlockBreakProgress == 0.0f) {
            blockCrackAnimator.enabled = true;

            blockCrackAnimator.Rebind();
            blockCrackAnimator.Update(0.0f);

            blockCrackAnimator.Play("blockcrack");
            playerEventSystem.InvokeBlockBreakStart();
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

    private void AdjustBlockCrackAnimatorSpeed() {
        blockCrackAnimator.speed = GetBlockBreakSpeed() * 0.9f;
    }
    
    private float GetBlockBreakSpeed() {
        BlockType type = BlockRegistry.BlockTypeDictionary[targetRaycastBlock];
        return 1.0f / type.hardness;
    }

    public void ModifyTargetBlock(object sender, ItemStack stack) {
        ModifyTargetBlock(stack.ID);
    }

    public void ModifyTargetBlock(ushort id) {
        targetBlock = id;
    }

    public void ModifyTargetBlock(object sender, ushort id) {
        ModifyTargetBlock(id);
    }

    private void DestroyTargetBlock() {
        Vector3 offsetTargetPos = GetOffsetTargetPos();

        if(!CanModifyAt(offsetTargetPos)) return;
        ushort block = WorldModifier.GetBlockAt(targetPos.x, targetPos.y, targetPos.z);

        WorldModifier.ModifyBlocks(new List<VoxelModification>() {
            new VoxelModification(targetPos.x, targetPos.y, targetPos.z, 0)
        });

        Vector3 blockBreakParticlePosition = GetOffsetTargetPos();
        
        BlockBreakData blockBreakData = new BlockBreakData(
            blockBreakParticlePosition.x, blockBreakParticlePosition.y, blockBreakParticlePosition.z,
            block, 1);

        playerEventSystem.InvokeBlockBreak(blockBreakData);
    }

    private void PlaceTargetBlock() {
        Vector3 offsetHighlightPos = GetOffsetHighlightPos();

        if(!CanModifyAt(offsetHighlightPos)) return;
        if(targetBlock == 0) return;

        WorldModifier.ModifyBlocks(new List<VoxelModification>() {
            new VoxelModification(highlightPos.x, highlightPos.y, highlightPos.z, targetBlock)
        });

        playerEventSystem.InvokeBlockPlace();
    }

    private void RaycastIntoWorld() {
        if(previousTargetPos != null) previousTargetPos = targetPos;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        float step = checkIncrement;
        Vector3 playerPosition = playerCamera.transform.position;

        int positionX = Mathf.FloorToInt(playerPosition.x);
        int positionY = Mathf.FloorToInt(playerPosition.y);
        int positionZ = Mathf.FloorToInt(playerPosition.z);

        Vector3Int playerPositionFloored = new Vector3Int(positionX, positionY, positionZ);

        targetPos = playerPositionFloored;
        highlightPos = playerPositionFloored;

        if(Physics.RaycastNonAlloc(ray, raycastHit, reach, chunkMask) != 0) {
            Vector3 targetPoint = HitData.point - HitData.normal * 0.1f;
            Vector3 targetHighlightPoint = HitData.point + HitData.normal * 0.1f;
        
            int targetPosX = Mathf.FloorToInt(targetPoint.x);
            int targetPosY = Mathf.FloorToInt(targetPoint.y);
            int targetPosZ = Mathf.FloorToInt(targetPoint.z);

            int targetHighlightPosX = Mathf.FloorToInt(targetHighlightPoint.x);
            int targetHighlightPosY = Mathf.FloorToInt(targetHighlightPoint.y);
            int targetHighlightPosZ = Mathf.FloorToInt(targetHighlightPoint.z);

            targetRaycastBlock = WorldModifier.GetBlockAt(targetPosX, targetPosY, targetPosZ);

            if(targetRaycastBlock != 0) {
                targetPos.x = targetPosX;
                targetPos.y = targetPosY;
                targetPos.z = targetPosZ;

                highlightPos.x = targetHighlightPosX;
                highlightPos.y = targetHighlightPosY;
                highlightPos.z = targetHighlightPosZ;
            }
        }
    }

    private bool CanModifyAt(Vector3 position) {
        return !Physics.CheckBox(position, Vector3.one * 0.5f, Quaternion.identity, playerMask, QueryTriggerInteraction.Ignore);
    }

    private void UpdateBlockOutline() {
        Vector3 blockOutlinePosition = GetOffsetTargetPos();

        blockOutline.SetActive(CanModifyAt(blockOutlinePosition));
        blockOutline.transform.position = blockOutlinePosition;
    }

    private void UpdateBlockCrackOutline() {
        Vector3 blockOutlinePosition = GetOffsetTargetPos();
        bool shouldCrackOutlineBeActive = CanModifyAt(blockOutlinePosition) && isMining;

        blockCrackOutline.SetActive(CanModifyAt(blockOutlinePosition) && isMining);
        blockCrackOutline.transform.position = blockOutlinePosition;

        if(previousTargetPos != targetPos) ResetMiningProgress(false);
    }

    private Vector3 GetOffsetTargetPos() {
        return new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, targetPos.z + 0.5f);
    }

    private Vector3 GetOffsetHighlightPos() {
        return new Vector3(highlightPos.x + 0.5f, highlightPos.y + 0.5f, highlightPos.z + 0.5f);
    }
}