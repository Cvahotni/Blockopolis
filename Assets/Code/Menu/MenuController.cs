using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }
    
    [SerializeField] private GameObject[] mainMenuObjects;
    [SerializeField] private GameObject[] worldSelectionObjects;
    [SerializeField] private GameObject[] createWorldMenuObjects;
    [SerializeField] private GameObject[] editWorldMenuObjects;

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
        ToggleMenuObjects(mainMenuObjects, true);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
    }

    public void ToggleToWorldSelection() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, true);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
    }

    public void ToggleToCreateWorld() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, true);
        ToggleMenuObjects(editWorldMenuObjects, false);
    }

    public void ToggleToEditWorld() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, true);
    }

    private void ToggleMenuObjects(GameObject[] menuObjects, bool value) {
        foreach(GameObject currentObject in menuObjects) {
            currentObject.SetActive(value);
        }
    }
}
