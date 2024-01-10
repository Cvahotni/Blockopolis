using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(5)]
public class MenuEventSystem : MonoBehaviour
{
    public static MenuEventSystem Instance {
        get {
            if(_instance == null) {
                Debug.LogError("The MenuEventSystem must be present in the main scene at all times.");
            }

            return _instance;
        }

        set {
            _instance = value;
        }
    }

    private static MenuEventSystem _instance;

    private MenuController menuController;
    private EditWorldMenuController editWorldMenuController;
    private WorldsEditingToggle worldsEditingToggle;
    private RenameWorldMenuController renameWorldMenuController;
    private WorldsButtonList worldsButtonList;
    private MenuAudioPlayer menuAudioPlayer;

    private UnityEvent worldClickEvent = new UnityEvent();
    private UnityEvent worldSelectEvent = new UnityEvent();
    private UnityEvent deleteEvent = new UnityEvent();
    private UnityEvent renameEvent = new UnityEvent();
    private UnityEvent editEnterEvent = new UnityEvent();
    private UnityEvent<string> editEvent = new UnityEvent<string>();
    private UnityEvent backEvent = new UnityEvent();

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        menuController = MenuController.Instance;
        editWorldMenuController = EditWorldMenuController.Instance;
        worldsEditingToggle = WorldsEditingToggle.Instance;
        renameWorldMenuController = RenameWorldMenuController.Instance;
        worldsButtonList = WorldsButtonList.Instance;
        menuAudioPlayer = MenuAudioPlayer.Instance;

        AddWorldClickListeners();
        AddWorldSelectListeners();
        AddDeleteListeners();
        AddEditListeners();
        AddEditEnterListeners();
        AddRenameListeners();
        AddBackListeners();
    }

    private void AddWorldClickListeners() {
        worldClickEvent.AddListener(menuAudioPlayer.PlayButtonClick);
    }

    private void AddWorldSelectListeners() {
        worldSelectEvent.AddListener(menuController.ToggleOverlay);
    }

    private void AddDeleteListeners() {
        deleteEvent.AddListener(editWorldMenuController.DeleteCurrentWorld);
        deleteEvent.AddListener(menuController.ToggleToWorldSelection);
        deleteEvent.AddListener(worldsEditingToggle.DisableEditMode);
        deleteEvent.AddListener(worldsButtonList.GenerateWorldListings);
    }

    private void AddEditListeners() {
        editEvent.AddListener(editWorldMenuController.SetCurrentWorldName);
        editEvent.AddListener(menuController.ToggleToEditWorld);
        editEvent.AddListener(editWorldMenuController.UpdateEditWorldText);
        editEvent.AddListener(editWorldMenuController.UpdateDeleteWorldText);
        editEvent.AddListener(renameWorldMenuController.UpdateRenameWorldText);
    }

    private void AddEditEnterListeners() {
        editEnterEvent.AddListener(worldsEditingToggle.ToggleEditMode);
    }

    private void AddRenameListeners() {
        renameEvent.AddListener(renameWorldMenuController.Rename);
        renameEvent.AddListener(menuController.ToggleToWorldSelection);
        renameEvent.AddListener(worldsEditingToggle.DisableEditMode);
        renameEvent.AddListener(worldsButtonList.GenerateWorldListings);
    }

    private void AddBackListeners() {
        backEvent.AddListener(worldsEditingToggle.DisableEditMode);
        backEvent.AddListener(menuController.ToggleToWorldSelection);
        backEvent.AddListener(worldsButtonList.GenerateWorldListings);
    }

    public void InvokeWorldClick() {
        worldClickEvent.Invoke();
    }

    public void InvokeWorldSelect() {
        worldSelectEvent.Invoke();
    }

    public void InvokeEdit(string name) {
        editEvent.Invoke(name);
    }

    public void InvokeEnterEdit() {
        editEnterEvent.Invoke();
    }

    public void InvokeDelete() {
        deleteEvent.Invoke();
    }

    public void InvokeRename() {
        renameEvent.Invoke();
    }

    public void InvokeBack() {
        backEvent.Invoke();
    }
}
