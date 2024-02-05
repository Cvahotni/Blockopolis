using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerBuild : MonoBehaviour
{
    public static PlayerBuild Instance { get; private set; }

    [SerializeField] private float reach = 5.0f;
    [SerializeField] private float checkIncrement = 0.5f;
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

    private ushort targetRaycastBlock;
    private ushort targetBlock;

    private bool isMining = false;
    private bool isEnabled = true;
    private bool canMine = true;

    private RaycastHit[] raycastHits;
    private BlockModifyData blockBreakStartData;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;
        raycastHits = new RaycastHit[2];

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

        ushort block = WorldModifier.GetBlockAt(targetPos.x, targetPos.y, targetPos.z);

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
        BlockType type = BlockRegistry.BlockTypeDictionary[targetRaycastBlock];

        if(itemRegistry == null) return 1.0f / type.hardness;
        return itemRegistry.GetItemMineMultiplier(targetBlock, targetRaycastBlock) / type.hardness;
    }

    public void ModifyTargetBlock(object sender, ItemPickupData data) {
        ModifyTargetBlock(data.itemStack.ID);
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
        if(targetBlock == 0) return;

        if(itemRegistry != null) {
            if(!itemRegistry.IsItemBlockItem(targetBlock)) return;
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
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        ResetRaycastPositions();

        int closestHitIndex = -1;
        RaycastHit closestHit = new RaycastHit();

        if(Physics.RaycastNonAlloc(ray, raycastHits, reach, chunkMask) != 0) {
            Array.Sort(raycastHits, delegate(RaycastHit hit1, RaycastHit hit2) { return hit1.distance.CompareTo(hit2.distance); } );

            foreach(RaycastHit hit in raycastHits) {
                if(hit.point == Vector3.zero || hit.normal == Vector3.zero) continue;
                closestHit = closestHitIndex >= 0 ? raycastHits[closestHitIndex] : raycastHits[0];
            }

            Vector3 targetPoint = closestHit.point - closestHit.normal * 0.01f;
            Vector3 targetHighlightPoint = closestHit.point + closestHit.normal * 0.01f;
        
            int targetPosX = Mathf.FloorToInt(targetPoint.x);
            int targetPosY = Mathf.FloorToInt(targetPoint.y);
            int targetPosZ = Mathf.FloorToInt(targetPoint.z);

            int targetHighlightPosX = Mathf.FloorToInt(targetHighlightPoint.x);
            int targetHighlightPosY = Mathf.FloorToInt(targetHighlightPoint.y);
            int targetHighlightPosZ = Mathf.FloorToInt(targetHighlightPoint.z);

            ushort currentTargetRaycastBlock = WorldModifier.GetBlockAt(targetPosX, targetPosY, targetPosZ);

            if(currentTargetRaycastBlock != 0) {
                targetPos.x = targetPosX;
                targetPos.y = targetPosY;
                targetPos.z = targetPosZ;

                highlightPos.x = targetHighlightPosX;
                highlightPos.y = targetHighlightPosY;
                highlightPos.z = targetHighlightPosZ;

                targetRaycastBlock = currentTargetRaycastBlock;
            }
        }
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
        return WorldModifier.GetBlockAt(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), Mathf.FloorToInt(position.z)) == 0;
    }

    private Vector3 GetOffsetTargetPos() {
        return new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, targetPos.z + 0.5f);
    }

    private Vector3 GetOffsetHighlightPos() {
        return new Vector3(highlightPos.x + 0.5f, highlightPos.y + 0.5f, highlightPos.z + 0.5f);
    }
}
