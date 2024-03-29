using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class WorldButtonListing : MonoBehaviour
{
    [SerializeField] private TMP_Text worldName;
    [SerializeField] private GameObject editIcon;
    [SerializeField] private Button listingButton;
    
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
        menuEventSystem.InvokeWorldClick();

        if(isEditing) EditWorld();
        else SelectWorld();
    }

    private void SelectWorld() {
        menuEventSystem.InvokeWorldSelect();
        bool loadStatus = WorldHandler.LoadWorld(currentName);
        
        if(loadStatus) {
            SceneManager.LoadScene(sceneName: MenuProperties.gameSceneName);
        }

        else {
            Debug.Log("World load attempt failed!");
        }
    }

    private void EditWorld() {
        menuEventSystem.InvokeEdit(currentName);
    }
}
