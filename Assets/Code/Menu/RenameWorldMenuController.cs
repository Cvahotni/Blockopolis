using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RenameWorldMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button renameButton;
    [SerializeField] private TMP_Text renameWorldText;
    [SerializeField] private TMP_Text renameWorldTextShadow;

    private MenuController menuController;

    private EditWorldMenuController editWorldMenuController;

    private void Start() {
        menuController = MenuController.Instance;
        editWorldMenuController = EditWorldMenuController.Instance;
    }

    private void FixedUpdate() {
        renameButton.interactable = WorldNameChecker.IsNameValid(nameField.text);
    }

    public void UpdateRenameWorldText() {
        string text = MenuProperties.renameWorldText.Replace("{0}", editWorldMenuController.CurrentWorldName);

        renameWorldText.text = text;
        renameWorldTextShadow.text = text;
    }

    public void Rename() {
        if(editWorldMenuController.CurrentWorldName == "") return;
        if(!WorldNameChecker.IsNameValid(nameField.text)) return;

        WorldHandler.LoadWorld(editWorldMenuController.CurrentWorldName);
        WorldHandler.RenameCurrentWorld(nameField.text);

        editWorldMenuController.CurrentWorldName = nameField.text;
        menuController.ToggleToWorldSelection();
    }
}
