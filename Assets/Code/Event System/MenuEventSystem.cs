using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MenuEventSystem : MonoBehaviour
{
    public static MenuEventSystem Instance { get; private set; }

    private MenuController menuController;
    private EditWorldMenuController editWorldMenuController;
    private WorldsEditingToggle worldsEditingToggle;
    private RenameWorldMenuController renameWorldMenuController;

    private UnityEvent deleteEvent = new UnityEvent();
    private UnityEvent renameEvent = new UnityEvent();
    private UnityEvent editEnterEvent = new UnityEvent();
    private UnityEvent<string> editEvent = new UnityEvent<string>();
    private UnityEvent backEvent = new UnityEvent();

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        menuController = MenuController.Instance;
        editWorldMenuController = EditWorldMenuController.Instance;
        worldsEditingToggle = WorldsEditingToggle.Instance;
        renameWorldMenuController = RenameWorldMenuController.Instance;

        AddDeleteListeners();
        AddEditListeners();
        AddEditEnterListeners();
        AddRenameListeners();
        AddBackListeners();
    }

    private void AddDeleteListeners() {
        deleteEvent.AddListener(editWorldMenuController.DeleteCurrentWorld);
        deleteEvent.AddListener(menuController.ToggleToWorldSelection);
        deleteEvent.AddListener(worldsEditingToggle.DisableEditMode);
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
    }

    private void AddBackListeners() {
        backEvent.AddListener(menuController.ToggleToWorldSelection);
        backEvent.AddListener(worldsEditingToggle.DisableEditMode);
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
