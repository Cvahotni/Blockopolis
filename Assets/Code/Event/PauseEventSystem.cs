using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
    private PlayerHand playerHand;
    private EndlessTerrain endlessTerrain;

    private UnityEvent pauseEvent = new UnityEvent();
    private UnityEvent unpauseEvent = new UnityEvent();
    private UnityEvent saveAndQuitEvent = new UnityEvent();

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
        playerHand = PlayerHand.Instance;
        endlessTerrain = EndlessTerrain.Instance;

        AddPauseListeners();
        AddUnpauseListeners();
        AddSaveAndQuitListeners();
    }

    private void AddPauseListeners() {
        pauseEvent.AddListener(TimeLock.Lock);
        pauseEvent.AddListener(pauseMenu.DisplayPauseMenu);
        pauseEvent.AddListener(mouseLook.ReleaseCursor);
        pauseEvent.AddListener(playerMove.Disable);
        pauseEvent.AddListener(mouseLook.Disable);
        pauseEvent.AddListener(playerBuild.Disable);
        pauseEvent.AddListener(hotbar.Disable);
    }

    private void AddUnpauseListeners() {
        unpauseEvent.AddListener(TimeLock.Unlock);
        unpauseEvent.AddListener(pauseMenu.HidePauseMenu);
        unpauseEvent.AddListener(mouseLook.LockCursor);
        unpauseEvent.AddListener(playerMove.Enable);
        unpauseEvent.AddListener(mouseLook.Enable);
        unpauseEvent.AddListener(playerBuild.Enable);
        unpauseEvent.AddListener(hotbar.Enable);
    }

    private void AddSaveAndQuitListeners() {
        saveAndQuitEvent.AddListener(WorldHandler.SaveCurrentWorld);
        saveAndQuitEvent.AddListener(TimeLock.Unlock);
        saveAndQuitEvent.AddListener(pauseMenu.ReturnToTitleScreen);
    }

    public void InvokePause() {
        pauseEvent.Invoke();
    }

    public void InvokeUnpause() {
        unpauseEvent.Invoke();
    }

    public void InvokeSaveAndQuit() {
        saveAndQuitEvent.Invoke();
    }
}
