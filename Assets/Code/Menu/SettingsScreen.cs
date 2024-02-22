using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SettingsScreen : MonoBehaviour
{
    public static SettingsScreen Instance { get; private set; }
    [SerializeField] private GameObject settingsScreen;

    private PauseEventSystem pauseEventSystem;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseEventSystem = PauseEventSystem.Instance;
    }

    public void EnableScreen(object sender, EventArgs e) {
        settingsScreen.SetActive(true);
    }

    public void DisableScreen(object sender, EventArgs e) {
        settingsScreen.SetActive(false);
    }
}
