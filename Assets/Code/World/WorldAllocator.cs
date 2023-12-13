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
        BuildNextChunk(ref immidiateChunkQueue);
        BuildNextChunk(ref chunkQueue);
    }

    public void AddChunkToQueue(object sender, long coord) {
        chunkQueue.Enqueue(coord);
    }

    public void AddImmidiateChunkToQueue(long coord) {
        immidiateChunkQueue.Enqueue(coord);
    }

    private void BuildNextChunk(ref Queue<long> queue) {
        if(queue.Count == 0 || !shouldBuildChunks) return;

        long chunkPos = queue.Dequeue();
        bool shouldCullChunk = !IsChunkInFrustum(chunkPos) && cullChunksOutOfView && !endlessTerrain.IsChunkOutOfRange(chunkPos, 0);

        long regionPos = RegionPositionHelper.ChunkPosToRegionPos(chunkPos);

        bool doesRegionExist = WorldStorage.DoesRegionExist(regionPos);
        bool isRegionSaved = WorldStorage.IsRegionSaved(currentWorld, regionPos);
        bool regionIsInvalid = !doesRegionExist && isRegionSaved;

        bool isWaitingForRegion = WorldStorage.IsWaitingForRegion(regionPos);

        bool shouldLoadRegion = !doesRegionExist && !isWaitingForRegion && isRegionSaved;
        bool shouldCreateRegion = !doesRegionExist && !isWaitingForRegion && !isRegionSaved;

        if(shouldLoadRegion) WorldStorage.LoadRegionToMap(currentWorld, regionPos);
        if(shouldCreateRegion) WorldStorage.CreateRegionAt(regionPos);

        if(shouldCullChunk || regionIsInvalid) {
            chunkQueue.Enqueue(chunkPos);
            return;
        }

        worldEventSystem.InvokeChunkBuild(chunkPos);

        chunksGenerated++;
        worldEventSystem.InvokeChunksGeneratedChange(chunksGenerated);
    }

    public static bool IsChunkOutsideOfWorld(long coord) {
        return coord == int.MaxValue;
    }

    public void UpdateCullChunksOutOfView(object sender, bool value) {
        cullChunksOutOfView = value;
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
