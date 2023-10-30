using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class CreateWorldMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField seedField;
    [SerializeField] private Button createButton;
    
    private void FixedUpdate() {
        createButton.interactable = CanCreate();
    }

    public void Create() {
        if(!CanCreate()) return;

        string worldName = nameField.text;
        string worldSeed = seedField.text;

        WorldHandler.CreateNewWorld(worldName, worldSeed);
        SceneManager.LoadScene(sceneName: MenuProperties.gameSceneName);
    }

    private bool CanCreate() {
        string worldName = nameField.text;
        string worldSeed = seedField.text;

        Regex regex = new Regex("^[a-zA-Z0-9_]+( [a-zA-Z0-9_]+)*$");

        bool nameValidCharacters = regex.IsMatch(worldName);
        bool seedValidCharacters = regex.IsMatch(worldSeed);

        return nameValidCharacters && seedValidCharacters;
    }
}
