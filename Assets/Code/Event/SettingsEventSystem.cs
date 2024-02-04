using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SettingsEventSystem : MonoBehaviour
{
    public static SettingsEventSystem Instance {
        get {
            if(_instance == null) {
                Debug.LogError("The SettingsEventSystem must be present in the main scene at all times.");
            }

            return _instance;
        }

        set {
            _instance = value;
        }
    }

    private static SettingsEventSystem _instance;

    private UnityEvent<int> viewDistanceChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> chunksPerSecondChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> featuresPerSecondChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> chunkBuildsPerFrameChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> maxFramerateChangeEvent = new UnityEvent<int>();
    private UnityEvent<bool> enableVSyncChangeEvent = new UnityEvent<bool>();
    private UnityEvent<bool> fullscreenChangeEvent = new UnityEvent<bool>();
    private UnityEvent<bool> enableShadersChangeEvent = new UnityEvent<bool>();
    private UnityEvent applyChangesEvent = new UnityEvent();

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        AddViewDistanceChangeListeners();
        AddChunksPerSecondChangeListeners();
        AddFeaturesPerSecondChangeListeners();
        AddChunkBuildsPerFrameChangeListener();
        AddMaxFramerateChangeListeners();
        AddEnableVSyncChangeEventListeners();
        AddFullscreenChangeEventListeners();
        AddEnableShadersChangeEventListeners();
        AddApplyChangesListeners();
    }

    private void AddViewDistanceChangeListeners() {
        viewDistanceChangeEvent.AddListener(GameSettings.SetViewDistance);
    }

    private void AddChunksPerSecondChangeListeners() {
        chunksPerSecondChangeEvent.AddListener(GameSettings.SetChunksPerSecond);
    }

    private void AddFeaturesPerSecondChangeListeners() {
        featuresPerSecondChangeEvent.AddListener(GameSettings.SetFeaturesPerSecond);
    }

    private void AddChunkBuildsPerFrameChangeListener() {
        chunkBuildsPerFrameChangeEvent.AddListener(GameSettings.SetChunkBuildsPerFrame);
    }

    private void AddMaxFramerateChangeListeners() {
        maxFramerateChangeEvent.AddListener(GameSettings.SetMaxFramerate);
    }

    private void AddEnableVSyncChangeEventListeners() {
        enableVSyncChangeEvent.AddListener(GameSettings.SetEnableVSync);
    }

    private void AddFullscreenChangeEventListeners() {
        fullscreenChangeEvent.AddListener(GameSettings.SetFullscreen);
    }

    private void AddEnableShadersChangeEventListeners() {
        enableShadersChangeEvent.AddListener(GameSettings.SetEnableShaders);
    }

    private void AddApplyChangesListeners() {
        applyChangesEvent.AddListener(GameSettings.ApplyChangesToUnity);
        applyChangesEvent.AddListener(GameSettingsStorage.Save);
    }

    public void InvokeViewDistanceChange(float amount) {
        viewDistanceChangeEvent.Invoke((int) amount);
    }

    public void InvokeChunksPerSecondChange(float amount) {
        chunksPerSecondChangeEvent.Invoke((int) amount * 50);
    }

    public void InvokeFeaturesPerSecondChange(float amount) {
        featuresPerSecondChangeEvent.Invoke((int) amount * 50);
    }

    public void InvokeChunkBuildsPerFrameChange(float amount) {
        chunkBuildsPerFrameChangeEvent.Invoke((int) amount * 1);
    }

    public void InvokeMaxFramerateChange(float amount) {
        maxFramerateChangeEvent.Invoke((int) amount * 10);
    }

    public void InvokeEnableVSyncChange(bool value) {
        enableVSyncChangeEvent.Invoke(value);
    }

    public void InvokeFullscreenChange(bool value) {
        fullscreenChangeEvent.Invoke(value);
    }

    public void InvokeEnableShadersChange(bool value) {
        enableShadersChangeEvent.Invoke(value);
    }

    public void InvokeApplyChanges() {
        applyChangesEvent.Invoke();
    }
}
