using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeleteWorldMenuController : MonoBehaviour
{
    [SerializeField] private TMP_Text deleteWorldText;

    private EditWorldMenuController editWorldMenuController;

    private void OnEnable() {
        editWorldMenuController = EditWorldMenuController.Instance;
        SetText();
    }

    private void SetText() {
        string text = MenuProperties.deleteWorldText.Replace("{0}", editWorldMenuController.CurrentWorldName);
        deleteWorldText.text = text;
    }
}
