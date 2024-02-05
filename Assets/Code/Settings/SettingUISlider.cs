using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUISlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float multiplier;
    [SerializeField] private SettingType settingType;

    private void Start() {
        SetSliderValue();
    }

    private void SetSliderValue() {
        switch(settingType) {
            case SettingType.ViewDistance: {
                slider.value = GameSettings.ViewDistance / multiplier;
                break;
            }

            case SettingType.ChunksPerSecond: {
                slider.value = GameSettings.ChunksPerSecond / multiplier;
                break;
            }

            case SettingType.FeaturesPerSecond: {
                slider.value = GameSettings.FeaturesPerSecond / multiplier;
                break;
            }

            case SettingType.MaxFramerate: {
                slider.value = GameSettings.MaxFramerate / multiplier;
                break;
            }

            case SettingType.ChunkBuildsPerFrame: {
                slider.value = GameSettings.ChunkBuildsPerFrame / multiplier;
                break;
            }

            case SettingType.FOV: {
                slider.value = GameSettings.FOV / multiplier;
                break;
            }

            case SettingType.Sensitivity: {
                slider.value = GameSettings.Sensitivity / multiplier;
                break;
            }

            case SettingType.Volume: {
                slider.value = GameSettings.Volume / multiplier;
                break;
            }

            default: { break; }
        }
    }
}
