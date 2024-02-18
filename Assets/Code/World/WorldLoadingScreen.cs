using System;
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

    private bool screenEnabled = true;
    private int amountOfChunksInViewDistance = 65536;
    private int chunksGenerated;
    
    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        ToggleActivity();
    }

    private void Update() {
        UpdateProgressBar();
        UpdateObjectsStatus();
    }

    private void UpdateObjectsStatus() {
        loadingScreen.SetActive(screenEnabled);
    }

    private void UpdateProgressBar() {
        progressBar.value = GetProgressAmount();
        
        if(progressBar.value >= 1.0f && screenEnabled) {
            screenEnabled = false;
            worldEventSystem.InvokeLoadingScreenStatus(!screenEnabled);
        }
    }

    private float GetProgressAmount() {
        return (float) chunksGenerated / amountOfChunksInViewDistance;
    }

    public void UpdateChunksGenerated(object sender, int amount) {
        chunksGenerated = amount;
    }

    public void UpdateAmountOfChunksInViewDistance(object sender, int amount) {
        amountOfChunksInViewDistance = amount;
    }

    public void ToggleActivity(object sender, EventArgs e) {
        ToggleActivity();
    }

    private void ToggleActivity() {
        playerObject.SetActive(!screenEnabled);
        backupCamera.gameObject.SetActive(screenEnabled);
    }
}
