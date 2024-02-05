using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;

public class DebugScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text positionText;
    [SerializeField] private TMP_Text chunkCoordText;
    [SerializeField] private TMP_Text chunkCoordLongText;
    [SerializeField] private TMP_Text regionLongText;
    [SerializeField] private TMP_Text regionPosText;

    [SerializeField] private Transform playerLocation;
    [SerializeField] private float updatesPerSecond;
    [SerializeField] private GameObject debugScreenObject;

    private bool debugScreenEnabled = false;
    private WaitForSeconds shortTime;

    private void Start() {
        shortTime = new WaitForSeconds(1.0f / updatesPerSecond);

        UpdateDebugScreenObject();
        StartCoroutine(UpdateTextCoroutine());
    }

    private void Update() {
        if(Input.GetButtonDown("DebugScreenToggle")) {
            debugScreenEnabled = !debugScreenEnabled;
            UpdateDebugScreenObject();
        }
    }

    private void UpdateDebugScreenObject() {
        debugScreenObject.SetActive(debugScreenEnabled);
    }

    private IEnumerator UpdateTextCoroutine() {
        for(;;) {
            yield return shortTime;

            UpdateFPSText();
            UpdatePositionText();
            UpdateChunkCoordText();
            UpdateChunkCoordLongText();
            UpdateRegionLongText();
            UpdateRegionPosText();
        }
    }

    private void UpdateFPSText() {
        if(!debugScreenEnabled) return;
        fpsText.text = Mathf.FloorToInt(1.0f / Time.unscaledDeltaTime) + " FPS";
    }

    private void UpdatePositionText() {
        if(!debugScreenEnabled) return;
        
        Vector3Int playerPos = GetPlayerPos();
        positionText.text = "Position: " + playerPos.x + ", " + playerPos.y + ", " + playerPos.z;
    }

    private void UpdateChunkCoordText() {
        if(!debugScreenEnabled) return;

        Vector3Int playerPos = GetPlayerPos();
        chunkCoordText.text = "Chunk Coord: " + (playerPos.x >> VoxelProperties.chunkBitShift) + ", " + (playerPos.z >> VoxelProperties.chunkBitShift);
    }

    private void UpdateRegionLongText() {
        if(!debugScreenEnabled) return;

        Vector3Int playerPos = GetPlayerPos();
        regionLongText.text = "Region Long: " + RegionPositionHelper.GetRegionPos(playerPos.x >> VoxelProperties.regionBitShift, playerPos.z >> VoxelProperties.regionBitShift);
    }

    private void UpdateRegionPosText() {
        if(!debugScreenEnabled) return;

        Vector3Int playerPos = GetPlayerPos();
        regionPosText.text = "Region Pos: " + (playerPos.x >> VoxelProperties.regionBitShift) + ", " + (playerPos.z >> VoxelProperties.regionBitShift);
    }

    private void UpdateChunkCoordLongText() {
        if(!debugScreenEnabled) return;

        Vector3Int playerPos = GetPlayerPos();
        chunkCoordLongText.text = "Chunk Long: " + ChunkPositionHelper.GetChunkPos(playerPos.x >> VoxelProperties.chunkBitShift, playerPos.z >> VoxelProperties.chunkBitShift);
    }

    private Vector3Int GetPlayerPos() {
        int playerX = Mathf.FloorToInt(playerLocation.position.x);
        int playerY = Mathf.FloorToInt(playerLocation.position.y);
        int playerZ = Mathf.FloorToInt(playerLocation.position.z);

        return new Vector3Int(playerX, playerY, playerZ);
    }
}
