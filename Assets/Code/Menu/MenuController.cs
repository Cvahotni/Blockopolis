using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }

    [SerializeField] private GameObject[] mainMenuObjects;
    [SerializeField] private GameObject[] worldSelectionObjects;
    [SerializeField] private GameObject[] createWorldMenuObjects;
    [SerializeField] private GameObject[] editWorldMenuObjects;
    [SerializeField] private GameObject[] deleteWorldMenuObjects;
    [SerializeField] private GameObject[] renameWorldMenuObjects;
    [SerializeField] private GameObject[] settingsMenuObjects;
    [SerializeField] private GameObject[] creditsMenuObjects;
    [SerializeField] private GameObject overlayObject;

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

    public void ToggleOverlay() {
        ToggleMenuObjects(new GameObject[] { overlayObject }, !overlayObject.activeSelf);
    }

    public void ToggleToMainMenu() {
        ToggleMenuObjects(mainMenuObjects, true);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToWorldSelection() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, true);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToCreateWorld() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, true);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToEditWorld(string name) {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, true);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToDeleteWorld() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, true);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToRenameWorld() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, true);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToSettings() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, true);
        ToggleMenuObjects(creditsMenuObjects, false);
    }

    public void ToggleToCredits() {
        ToggleMenuObjects(mainMenuObjects, false);
        ToggleMenuObjects(worldSelectionObjects, false);
        ToggleMenuObjects(createWorldMenuObjects, false);
        ToggleMenuObjects(editWorldMenuObjects, false);
        ToggleMenuObjects(deleteWorldMenuObjects, false);
        ToggleMenuObjects(renameWorldMenuObjects, false);
        ToggleMenuObjects(settingsMenuObjects, false);
        ToggleMenuObjects(creditsMenuObjects, true);
    }

    private void ToggleMenuObjects(GameObject[] menuObjects, bool value) {
        foreach(GameObject menuObject in menuObjects) {
            menuObject.SetActive(value);
        }
    }
}
