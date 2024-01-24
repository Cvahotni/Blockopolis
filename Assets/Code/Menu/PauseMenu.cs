using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuButtons;

    [SerializeField] private int saveChecksPerSecond = 4;
    private WaitForSeconds pauseWaitForSeconds;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseWaitForSeconds = new WaitForSeconds(1.0f / saveChecksPerSecond);
        HidePauseMenu();
    }

    public void DisplayPauseMenu(object sender, EventArgs e) {
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu(object sender, EventArgs e) {
        HidePauseMenu();
    }

    public void HidePauseMenu() {
        pauseMenu.SetActive(false);
    }

    public void ReturnToTitleScreen(object sender, EventArgs e) {
        StartCoroutine(ReturnToTitleScreenCoroutine());
    }

    private IEnumerator ReturnToTitleScreenCoroutine() {
        for(;;) {
            yield return pauseWaitForSeconds;

            if(WorldStorage.RegionsSaved >= WorldStorage.RegionMapSize()) {
                WorldStorage.ResetRegionsSaved();
                SceneManager.LoadScene(sceneName: MenuProperties.menuSceneName);
            }
        }
    }

    public void HidePauseButtons(object sender, EventArgs e) {
        pauseMenuButtons.SetActive(false);
    }
}
