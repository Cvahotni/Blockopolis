using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(EditWorldMenuController))]
public class RenameWorldMenuController : MonoBehaviour
{
    public static RenameWorldMenuController Instance { get; private set; }

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button renameButton;
    [SerializeField] private TMP_Text renameWorldText;

    private EditWorldMenuController editWorldMenuController;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        editWorldMenuController = EditWorldMenuController.Instance;
    }

    private void FixedUpdate() {
        renameButton.interactable = WorldNameChecker.IsNameValid(nameField.text);
    }

    public void UpdateRenameWorldText() {
        string text = MenuProperties.renameWorldText.Replace("{0}", editWorldMenuController.CurrentWorldName);
        renameWorldText.text = text;
    }

    public void Rename() {
        if(editWorldMenuController.CurrentWorldName == "") return;
        if(!WorldNameChecker.IsNameValid(nameField.text)) return;

        WorldHandler.LoadWorld(editWorldMenuController.CurrentWorldName);
        WorldHandler.RenameCurrentWorld(nameField.text);

        editWorldMenuController.CurrentWorldName = nameField.text;
    }
}
