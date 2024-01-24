using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SavingScreen : MonoBehaviour
{
    public static SavingScreen Instance { get; private set; }
    [SerializeField] private GameObject savingScreen;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void ToggleSavingScreen(object sender, EventArgs e) {
        savingScreen.SetActive(!savingScreen.activeSelf);
    }
}
