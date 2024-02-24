using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryScreen : MonoBehaviour
{
    public static InventoryScreen Instance { get; private set; }

    private InventoryEventSystem inventoryEventSystem;
    private bool inventoryEnabled = false;

    [SerializeField] private GameObject inventoryScreen;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        inventoryEventSystem = InventoryEventSystem.Instance;
    }

    private void Update() {
        if(!inventoryEnabled) return;

        if(Input.GetButtonDown("Inventory")) {
            if(inventoryScreen.activeSelf) {
                inventoryEventSystem.InvokeInventoryScreenClose();
            }

            else inventoryEventSystem.InvokeInventoryScreenOpen();
        }
    }

    public void Enable(object sender, EventArgs e) {
        inventoryEnabled = true;
    }

    public void Disable(object sender, EventArgs e) {
        inventoryEnabled = false;
    }
}
