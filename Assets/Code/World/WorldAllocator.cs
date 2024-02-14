using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System;

[RequireComponent(typeof(EndlessTerrain))]
public class WorldAllocator : MonoBehaviour
{
    public static WorldAllocator Instance { get; private set; }
    
    private Queue<long> chunkQueue = new Queue<long>();
    private Queue<long> immidiateChunkQueue = new Queue<long>();

    private WorldEventSystem worldEventSystem;
    private EndlessTerrain endlessTerrain;

    private int chunksGenerated;
    private bool cullChunksOutOfView = false;
    private bool shouldBuildChunks = true;

    private World currentWorld;
    private bool buildChunksQuickly = true;

    [SerializeField] private Camera playerCamera;

    public int ChunksGenerated {
        get { return chunksGenerated; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        endlessTerrain = EndlessTerrain.Instance;

        currentWorld = WorldHandler.CurrentWorld;
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
        bool shouldBuildNextChunk = shouldBuildChunks || immediate;
        if(queue.Count == 0 || !shouldBuildNextChunk) return;

        long chunkPos = queue.Dequeue();
        bool shouldCullChunk = !IsChunkInFrustum(chunkPos) && cullChunksOutOfView && !endlessTerrain.IsChunkOutOfRange(chunkPos, 0);
        
        long regionPos = RegionPositionHelper.ChunkPosToRegionPos(chunkPos);

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
        
        if(shouldCullChunk || skipChunkX || skipChunkZ || skipCurrentChunk && !immediate) {
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

    public void UpdateCullChunksOutOfView(object sender, bool value) {
        cullChunksOutOfView = value;
    }

    public void UpdateBuildChunksQuickly(object sender, bool value) {
        buildChunksQuickly = !value;
    }

    public void DisableChunkBuilding(object sender, int3 data) {
        DisableChunkBuilding();
    }

    public void EnableChunkBuilding(object sender, EventArgs e) {
        EnableChunkBuilding();
    }

    private void DisableChunkBuilding() {
        shouldBuildChunks = false;
    }

    private void EnableChunkBuilding() {
        shouldBuildChunks = true;
    }

    private bool IsChunkInFrustum(long coord) {
        int worldX = ChunkPositionHelper.GetChunkPosWX(coord);
        int worldZ = ChunkPositionHelper.GetChunkPosWZ(coord);

        float boundsCenterX = worldX + (VoxelProperties.chunkWidth / 2);
        float boundsCenterY = VoxelProperties.chunkHeight / 2;
        float boundsCenterZ = worldZ + (VoxelProperties.chunkWidth / 2);
        
        Vector3 boundsCenter = new Vector3(boundsCenterX, boundsCenterY, boundsCenterZ);
        Vector3 boundSize = new Vector3(VoxelProperties.chunkWidth, VoxelProperties.chunkHeight, VoxelProperties.chunkWidth);
    
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        return GeometryUtility.TestPlanesAABB(frustumPlanes, new Bounds(boundsCenter, boundSize));
    }

    private void OnDestroy() {
        WorldStorage.Clear();
    }
}
