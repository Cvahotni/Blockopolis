using UnityEngine;
using System;

public class PauseMenuToggle : MonoBehaviour
{
    public static PauseMenuToggle Instance { get; private set; }

    private PauseEventSystem pauseEventSystem;
    private InventoryEventSystem inventoryEventSystem;

    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject inventoryScreen;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        pauseEventSystem = PauseEventSystem.Instance;
        inventoryEventSystem = InventoryEventSystem.Instance;
    }

    public void TogglePause(object sender, EventArgs e) {
        if(inventoryScreen.activeSelf) {
            inventoryEventSystem.InvokeInventoryScreenClose();
        }

        else {
            if(pauseScreen.activeSelf || settingsScreen.activeSelf) {
                pauseEventSystem.InvokeUnpause();
            }

            else pauseEventSystem.InvokePause(); 
        }
    }
}
