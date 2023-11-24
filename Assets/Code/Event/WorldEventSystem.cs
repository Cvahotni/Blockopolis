using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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

    private event EventHandler<long> chunkAddEvent;
    private event EventHandler<long> chunkRemoveEvent;
    private event EventHandler<long> chunkBuildEvent;
    private event EventHandler<BuiltChunkData> chunkObjectBuildEvent;
    private event EventHandler<bool> cullChunksChangeEvent;
    private event EventHandler<int> chunksGeneratedChangeEvent;
    private event EventHandler<int> amountOfChunksInViewDistanceChangeEvent;


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

        AddChunkAddListeners();
        AddChunkRemoveListeners();
        AddChunkBuildListeners();
        AddChunkObjectBuildListeners();
        AddCullChunksChangeListeners();
        AddChunksGeneratedChangeListeners();
        AddAmountOfChunksInViewDistanceChangeListeners();
    }

    private void AddChunkAddListeners() {
        chunkAddEvent += worldAllocator.AddChunkToQueue;
        chunkAddEvent += endlessTerrain.AddChunkToAddedChunks;
    }

    private void AddChunkRemoveListeners() {
        chunkRemoveEvent += chunkObjectPool.ReturnToPool;
        chunkRemoveEvent += endlessTerrain.RemoveChunk;
    }

    private void AddChunkBuildListeners() {
        chunkBuildEvent += chunkBuilder.BuildChunk;
    }

    private void AddChunkObjectBuildListeners() {
        chunkObjectBuildEvent += chunkObjectPool.ReturnToPool;
        chunkObjectBuildEvent += chunkObjectBuilder.BuildChunkObject;
    }

    private void AddCullChunksChangeListeners() {
        cullChunksChangeEvent += worldAllocator.UpdateCullChunksOutOfView;
        cullChunksChangeEvent += hotbar.SetStatus;
    }

    private void AddChunksGeneratedChangeListeners() {
        chunksGeneratedChangeEvent += worldLoadingScreen.UpdateChunksGenerated;
    }

    private void AddAmountOfChunksInViewDistanceChangeListeners() {
        amountOfChunksInViewDistanceChangeEvent += worldLoadingScreen.UpdateAmountOfChunksInViewDistance;
    }

    public void InvokeChunkAdd(long coord) {
        chunkAddEvent.Invoke(this, coord);
    }

    public void InvokeChunkRemove(long coord) {
        chunkRemoveEvent.Invoke(this, coord);
    }

    public void InvokeChunkBuild(long coord) {
        chunkBuildEvent.Invoke(this, coord);
    }

    public void InvokeChunkObjectBuild(BuiltChunkData data) {
        chunkObjectBuildEvent.Invoke(this, data);
    }

    public void InvokeCullChunksChange(bool value) {
        cullChunksChangeEvent.Invoke(this, value);
    }

    public void InvokeChunksGeneratedChange(int amount) {
        chunksGeneratedChangeEvent.Invoke(this, amount);
    }

    public void InvokeAmountOfChunksInViewDistanceChange(int amount) {
        amountOfChunksInViewDistanceChangeEvent.Invoke(this, amount);
    }
}
