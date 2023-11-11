using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuild : MonoBehaviour
{
    public static PlayerBuild Instance { get; private set; }

    [SerializeField] private float reach = 5.0f;
    [SerializeField] private float checkIncrement = 0.5f;
    [SerializeField] private float playerBounds = 0.5f;

    [SerializeField] private GameObject blockOutline;
    [SerializeField] private GameObject blockCrackOutline;

    private Animator blockCrackAnimator;
    private Camera playerCamera;

    private Vector3Int targetPos;
    private Vector3Int previousTargetPos;

    private Vector3Int highlightPos;

    private float currentBlockBreakProgress;
    private float maxBlockBreakProgress = 1.0f;

    private bool isMining = false;

    private PlayerEventSystem playerEventSystem;

    private ushort targetRaycastBlock;
    private ushort targetBlock;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerEventSystem = PlayerEventSystem.Instance;

        playerCamera = Camera.main;
        blockCrackAnimator = blockCrackOutline.GetComponent<Animator>();
    }

    private void Update() {
        RaycastIntoWorld();
        UpdateBlockOutline();
        UpdateBlockCrackOutline();

        AdjustBlockCrackAnimatorSpeed();
        GetInputs();
    }

    private void GetInputs() {
        isMining = Input.GetButton("Click");

        if(isMining) MineCurrentBlock();
        else ResetMiningProgress(true);

        if(Input.GetButtonDown("Click2")) PlaceTargetBlock();
    }

    private void MineCurrentBlock() {
        if(!CanModifyAt(targetPos)) return;
        if(!blockCrackOutline.activeSelf) return;

        if(currentBlockBreakProgress == 0.0f) {
            blockCrackAnimator.enabled = true;

            blockCrackAnimator.Rebind();
            blockCrackAnimator.Update(0.0f);

            blockCrackAnimator.Play("blockcrack");
            playerEventSystem.InvokeBlockBreakStart();
        }

        currentBlockBreakProgress += Time.deltaTime * GetBlockBreakSpeed();

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

    public void ModifyTargetBlock(ItemStack stack) {
        ModifyTargetBlock(stack.ID);
    }

    public void ModifyTargetBlock(ushort id) {
        targetBlock = id;
    }

    private void DestroyTargetBlock() {
        if(!CanModifyAt(targetPos)) return;
        ushort block = WorldModifier.GetBlockAt(targetPos.x, targetPos.y, targetPos.z);

        WorldModifier.ModifyBlocks(new List<VoxelModification>() {
            new VoxelModification(targetPos, 0)
        });

        Vector3 blockBreakParticlePosition = GetOffsetTargetPos();
        
        BlockBreakData blockBreakData = new BlockBreakData(
            blockBreakParticlePosition.x, blockBreakParticlePosition.y, blockBreakParticlePosition.z,
            block, 1);

        playerEventSystem.InvokeBlockBreak(blockBreakData);
    }

    private void PlaceTargetBlock() {
        if(!CanModifyAt(highlightPos)) return;
        if(targetBlock == 0) return;

        WorldModifier.ModifyBlocks(new List<VoxelModification>() {
            new VoxelModification(highlightPos, targetBlock)
        });

        playerEventSystem.InvokeBlockPlace();
    }

    private void RaycastIntoWorld() {
        if(previousTargetPos != null) previousTargetPos = targetPos;

        float step = checkIncrement;
        Vector3 playerPosition = playerCamera.transform.position;

        int positionX = Mathf.FloorToInt(playerPosition.x);
        int positionY = Mathf.FloorToInt(playerPosition.y);
        int positionZ = Mathf.FloorToInt(playerPosition.z);

        Vector3Int playerPositionFloored = new Vector3Int(positionX, positionY, positionZ);
        Vector3 lastPos = new Vector3();

        while(step < reach) {
            Vector3 pos = playerCamera.transform.position + playerCamera.transform.forward * step;
            Vector3Int posFlooredAsInt = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));

            targetRaycastBlock = WorldModifier.GetBlockAt(posFlooredAsInt.x, posFlooredAsInt.y, posFlooredAsInt.z);

            if(targetRaycastBlock != 0) {
                Vector3Int lastPosFloored = new Vector3Int(Mathf.FloorToInt(lastPos.x), Mathf.FloorToInt(lastPos.y), Mathf.FloorToInt(lastPos.z));

                highlightPos = lastPosFloored;
                targetPos = posFlooredAsInt;
            
                return;
            }
            
            lastPos = posFlooredAsInt;
            step += checkIncrement;
        }

        targetPos = playerPositionFloored;
        highlightPos = playerPositionFloored;
    }

    private bool CanModifyAt(Vector3Int position) {
        int cameraPositionX = Mathf.FloorToInt(playerCamera.transform.position.x);
        int cameraPositionY = Mathf.FloorToInt(playerCamera.transform.position.y);
        int cameraPositionZ = Mathf.FloorToInt(playerCamera.transform.position.z);
    
        Vector3Int boxPosition = new Vector3Int(cameraPositionX, Mathf.FloorToInt(cameraPositionY + 0.5f), cameraPositionZ);
        Vector3Int boxLowerPosition = new Vector3Int(cameraPositionX, Mathf.FloorToInt(cameraPositionY - 0.5f), cameraPositionZ);

        bool boxCheck = Vector3Int.Distance(position, boxPosition) <= playerBounds;
        bool boxLowerCheck = Vector3Int.Distance(position, boxLowerPosition) <= playerBounds;

        bool boxBoundsCheck = !boxCheck && !boxLowerCheck;
        bool yIsInWorld = position.y >= 0 && position.y < VoxelProperties.chunkHeight;

        return boxBoundsCheck && yIsInWorld;
    }

    private void UpdateBlockOutline() {
        Vector3 blockOutlinePosition = GetOffsetTargetPos();

        blockOutline.SetActive(CanModifyAt(targetPos));
        blockOutline.transform.position = blockOutlinePosition;
    }

    private void UpdateBlockCrackOutline() {
        Vector3 blockOutlinePosition = GetOffsetTargetPos();
        bool shouldCrackOutlineBeActive = CanModifyAt(targetPos) && isMining;

        blockCrackOutline.SetActive(CanModifyAt(targetPos) && isMining);
        blockCrackOutline.transform.position = blockOutlinePosition;

        if(previousTargetPos != targetPos) ResetMiningProgress(false);
    }

    private Vector3 GetOffsetTargetPos() {
        return new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, targetPos.z + 0.5f);
    }
}
