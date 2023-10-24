using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldLoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject playerObject;
    [SerializeField] private Camera backupCamera;
    [SerializeField] private Slider progressBar;

    private EndlessTerrain endlessTerrain;
    private WorldAllocator worldAllocator;
    private Hotbar hotbar;

    private bool screenEnabled = true;
    private int amountOfChunksInViewDistance;

    public bool ScreenEnabled {
        get { return screenEnabled; }
    }

    private void Start() {
        endlessTerrain = EndlessTerrain.Instance;
        worldAllocator = WorldAllocator.Instance;
        hotbar = Hotbar.Instance;

        amountOfChunksInViewDistance = endlessTerrain.GetAmountOfChunksInViewDistance();
    }

    private void Update() {
        UpdateProgressBar();
        UpdateObjectsStatus();
    }

    private void UpdateObjectsStatus() {
        loadingScreen.SetActive(screenEnabled);
        backupCamera.gameObject.SetActive(screenEnabled);

        playerObject.SetActive(!screenEnabled);
    }

    private void UpdateProgressBar() {
        progressBar.value = GetProgressAmount();
        
        if(progressBar.value >= 1.0f) {
            screenEnabled = false;

            UpdateWorldAllocator();
            UpdateHotbar();
        }
    }

    private void UpdateWorldAllocator() {
        worldAllocator.CullChunksOutOfView = !screenEnabled;
    }

    private void UpdateHotbar() {
        hotbar.HotbarEnabled = !screenEnabled;
    }    

    private float GetProgressAmount() {
        return (float) worldAllocator.ChunksGenerated / amountOfChunksInViewDistance;
    }
}
