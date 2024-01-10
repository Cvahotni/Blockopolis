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

    [SerializeField] private Transform playerLocation;
    [SerializeField] private float updatesPerSecond;
    [SerializeField] private GameObject debugScreenObject;

    private StringBuilder fpsTextBuilder = new StringBuilder(100);
    private StringBuilder positionTextBuilder = new StringBuilder(100);
    private StringBuilder chunkCoordTextBuilder = new StringBuilder(100);
    private StringBuilder chunkCoordLongTextBuilder = new StringBuilder(100);

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
        }
    }

    private void UpdateFPSText() {
        if(!debugScreenEnabled) return;
        fpsTextBuilder.Clear();

        fpsTextBuilder.Append(Mathf.FloorToInt(1.0f / Time.deltaTime));
        fpsTextBuilder.Append(" FPS");

        fpsText.text = fpsTextBuilder.ToString();
    }

    private void UpdatePositionText() {
        if(!debugScreenEnabled) return;
        
        Vector3Int playerPos = GetPlayerPos();
        positionTextBuilder.Clear();

        positionTextBuilder.Append("Position: ");
        positionTextBuilder.Append(playerPos.x);
        positionTextBuilder.Append(", ");
        positionTextBuilder.Append(playerPos.y);
        positionTextBuilder.Append(", ");
        positionTextBuilder.Append(playerPos.z);

        positionText.text = positionTextBuilder.ToString();
    }

    private void UpdateChunkCoordText() {
        if(!debugScreenEnabled) return;

        Vector3Int playerPos = GetPlayerPos();
        chunkCoordTextBuilder.Clear();

        chunkCoordTextBuilder.Append("Chunk Coord: ");
        chunkCoordTextBuilder.Append(playerPos.x >> VoxelProperties.chunkBitShift);
        chunkCoordTextBuilder.Append(", ");
        chunkCoordTextBuilder.Append(playerPos.z >> VoxelProperties.chunkBitShift);

        chunkCoordText.text = chunkCoordTextBuilder.ToString();
    }

    private void UpdateChunkCoordLongText() {
        if(!debugScreenEnabled) return;

        Vector3Int playerPos = GetPlayerPos();
        chunkCoordLongTextBuilder.Clear();

        chunkCoordLongTextBuilder.Append("Chunk Long: ");
        chunkCoordLongTextBuilder.Append(ChunkPositionHelper.GetChunkPos(playerPos.x >> VoxelProperties.chunkBitShift, playerPos.z >> VoxelProperties.chunkBitShift));

        chunkCoordLongText.text = chunkCoordLongTextBuilder.ToString();
    }

    private Vector3Int GetPlayerPos() {
        int playerX = Mathf.FloorToInt(playerLocation.position.x);
        int playerY = Mathf.FloorToInt(playerLocation.position.y);
        int playerZ = Mathf.FloorToInt(playerLocation.position.z);

        return new Vector3Int(playerX, playerY, playerZ);
    }
}
