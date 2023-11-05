using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WorldEventSystem : MonoBehaviour
{
    public static WorldEventSystem Instance { get; private set; }

    private EndlessTerrain endlessTerrain;
    private WorldAllocator worldAllocator;
    private WorldLoadingScreen worldLoadingScreen;
    private Hotbar hotbar;
    private ChunkBuilder chunkBuilder;
    private ChunkObjectPool chunkObjectPool;
    private ChunkObjectBuilder chunkObjectBuilder;

    private UnityEvent<long> chunkAddEvent = new UnityEvent<long>();
    private UnityEvent<long> chunkRemoveEvent = new UnityEvent<long>();
    private UnityEvent<long> chunkBuildEvent = new UnityEvent<long>();
    private UnityEvent<BuiltChunkData> chunkObjectBuildEvent = new UnityEvent<BuiltChunkData>();
    private UnityEvent<bool> cullChunksChangeEvent = new UnityEvent<bool>();
    private UnityEvent<int> chunksGeneratedChangeEvent = new UnityEvent<int>();
    private UnityEvent<int> amountOfChunksInViewDistanceChangeEvent = new UnityEvent<int>();


    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
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
        chunkAddEvent.AddListener(worldAllocator.AddChunkToQueue);
        chunkAddEvent.AddListener(endlessTerrain.AddChunkToAddedChunks);
    }

    private void AddChunkRemoveListeners() {
        chunkRemoveEvent.AddListener(chunkObjectPool.ReturnToPool);
        chunkRemoveEvent.AddListener(endlessTerrain.RemoveChunk);
    }

    private void AddChunkBuildListeners() {
        chunkBuildEvent.AddListener(chunkBuilder.BuildChunk);
    }

    private void AddChunkObjectBuildListeners() {
        chunkObjectBuildEvent.AddListener(chunkObjectPool.ReturnToPool);
        chunkObjectBuildEvent.AddListener(chunkObjectBuilder.BuildChunkObject);
    }

    private void AddCullChunksChangeListeners() {
        cullChunksChangeEvent.AddListener(worldAllocator.UpdateCullChunksOutOfView);
        cullChunksChangeEvent.AddListener(hotbar.SetStatus);
    }

    private void AddChunksGeneratedChangeListeners() {
        chunksGeneratedChangeEvent.AddListener(worldLoadingScreen.UpdateChunksGenerated);
    }

    private void AddAmountOfChunksInViewDistanceChangeListeners() {
        amountOfChunksInViewDistanceChangeEvent.AddListener(worldLoadingScreen.UpdateAmountOfChunksInViewDistance);
    }

    public void InvokeChunkAdd(long coord) {
        chunkAddEvent.Invoke(coord);
    }

    public void InvokeChunkRemove(long coord) {
        chunkRemoveEvent.Invoke(coord);
    }

    public void InvokeChunkBuild(long coord) {
        chunkBuildEvent.Invoke(coord);
    }

    public void InvokeChunkObjectBuild(BuiltChunkData data) {
        chunkObjectBuildEvent.Invoke(data);
    }

    public void InvokeCullChunksChange(bool value) {
        cullChunksChangeEvent.Invoke(value);
    }

    public void InvokeChunksGeneratedChange(int amount) {
        chunksGeneratedChangeEvent.Invoke(amount);
    }

    public void InvokeAmountOfChunksInViewDistanceChange(int amount) {
        Debug.Log("Invoked!");
        amountOfChunksInViewDistanceChangeEvent.Invoke(amount);
    }
}
