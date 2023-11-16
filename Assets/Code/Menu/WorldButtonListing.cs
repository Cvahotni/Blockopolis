using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WorldButtonListing : MonoBehaviour
{
    [SerializeField] private TMP_Text worldName;
    [SerializeField] private GameObject editIcon;
    
    private string currentName;
    private bool isEditing;

    private MenuEventSystem menuEventSystem;

    public bool IsEditing { 
        set { 
            isEditing = value; 
            editIcon.SetActive(value);
        }
    }

    private void Start() {
        menuEventSystem = MenuEventSystem.Instance;
    }

    public void SetName(string name) {
        worldName.text = name;
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
        menuEventSystem.InvokeEdit(currentName);
    }
}
