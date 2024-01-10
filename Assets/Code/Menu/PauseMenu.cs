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

    [SerializeField] private float switchSceneDelay = 0.2f;

    private bool sceneSwitchCountingStatus = false;
    private float sceneSwitchCountdown = 0.1f;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        HidePauseMenu();
    }

    private void Update() {
        if(sceneSwitchCountingStatus) sceneSwitchCountdown += Time.fixedDeltaTime;

        if(sceneSwitchCountdown > switchSceneDelay) {
            sceneSwitchCountdown = 0.0f;
            SceneManager.LoadSceneAsync(sceneName: MenuProperties.menuSceneName);
        }
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
        pauseMenuButtons.SetActive(false);
        sceneSwitchCountingStatus = true;
    }
}
