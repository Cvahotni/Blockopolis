using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[RequireComponent(typeof(WorldAllocator))]
public class EndlessTerrain : MonoBehaviour
{
    public static EndlessTerrain Instance { get; private set; }

    private WorldAllocator worldAllocator;

    public Transform playerTransform;
    private List<long> addedChunks = new List<long>();

    private long playerChunkCoord = long.MaxValue;
    private long lastPlayerChunkCoord = long.MaxValue;
    
    [SerializeField]
    private int viewDistance = 4;

    [SerializeField]
    private int chunksPerSecond = 256;

    [SerializeField]
    private List<float> frequencies = new List<float>();

    [SerializeField]
    private List<float> amplitudes = new List<float>();

    public List<float> Frequencies { get { return frequencies; }}
    public List<float> Amplitudes { get { return amplitudes; }}
    
    private WaitForSeconds shortWait;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldAllocator = WorldAllocator.Instance;
        shortWait = new WaitForSeconds(1.0f / chunksPerSecond);

        MovePlayerToSpawn();
        BuildInitialChunks();
    }

    private void MovePlayerToSpawn() {
        playerTransform.position = new Vector3(WorldSpawner.SpawnX, VoxelProperties.chunkHeight, WorldSpawner.SpawnZ);
    }

    private void BuildInitialChunks() {
        lastPlayerChunkCoord = GetChunkCoordFromVector3(playerTransform.position);
        StartCoroutine(GenerateChunksAroundPlayer(GetPlayerChunkX(), GetPlayerChunkZ()));
    }

    private void Update() {
        playerChunkCoord = GetChunkCoordFromVector3(playerTransform.position);

        if(playerChunkCoord != lastPlayerChunkCoord) {
            RemoveOutOfRangeChunks();
            CheckViewDistance();
        }
    }

    private void CheckViewDistance() {
        long coord = GetChunkCoordFromVector3(playerTransform.position);
        lastPlayerChunkCoord = coord;

        StartCoroutine(GenerateChunksAroundPlayer(GetPlayerChunkX(), GetPlayerChunkZ()));
    }

    private void RemoveOutOfRangeChunks() {
        for(int i = addedChunks.Count - 1; i >= 0; i--) {
            long chunk = addedChunks[i];
            
            if(IsChunkOutOfRange(chunk)) {
                bool removalResult = worldAllocator.RemoveChunkFromScene(chunk);
                if(removalResult) RemoveChunk(chunk);
            }
        }
    }

    private IEnumerator GenerateChunksAroundPlayer(int originX, int originZ) {
        for(int x = -viewDistance + originX; x < viewDistance + originX; x++) {
            for(int z = -viewDistance + originZ; z < viewDistance + originZ; z++) {
                long coord = ChunkPositionHelper.GetChunkPos(x, z);

                if(worldAllocator.IsChunkOutsideOfWorld(coord)) continue;
                if(IsChunkInWorld(coord)) continue;
                if(IsChunkOutOfRange(coord)) continue;

                worldAllocator.AddChunkToQueue(coord);
                addedChunks.Add(coord);

                yield return shortWait;
            }
        }
    }

    public int GetAmountOfChunksInViewDistance() {
        int originX = GetPlayerChunkX();
        int originZ = GetPlayerChunkZ();

        int chunkCount = 0;

        for(int x = -viewDistance + originX; x < viewDistance + originX; x++) {
            for(int z = -viewDistance + originZ; z < viewDistance + originZ; z++) {
                long coord = ChunkPositionHelper.GetChunkPos(x, z);

                if(worldAllocator.IsChunkOutsideOfWorld(coord)) continue;
                if(IsChunkOutOfRange(coord)) continue;

                chunkCount++;
            }
        }

        return chunkCount;
    }

    private bool IsChunkInWorld(long coord) {
        return addedChunks.Contains(coord);
    }

    public void RemoveChunk(long coord) {
        addedChunks.Remove(coord);
    }

    public bool IsChunkOutOfRange(long coord) {
        int worldX = ChunkPositionHelper.GetChunkPosWX(coord);
        int worldZ = ChunkPositionHelper.GetChunkPosWZ(coord);

        worldX = worldX + (VoxelProperties.chunkWidth / 2) - 1;
        worldZ = worldZ + (VoxelProperties.chunkWidth / 2) - 1;

        Vector3 playerTransformPosition = new Vector3(playerTransform.position.x, 0.0f, playerTransform.position.z);
        Vector3 worldPosition = new Vector3(worldX, 0.0f, worldZ);

        return Vector3.Distance(playerTransformPosition, worldPosition) > viewDistance * VoxelProperties.chunkWidth;
    }

    private long GetChunkCoordFromVector3(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / VoxelProperties.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelProperties.chunkWidth);

        return ChunkPositionHelper.GetChunkPos(x, z);
    }

    public Vector2 GetTerrainNoiseOffset() {
        Random.InitState(WorldHandler.CurrentWorld.Seed);
        return new Vector2(Random.value, Random.value);
    }

    private int GetPlayerChunkX() {
        return (int) playerTransform.position.x >> VoxelProperties.chunkBitShift;
    }

    private int GetPlayerChunkZ() {
        return (int) playerTransform.position.z >> VoxelProperties.chunkBitShift;
    }
}