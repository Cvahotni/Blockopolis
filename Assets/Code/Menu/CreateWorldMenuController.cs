using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class CreateWorldMenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField seedField;
    [SerializeField] private Button createButton;

    private void Start() {
        nameField.characterLimit = WorldStorageProperties.worldNameLimit;
        seedField.characterLimit = WorldStorageProperties.worldSeedLimit;
    }
    
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
        return WorldNameChecker.IsNameValid(nameField.text) && WorldNameChecker.IsSeedValid(seedField.text);
    }
}
