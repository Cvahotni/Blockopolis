using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PauseEventSystem : MonoBehaviour
{
    public static PauseEventSystem Instance {
        get {
            if(_instance == null) {
                Debug.LogError("The PauseEventSystem must be present in the scene at all times.");
            }

            return _instance;
        }

        set {
            _instance = value;
        }
    }

    private static PauseEventSystem _instance;

    private PlayerMove playerMove;
    private MouseLook mouseLook;
    private PlayerBuild playerBuild;
    private Hotbar hotbar;
    private PauseMenu pauseMenu;
    private PauseMenuToggle pauseMenuToggle;
    private PlayerHand playerHand;
    private EndlessTerrain endlessTerrain;

    private event EventHandler pauseEvent;
    private event EventHandler unpauseEvent;
    private event EventHandler pauseToggleEvent;
    private event EventHandler saveAndQuitEvent;

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        playerMove = PlayerMove.Instance;
        mouseLook = MouseLook.Instance;
        playerBuild = PlayerBuild.Instance;
        hotbar = Hotbar.Instance;
        pauseMenu = PauseMenu.Instance;
        pauseMenuToggle = PauseMenuToggle.Instance;
        playerHand = PlayerHand.Instance;
        endlessTerrain = EndlessTerrain.Instance;

        AddPauseListeners();
        AddUnpauseListeners();
        AddPauseToggleListeners();
        AddSaveAndQuitListeners();
    }

    private void AddPauseListeners() {
        pauseEvent += TimeLock.Lock;
        pauseEvent += pauseMenu.DisplayPauseMenu;
        pauseEvent += mouseLook.ReleaseCursor;
        pauseEvent += playerMove.Disable;
        pauseEvent += mouseLook.Disable;
        pauseEvent += playerBuild.Disable;
        pauseEvent += hotbar.Disable;
    }

    private void AddUnpauseListeners() {
        unpauseEvent += TimeLock.Unlock;
        unpauseEvent += pauseMenu.HidePauseMenu;
        unpauseEvent += mouseLook.LockCursor;
        unpauseEvent += playerMove.Enable;
        unpauseEvent += mouseLook.Enable;
        unpauseEvent += playerBuild.Enable;
        unpauseEvent += hotbar.Enable;
    }

    private void AddPauseToggleListeners() {
        pauseToggleEvent += pauseMenuToggle.TogglePause;
    }

    private void AddSaveAndQuitListeners() {
        saveAndQuitEvent += TimeLock.Unlock;
        saveAndQuitEvent += WorldHandler.SaveCurrentWorld;
        saveAndQuitEvent += pauseMenu.ReturnToTitleScreen;
    }

    public void InvokePause() {
        pauseEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokeUnpause() {
        unpauseEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokePauseToggle() {
        pauseToggleEvent.Invoke(this, EventArgs.Empty);
    }

    public void InvokeSaveAndQuit() {
        saveAndQuitEvent.Invoke(this, EventArgs.Empty);
    }
}
