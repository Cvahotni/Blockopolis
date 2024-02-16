using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseMenuToggle : MonoBehaviour
{
    public static PauseMenuToggle Instance { get; private set; }
    private PauseEventSystem pauseEventSystem;

    [SerializeField] private GameObject pauseScreen;
    private bool inventoryIsInUI = false;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseEventSystem = PauseEventSystem.Instance;
    }

    public void TogglePause(object sender, EventArgs e) {
        if(inventoryIsInUI) return;

        if(pauseScreen.activeSelf) pauseEventSystem.InvokeUnpause();
        else pauseEventSystem.InvokePause(); 
    }

    public void DisableInUI(object sender, EventArgs e) {
        inventoryIsInUI = false;
    }

    public void EnableInUI(object sender, EventArgs e) {
        inventoryIsInUI = true;
    }
}
