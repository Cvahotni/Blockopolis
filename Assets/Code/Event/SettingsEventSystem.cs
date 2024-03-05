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

    private WorldFeaturePlacer worldFeaturePlacer;
    private EndlessTerrain endlessTerrain;
    private WorldAllocator worldAllocator;

    private UnityEvent<int> viewDistanceChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> chunksPerSecondChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> featuresPerSecondChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> chunkBuildsPerFrameChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> maxFramerateChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> fovChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> sensitivityChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> volumeChangeEvent = new UnityEvent<int>();
    private UnityEvent<bool> enableVSyncChangeEvent = new UnityEvent<bool>();
    private UnityEvent<bool> fullscreenChangeEvent = new UnityEvent<bool>();
    private UnityEvent<bool> enableShadersChangeEvent = new UnityEvent<bool>();
    private UnityEvent applyGameChangesEvent = new UnityEvent();
    private UnityEvent applyChangesEvent = new UnityEvent();

    private bool registeredInGame = false;

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        AddSettingsListeners();
        AddApplyChangesListeners();
    }

    public void RegisterInGame() {
        if(registeredInGame) return;

        endlessTerrain = EndlessTerrain.Instance;
        worldAllocator = WorldAllocator.Instance;
        worldFeaturePlacer = WorldFeaturePlacer.Instance;

        AddApplyGameChangesListeners();
        registeredInGame = true;
    }

    private void AddSettingsListeners() {
        viewDistanceChangeEvent.AddListener(GameSettings.SetViewDistance);
        chunksPerSecondChangeEvent.AddListener(GameSettings.SetChunksPerSecond);
        featuresPerSecondChangeEvent.AddListener(GameSettings.SetFeaturesPerSecond);
        chunkBuildsPerFrameChangeEvent.AddListener(GameSettings.SetChunkBuildsPerFrame);
        maxFramerateChangeEvent.AddListener(GameSettings.SetMaxFramerate);
        fovChangeEvent.AddListener(GameSettings.SetFOV);
        sensitivityChangeEvent.AddListener(GameSettings.SetSensitivity);
        volumeChangeEvent.AddListener(GameSettings.SetVolume);
        enableVSyncChangeEvent.AddListener(GameSettings.SetEnableVSync);
        fullscreenChangeEvent.AddListener(GameSettings.SetFullscreen);
        enableShadersChangeEvent.AddListener(GameSettings.SetEnableShaders);
    }

    private void AddApplyChangesListeners() {
        applyChangesEvent.AddListener(GameSettings.ApplyChangesToUnity);
        applyChangesEvent.AddListener(GameSettingsStorage.Save);
    }

    private void AddApplyGameChangesListeners() {
        applyGameChangesEvent.AddListener(endlessTerrain.ForceRemoveOutOfRangeChunks);
        applyGameChangesEvent.AddListener(endlessTerrain.ViewDistanceChange);
        applyGameChangesEvent.AddListener(endlessTerrain.BuildInitialChunks);
        applyGameChangesEvent.AddListener(worldAllocator.RemoveOutOfRangeChunks);
        applyGameChangesEvent.AddListener(worldFeaturePlacer.EnableBuildFeaturesQuickly);
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
        chunkBuildsPerFrameChangeEvent.Invoke((int) amount);
    }

    public void InvokeMaxFramerateChange(float amount) {
        maxFramerateChangeEvent.Invoke((int) amount * 10);
    }

    public void InvokeFOVChange(float amount) {
        fovChangeEvent.Invoke((int) amount * 5);
    }

    public void InvokeSensitivityChange(float amount) {
        sensitivityChangeEvent.Invoke((int) amount);
    }

    public void InvokeVolumeChange(float amount) {
        volumeChangeEvent.Invoke((int) amount);
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

    public void InvokeApplyGameChanges() {
        applyGameChangesEvent.Invoke();
    }
}
