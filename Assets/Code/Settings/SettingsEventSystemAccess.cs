using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsEventSystemAccess : MonoBehaviour
{
    public void InvokeViewDistanceChange(float amount) {
        SettingsEventSystem.Instance.InvokeViewDistanceChange((int) amount);
    }

    public void InvokeChunksPerSecondChange(float amount) {
        SettingsEventSystem.Instance.InvokeChunksPerSecondChange((int) amount);
    }

    public void InvokeFeaturesPerSecondChange(float amount) {
        SettingsEventSystem.Instance.InvokeFeaturesPerSecondChange((int) amount);
    }

    public void InvokeChunkBuildsPerFrameChange(float amount) {
        SettingsEventSystem.Instance.InvokeChunkBuildsPerFrameChange((int) amount);
    }

    public void InvokeMaxFramerateChange(float amount) {
        SettingsEventSystem.Instance.InvokeMaxFramerateChange((int) amount);
    }

    public void InvokeFOVChange(float amount) {
        SettingsEventSystem.Instance.InvokeFOVChange((int) amount);
    }

    public void InvokeSensitivityChange(float amount) {
        SettingsEventSystem.Instance.InvokeSensitivityChange((int) amount);
    }

    public void InvokeVolumeChange(float amount) {
        SettingsEventSystem.Instance.InvokeVolumeChange((int) amount);
    }

    public void InvokeEnableVSyncChange(bool value) {
        SettingsEventSystem.Instance.InvokeEnableVSyncChange(value);
    }

    public void InvokeFullscreenChange(bool value) {
        SettingsEventSystem.Instance.InvokeFullscreenChange(value);
    }

    public void InvokeEnableShadersChange(bool value) {
        SettingsEventSystem.Instance.InvokeEnableShadersChange(value);
    }

    public void InvokeApplyChanges() {
        SettingsEventSystem.Instance.InvokeApplyChanges();
    }

    public void InvokeApplyGameChanges() {
        SettingsEventSystem.Instance.InvokeApplyGameChanges();
    }
}
