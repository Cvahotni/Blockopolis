using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUICheckbox : MonoBehaviour
{
    [SerializeField] private Toggle toggle;
    [SerializeField] private SettingType settingType;

    private void Start() {
        SetCheckboxValue();
    }

    private void SetCheckboxValue() {
        switch(settingType) {
            case SettingType.EnableVSync: {
                toggle.isOn = GameSettings.EnableVSync;
                break;
            }

            case SettingType.Fullscreen: {
                toggle.isOn = GameSettings.Fullscreen;
                break;
            }

            case SettingType.EnableShaders: {
                toggle.isOn = GameSettings.EnableShaders;
                break;
            }

            default: { break; }
        }
    }
}
