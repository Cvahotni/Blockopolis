using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

[RequireComponent(typeof(EndlessTerrain))]
[RequireComponent(typeof(ChunkObjectPool))]
public class WorldAllocator : MonoBehaviour
{
    public static WorldAllocator Instance { get; private set; }
    
    private Queue<long> chunkQueue = new Queue<long>();
    private Queue<long> immidiateChunkQueue = new Queue<long>();

    private ChunkObjectPool chunkObjectPool;
    private EndlessTerrain endlessTerrain;
    private ChunkBuilder chunkBuilder;

    private int chunksGenerated;
    private bool cullChunksOutOfView = false;

    private World currentWorld;

    public bool CullChunksOutOfView {
        set { cullChunksOutOfView = value; }
    }

    [SerializeField] private Camera playerCamera;

    public int ChunksGenerated {
        get { return chunksGenerated; }
    }

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        chunkObjectPool = ChunkObjectPool.Instance;
        endlessTerrain = EndlessTerrain.Instance;
        chunkBuilder = ChunkBuilder.Instance;

        currentWorld = WorldHandler.CurrentWorld;
    }

    private void Update() {
        BuildNextChunk(ref immidiateChunkQueue);
        BuildNextChunk(ref chunkQueue);

        if(Input.GetKeyDown(KeyCode.H)) WorldHandler.SaveCurrentWorld();
    }

    public void AddChunkToQueue(long coord) {
        chunkQueue.Enqueue(coord);
    }

    public void AddImmidiateChunkToQueue(long coord) {
        immidiateChunkQueue.Enqueue(coord);
    }

    public bool RemoveChunkFromScene(long coord) {
        GameObject gameObject = GameObject.Find("" + coord);
        if(gameObject == null) return false;

        chunkObjectPool.ReturnToPool(gameObject);
        return true;
    }

    private void BuildNextChunk(ref Queue<long> queue) {
        if(queue.Count == 0) return;

        long chunkPos = queue.Dequeue();
        bool shouldCullChunk = !IsChunkInFrustum(chunkPos) && cullChunksOutOfView && !endlessTerrain.IsChunkOutOfRange(chunkPos);

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

        chunkBuilder.BuildChunk(chunkPos);
        chunksGenerated++;
    }

    public bool IsChunkOutsideOfWorld(long coord) {
        return coord == int.MaxValue;
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
        BlockRegistry.OnDestroy();
        WorldStorage.Destroy();
    }
}