using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditWorldMenuController : MonoBehaviour
{
    public static EditWorldMenuController Instance { get; private set; }

    private string currentWorldName = "";

    public string CurrentWorldName { 
        get { return currentWorldName; }
        set { currentWorldName = value; }
    }

    [SerializeField] private TMP_Text editWorldText;
    [SerializeField] private TMP_Text deleteWorldText;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void DeleteCurrentWorld() {
        WorldHandler.LoadWorld(currentWorldName);
        WorldHandler.DeleteCurrentWorld();
    }

    public void UpdateEditWorldText(string name) {
        editWorldText.text = MenuProperties.editWorldTextPrefix + currentWorldName;
    }

    public void UpdateDeleteWorldText(string name) {
        string text = MenuProperties.deleteWorldText.Replace("{0}", currentWorldName);
        deleteWorldText.text = text;
    }

    public void SetCurrentWorldName(string name) {
        currentWorldName = name;
    }
}
