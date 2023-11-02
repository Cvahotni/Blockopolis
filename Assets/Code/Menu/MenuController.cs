using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }
    
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private GameObject worldSelectionObject;
    [SerializeField] private GameObject createWorldMenuObject;
    [SerializeField] private GameObject editWorldMenuObject;
    [SerializeField] private GameObject deleteWorldMenuObject;
    [SerializeField] private GameObject renameWorldMenuObject;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        ToggleToMainMenu();
    }

    public void Quit() {
        Application.Quit();
    }

    public void ToggleToMainMenu() {
        ToggleMenuObject(mainMenuObject, true);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
    }

    public void ToggleToWorldSelection() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, true);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
    }

    public void ToggleToCreateWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, true);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
    }

    public void ToggleToEditWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, true);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
    }

    public void ToggleToDeleteWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, true);
        ToggleMenuObject(renameWorldMenuObject, false);
    }

    public void ToggleToRenameWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, true);
    }

    private void ToggleMenuObject(GameObject menuObject, bool value) {
        menuObject.SetActive(value);
    }
}
