using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[RequireComponent(typeof(EndlessTerrain))]
public class WorldAllocator : MonoBehaviour
{
    public static WorldAllocator Instance { get; private set; }
    
    private Queue<long> chunkQueue = new Queue<long>();
    private Queue<long> immidiateChunkQueue = new Queue<long>();

    private WorldEventSystem worldEventSystem;
    private SettingsEventSystem settingsEventSystem;

    private EndlessTerrain endlessTerrain;

    private int chunksGenerated;
    private bool shouldBuildChunks = true;

    private World currentWorld;
    private bool buildChunksQuickly = true;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        settingsEventSystem = SettingsEventSystem.Instance;

        endlessTerrain = EndlessTerrain.Instance;
        currentWorld = WorldHandler.CurrentWorld;

        settingsEventSystem.RegisterInGame();
    }

    private void Update() {
        int chunkBuildsPerUpdate = buildChunksQuickly ? GameSettings.ChunksPerSecondMultiplier : GameSettings.ChunkBuildsPerFrame;

        for(int i = 0; i < chunkBuildsPerUpdate; i++) {
            BuildNextChunk(ref chunkQueue, false);
        }

        BuildNextChunk(ref immidiateChunkQueue, true);
    }

    public void AddChunkToQueue(object sender, long coord) {
        chunkQueue.Enqueue(coord);
    }

    public void AddImmidiateChunkToQueue(long coord) {
        immidiateChunkQueue.Enqueue(coord);
    }

    private void BuildNextChunk(ref Queue<long> queue, bool immediate) {
        if(queue.Count == 0) return;
        long chunkPos = queue.Dequeue();

        long regionPos = RegionPositionHelper.ChunkPosToRegionPos(chunkPos);
        if(endlessTerrain.IsChunkOutOfRange(chunkPos, 2)) return;

        int chunkX = ChunkPositionHelper.GetChunkPosX(chunkPos);
        int chunkZ = ChunkPositionHelper.GetChunkPosZ(chunkPos);

        int relativeChunkX = WorldPositionHelper.GetRelativeChunkX(chunkX);
        int relativeChunkZ = WorldPositionHelper.GetRelativeChunkZ(chunkZ);

        Border.BorderDirection borderDirection = WorldPositionHelper.GetBorderDirection(relativeChunkX, relativeChunkZ, VoxelProperties.regionWidthInChunks);
        Vector2Int axis = Border.Axes[(int) borderDirection];

        int x = WorldPositionHelper.IsChunkOnRegionEdge(relativeChunkX, relativeChunkZ) ? axis.x : 0;
        int z = WorldPositionHelper.IsChunkOnRegionEdge(relativeChunkX, relativeChunkZ) ? axis.y : 0;

        bool skipChunkX = HandleRegionAllocation(RegionPositionHelper.ModifyRegionPos(regionPos, x, z));
        bool skipChunkZ = HandleRegionAllocation(RegionPositionHelper.ModifyRegionPos(regionPos, x, z));
        bool skipCurrentChunk = HandleRegionAllocation(regionPos);
        
        if(skipChunkX || skipChunkZ || skipCurrentChunk && !immediate) {
            queue.Enqueue(chunkPos);
            return;
        }

        worldEventSystem.InvokeChunkBuild(chunkPos);

        chunksGenerated++;
        worldEventSystem.InvokeChunksGeneratedChange(chunksGenerated);
    }

    private bool HandleRegionAllocation(long regionPos) {
        bool doesRegionExist = WorldStorage.DoesRegionExist(regionPos);
        bool isRegionSaved = WorldStorage.IsRegionSaved(currentWorld, regionPos);
        bool regionIsInvalid = !doesRegionExist && isRegionSaved;
        bool isWaitingForRegion = WorldStorage.IsWaitingForLoadRegion(regionPos);

        bool shouldLoadRegion = !doesRegionExist && isRegionSaved && !WorldStorage.IsWaitingForAnyLoadRegion();
        bool shouldCreateRegion = !doesRegionExist && !isRegionSaved;

        if(shouldLoadRegion) WorldStorage.LoadRegionToMap(currentWorld, regionPos);
        if(shouldCreateRegion) WorldStorage.CreateRegionAt(regionPos);

        return regionIsInvalid || isWaitingForRegion;
    }

    public static bool IsChunkOutsideOfWorld(long coord) {
        return coord == int.MaxValue;
    }

    public void RemoveOutOfRangeChunks() {
        for(int i = 0; i < chunkQueue.Count; i++) {
            long coord = chunkQueue.Dequeue();
            
            if(!endlessTerrain.IsChunkOutOfRange(coord, 2)) {
                chunkQueue.Enqueue(coord);
            }
        }
    }

    public void UpdateBuildChunksQuickly(object sender, bool value) {
        buildChunksQuickly = !value;
    }

    private void OnDestroy() {
        WorldStorage.Clear();
    }
}
