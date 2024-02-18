using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SavingScreen : MonoBehaviour
{
    public static SavingScreen Instance { get; private set; }
    [SerializeField] private GameObject savingScreen;
    [SerializeField] private int saveChecksPerSecond = 4;

    private WorldEventSystem worldEventSystem;
    private PlayerEventSystem playerEventSystem;
    private WaitForSeconds savingScreenWaitForSeconds;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        playerEventSystem = PlayerEventSystem.Instance;

        savingScreenWaitForSeconds = new WaitForSeconds(1.0f / saveChecksPerSecond);
    }

    public void ToggleSavingScreen(object sender, EventArgs e) {
        ToggleSavingScreen();
    }

    private void ToggleSavingScreen() {
        savingScreen.SetActive(!savingScreen.activeSelf);
    }

    public void Save(object sender, bool var) {
        StartCoroutine(SaveCheckCoroutine(false));
        ToggleSavingScreen();
    }

    public void ReturnToTitleScreenAfterSaving(object sender, EventArgs e) {
        StartCoroutine(SaveCheckCoroutine(true));
    }

    private IEnumerator SaveCheckCoroutine(bool quit) {
        for(;;) {
            yield return savingScreenWaitForSeconds;

            if(WorldStorage.RegionsSaved >= WorldStorage.RegionMapSize()) {
                WorldStorage.ResetRegionsSaved();

                if(quit) {
                    Debug.Log("Returning to main menu.");
                    SceneManager.LoadScene(sceneName: MenuProperties.menuSceneName);
                }

                else {
                    Debug.Log("Save finished.");
                    ToggleSavingScreen();

                    worldEventSystem.InvokeWorldFinishedLoading();
                    playerEventSystem.InvokePlayerSpawn();
                }
            }
        }
    }
}
