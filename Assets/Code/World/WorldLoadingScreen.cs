using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLoadingScreen : MonoBehaviour
{
    public static WorldLoadingScreen Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera backupCamera;
    [SerializeField] private Slider progressBar;

    private WorldEventSystem worldEventSystem;
    private PlayerEventSystem playerEventSystem;

    private bool screenEnabled = true;
    private int amountOfChunksInViewDistance = 65536;
    private int chunksGenerated;

    private bool playerTeleportedToSpawn = false;

    public bool ScreenEnabled {
        get { return screenEnabled; }
    }
    
    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        playerEventSystem = PlayerEventSystem.Instance;
    }

    private void Update() {
        UpdateProgressBar();
        UpdateObjectsStatus();
        UpdatePlayerSpawnStatus();
    }

    private void UpdateObjectsStatus() {
        loadingScreen.SetActive(screenEnabled);
        backupCamera.gameObject.SetActive(screenEnabled);

        playerObject.SetActive(!screenEnabled);
    }

    private void UpdatePlayerSpawnStatus() {
        if(playerTeleportedToSpawn || screenEnabled) return;
        
        playerEventSystem.InvokePlayerSpawn();
        playerTeleportedToSpawn = true;
    }

    private void UpdateProgressBar() {
        progressBar.value = GetProgressAmount();
        
        if(progressBar.value >= 1.0f && screenEnabled) {
            screenEnabled = false;
            worldEventSystem.InvokeCullChunksChange(!screenEnabled);
        }
    }

    private float GetProgressAmount() {
        return (float) chunksGenerated / amountOfChunksInViewDistance;
    }

    public void UpdateChunksGenerated(int amount) {
        chunksGenerated = amount;
    }

    public void UpdateAmountOfChunksInViewDistance(int amount) {
        Debug.Log("Amount changed!");
        amountOfChunksInViewDistance = amount;
    }
}
