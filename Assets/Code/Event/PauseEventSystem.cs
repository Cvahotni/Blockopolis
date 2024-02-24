using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[DefaultExecutionOrder(-250)]
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
    private SavingScreen savingScreen;
    private Inventory inventory;
    private InventoryScreen inventoryScreen;
    private PlayerStorage playerStorage;
    private SettingsScreen settingsScreen;

    private event EventHandler pauseEvent;
    private event EventHandler unpauseEvent;
    private event EventHandler pauseToggleEvent;
    private event EventHandler saveAndQuitEvent;
    private event EventHandler pauseSettingsEnableEvent;
    private event EventHandler pauseSettingsDisableEvent;

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
        savingScreen = SavingScreen.Instance;
        inventory = Inventory.Instance;
        inventoryScreen = InventoryScreen.Instance;
        playerStorage = PlayerStorage.Instance;
        settingsScreen = SettingsScreen.Instance;

        AddPauseListeners();
        AddUnpauseListeners();
        AddPauseToggleListeners();
        AddSaveAndQuitListeners();
        AddPauseSettingsEnableListeners();
        AddPauseSettingsDisableListeners();
    }

    private void AddPauseListeners() {
        pauseEvent += TimeLock.Lock;
        pauseEvent += pauseMenu.DisplayPauseMenu;
        pauseEvent += mouseLook.ReleaseCursor;
        pauseEvent += playerMove.Disable;
        pauseEvent += playerMove.DenyInput;
        pauseEvent += mouseLook.Disable;
        pauseEvent += playerBuild.Disable;
        pauseEvent += hotbar.Disable;
        pauseEvent += inventory.DeactivateInUI;
        pauseEvent += inventoryScreen.Disable;
        pauseEvent += settingsScreen.DisableScreen;
    }

    private void AddUnpauseListeners() {
        unpauseEvent += TimeLock.Unlock;
        unpauseEvent += pauseMenu.HidePauseMenu;
        unpauseEvent += mouseLook.LockCursor;
        unpauseEvent += playerMove.Enable;
        unpauseEvent += playerMove.AllowInput;
        unpauseEvent += mouseLook.Enable;
        unpauseEvent += playerBuild.Enable;
        unpauseEvent += hotbar.Enable;
        unpauseEvent += inventory.DeactivateInUI;
        unpauseEvent += inventoryScreen.Enable;
        unpauseEvent += settingsScreen.DisableScreen;
    }

    private void AddPauseToggleListeners() {
        pauseToggleEvent += pauseMenuToggle.TogglePause;
    }

    private void AddSaveAndQuitListeners() {
        saveAndQuitEvent += TimeLock.Unlock;
        saveAndQuitEvent += playerStorage.SavePlayer;
        saveAndQuitEvent += WorldHandler.SaveCurrentWorldQuickly;
        saveAndQuitEvent += pauseMenu.HidePauseButtons;
        saveAndQuitEvent += settingsScreen.DisableScreen;
        saveAndQuitEvent += savingScreen.ToggleSavingScreen;
        saveAndQuitEvent += savingScreen.ReturnToTitleScreenAfterSaving;
    }

    private void AddPauseSettingsEnableListeners() {
        pauseSettingsEnableEvent += pauseMenu.HidePauseMenu;
        pauseSettingsEnableEvent += settingsScreen.EnableScreen;
    }

    private void AddPauseSettingsDisableListeners() {
        pauseSettingsDisableEvent += pauseMenu.DisplayPauseMenu;
        pauseSettingsDisableEvent += settingsScreen.DisableScreen;
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

    public void InvokePauseSettingsEnable() {
        pauseSettingsEnableEvent.Invoke(this, EventArgs.Empty);
    }

     public void InvokePauseSettingsDisable() {
        pauseSettingsDisableEvent.Invoke(this, EventArgs.Empty);
    }
}
