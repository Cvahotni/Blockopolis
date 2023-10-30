using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldListing : MonoBehaviour
{
    [SerializeField] private TMP_Text worldName;
    [SerializeField] private TMP_Text worldNameShadow;
    [SerializeField] private GameObject editIcon;
    
    private string currentName;
    private bool isEditing;

    private MenuController menuController;
    private EditWorldMenuController editWorldMenuController;

    public bool IsEditing { set { isEditing = value; }}

    private void Start() {
        menuController = MenuController.Instance;
        editWorldMenuController = EditWorldMenuController.Instance;
    }

    public void SetName(string name) {
        worldName.text = name;
        worldNameShadow.text = name;

        currentName = name;
    }

    public void Select() {
        if(isEditing) EditWorld();
        else SelectWorld();
    }

    private void SelectWorld() {
        WorldHandler.LoadWorld(currentName);
        SceneManager.LoadScene(sceneName: MenuProperties.gameSceneName);
    }

    private void EditWorld() {
        editWorldMenuController.CurrentWorldName = currentName;
        editWorldMenuController.UpdateEditWorldText();

        menuController.ToggleToEditWorld();
    }

    private void FixedUpdate() {
        editIcon.SetActive(isEditing);
    }
}
