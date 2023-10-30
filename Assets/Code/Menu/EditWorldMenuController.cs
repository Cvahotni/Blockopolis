using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditWorldMenuController : MonoBehaviour
{
    public static EditWorldMenuController Instance { get; private set; }

    private MenuController menuController;
    private WorldsEditingToggle worldsEditingToggle;

    private string currentWorldName = "";

    public string CurrentWorldName { 
        get { return currentWorldName; }
        set { currentWorldName = value; }
    }

    [SerializeField] private TMP_Text editWorldText;
    [SerializeField] private TMP_Text editWorldTextShadow;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        menuController = MenuController.Instance;
        worldsEditingToggle = WorldsEditingToggle.Instance;
    }

    public void Delete() {
        if(currentWorldName == "") return;

        WorldHandler.LoadWorld(currentWorldName);
        WorldHandler.DeleteCurrentWorld();

        menuController.ToggleToWorldSelection();
    }

    public void UpdateEditWorldText() {
        editWorldText.text = MenuProperties.editWorldTextPrefix + currentWorldName;
        editWorldTextShadow.text = MenuProperties.editWorldTextPrefix + currentWorldName;
    }
}
