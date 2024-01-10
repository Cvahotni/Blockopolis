using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseMenuToggle : MonoBehaviour
{
    public static PauseMenuToggle Instance { get; private set; }
    private PauseEventSystem pauseEventSystem;

    [SerializeField] private GameObject pauseScreen;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseEventSystem = PauseEventSystem.Instance;
    }

    public void TogglePause(object sender, EventArgs e) {
        if(pauseScreen.activeSelf) pauseEventSystem.InvokeUnpause();
        else pauseEventSystem.InvokePause(); 
    }
}
