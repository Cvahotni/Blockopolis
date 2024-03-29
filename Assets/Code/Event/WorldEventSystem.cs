using UnityEngine;
using System;

[DefaultExecutionOrder(-500)]
[RequireComponent(typeof(PlayerEventSystem))]
[RequireComponent(typeof(InventoryEventSystem))]
public class WorldEventSystem : MonoBehaviour
{
    public static WorldEventSystem Instance {
        get {
            if(_instance == null) {
                Debug.LogError("The WorldEventSystem must be present in the scene at all times.");
            }

            return _instance;
        }

        set {
            _instance = value;
        }
    }

    private static WorldEventSystem _instance;

    private EndlessTerrain endlessTerrain;
    private WorldAllocator worldAllocator;
    private WorldLoadingScreen worldLoadingScreen;
    private Hotbar hotbar;
    private ChunkBuilder chunkBuilder;
    private ChunkObjectPool chunkObjectPool;
    private ChunkObjectBuilder chunkObjectBuilder;
    private WorldFeaturePlacer worldFeaturePlacer;
    private SavingScreen savingScreen;
    private PlayerStorage playerStorage;
    private InventoryScreen inventoryScreen;
    private MouseLook mouseLook;

    private event EventHandler<long> chunkAddEvent;
    private event EventHandler<long> chunkRemoveEvent;
    private event EventHandler<long> chunkRemoveFinalEvent;
    private event EventHandler<long> chunkBuildEvent;

    private event EventHandler<BuiltChunkData> chunkObjectBuildEvent;
    private event EventHandler<bool> loadingScreenStatusEvent;
    private event EventHandler<int> chunksGeneratedChangeEvent;
    private event EventHandler<int> amountOfChunksInViewDistanceChangeEvent;
    private event EventHandler worldFinishedLoadingEvent;

    private void Awake() {
        if(_instance != null && _instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        endlessTerrain = EndlessTerrain.Instance;
        worldAllocator = WorldAllocator.Instance;
        worldLoadingScreen = WorldLoadingScreen.Instance;
        hotbar = Hotbar.Instance;
        chunkBuilder = ChunkBuilder.Instance;
        chunkObjectPool = ChunkObjectPool.Instance;
        chunkObjectBuilder = ChunkObjectBuilder.Instance;
        worldFeaturePlacer = WorldFeaturePlacer.Instance;
        savingScreen = SavingScreen.Instance;
        playerStorage = PlayerStorage.Instance;
        inventoryScreen = InventoryScreen.Instance;
        mouseLook = MouseLook.Instance;

        AddChunkAddListeners();
        AddChunkRemoveListeners();
        AddChunkRemoveFinalListeners();
        AddChunkBuildListeners();
        AddChunkObjectBuildListeners();
        AddLoadingScreenStatusListeners();
        AddChunksGeneratedChangeListeners();
        AddAmountOfChunksInViewDistanceChangeListeners();
        AddWorldFinishedLoadingListeners();
    }

    private void AddChunkAddListeners() {
        chunkAddEvent += worldAllocator.AddChunkToQueue;
        chunkAddEvent += endlessTerrain.AddChunkToAddedChunks;
    }

    private void AddChunkRemoveListeners() {
        chunkRemoveEvent += chunkObjectPool.ReturnToPool;
    }

    private void AddChunkRemoveFinalListeners() {
        chunkRemoveFinalEvent += endlessTerrain.RemoveChunk;
    }

    private void AddChunkBuildListeners() {
        chunkBuildEvent += worldFeaturePlacer.PlaceFeatures;
        chunkBuildEvent += chunkBuilder.BuildChunk;
    }

    private void AddChunkObjectBuildListeners() {
        chunkObjectBuildEvent += chunkObjectBuilder.BuildChunkObject;
    }

    private void AddLoadingScreenStatusListeners() {
        loadingScreenStatusEvent += WorldHandler.SaveCurrentWorldQuickly;
        loadingScreenStatusEvent += worldAllocator.UpdateBuildChunksQuickly;
        loadingScreenStatusEvent += savingScreen.Save;
    }

    private void AddChunksGeneratedChangeListeners() {
        chunksGeneratedChangeEvent += worldLoadingScreen.UpdateChunksGenerated;
    }

    private void AddAmountOfChunksInViewDistanceChangeListeners() {
        amountOfChunksInViewDistanceChangeEvent += worldLoadingScreen.UpdateAmountOfChunksInViewDistance;
    }

    private void AddWorldFinishedLoadingListeners() {
        worldFinishedLoadingEvent += hotbar.EnableInput;
        worldFinishedLoadingEvent += inventoryScreen.Enable;
        worldFinishedLoadingEvent += worldLoadingScreen.ToggleActivity;
        worldFinishedLoadingEvent += playerStorage.LoadPlayerRotation;
        worldFinishedLoadingEvent += mouseLook.SetupRotation;
        worldFinishedLoadingEvent += worldAllocator.EnableCullChunksOutOfView;
    }

    public void InvokeChunkAdd(long coord) {
        chunkAddEvent.Invoke(this, coord);
    }

    public void InvokeChunkRemove(long coord) {
        chunkRemoveEvent.Invoke(this, coord);
    }

    public void InvokeChunkRemoveFinal(long coord) {
        chunkRemoveFinalEvent.Invoke(this, coord);
    }

    public void InvokeChunkBuild(long coord) {
        chunkBuildEvent.Invoke(this, coord);
    }

    public void InvokeChunkObjectBuild(BuiltChunkData data) {
        chunkObjectBuildEvent.Invoke(this, data);
    }

    public void InvokeLoadingScreenStatus(bool value) {
        loadingScreenStatusEvent.Invoke(this, value);
    }

    public void InvokeChunksGeneratedChange(int amount) {
        chunksGeneratedChangeEvent.Invoke(this, amount);
    }

    public void InvokeAmountOfChunksInViewDistanceChange(int amount) {
        amountOfChunksInViewDistanceChangeEvent.Invoke(this, amount);
    }

    public void InvokeWorldFinishedLoading() {
        worldFinishedLoadingEvent.Invoke(this, EventArgs.Empty);
    }
}
