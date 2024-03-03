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
    [SerializeField] private GameObject settingsMenuObject;
    [SerializeField] private GameObject creditsMenuObject;
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
        ToggleMenuObject(overlayObject, !overlayObject.activeSelf);
    }

    public void ToggleToMainMenu() {
        ToggleMenuObject(mainMenuObject, true);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToWorldSelection() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, true);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToCreateWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, true);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToEditWorld(string name) {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, true);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToDeleteWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, true);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToRenameWorld() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, true);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToSettings() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, true);
        ToggleMenuObject(creditsMenuObject, false);
    }

    public void ToggleToCredits() {
        ToggleMenuObject(mainMenuObject, false);
        ToggleMenuObject(worldSelectionObject, false);
        ToggleMenuObject(createWorldMenuObject, false);
        ToggleMenuObject(editWorldMenuObject, false);
        ToggleMenuObject(deleteWorldMenuObject, false);
        ToggleMenuObject(renameWorldMenuObject, false);
        ToggleMenuObject(settingsMenuObject, false);
        ToggleMenuObject(creditsMenuObject, true);
    }

    private void ToggleMenuObject(GameObject menuObject, bool value) {
        menuObject.SetActive(value);
    }
}
