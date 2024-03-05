using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class EndlessTerrain : MonoBehaviour
{
    public static EndlessTerrain Instance { get; private set; }
    private WorldEventSystem worldEventSystem;

    public Transform playerTransform;
    private List<long> addedChunks = new List<long>();

    private long playerChunkCoord = long.MaxValue;
    private long lastPlayerChunkCoord = long.MaxValue;

    [SerializeField] private List<float> frequencies = new List<float>();
    [SerializeField] private List<float> amplitudes = new List<float>();

    public List<float> Frequencies { get { return frequencies; }}
    public List<float> Amplitudes { get { return amplitudes; }}

    private NativeList<float> nativeFrequencies;
    private NativeList<float> nativeAmplitudes;

    public NativeList<float> NativeFrequencies { get { return nativeFrequencies; }}
    public NativeList<float> NativeAmplitudes { get { return nativeAmplitudes; }}

    private Vector2 noiseOffset;
    public Vector2 NoiseOffset { get { return noiseOffset; }}
    
    private WaitForSeconds shortWait;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        shortWait = new WaitForSeconds(1.0f / GameSettings.ChunksPerSecond);

        if(!WorldHandler.IsCurrentWorldInfoValid()) {
            Debug.LogError("Invalid world detected! " + WorldHandler.CurrentWorld.Name);
            enabled = false;

            return;
        }

        noiseOffset = GetTerrainNoiseOffset();

        BuildNativeArrays();
        MovePlayerToSpawn();
        ViewDistanceChange();
        BuildInitialChunks();
    }

    public void ViewDistanceChange() {
        worldEventSystem.InvokeAmountOfChunksInViewDistanceChange(
            GetAmountOfChunksInViewDistance()
        );
    }

    private void BuildNativeArrays() {
        nativeFrequencies = new NativeList<float>(Allocator.Persistent);
        nativeAmplitudes = new NativeList<float>(Allocator.Persistent);

        foreach(float frequency in frequencies) nativeFrequencies.Add(frequency);
        foreach(float amplitude in amplitudes) nativeAmplitudes.Add(amplitude);
    }

    private void MovePlayerToSpawn() {
        playerTransform.position = WorldSpawner.GetPlayerSpawnLocation();
    }

    public void BuildInitialChunks() {
        lastPlayerChunkCoord = GetChunkCoordFromVector3(playerTransform.position);
        StartCoroutine(GenerateChunksAroundPlayer(GetPlayerChunkX(), GetPlayerChunkZ(), false));
    }

    private void Update() {
        playerChunkCoord = GetChunkCoordFromVector3(playerTransform.position);

        if(playerChunkCoord != lastPlayerChunkCoord) {
            worldEventSystem.InvokeRemoveFeatures(
                new int3(GetPlayerChunkX(), GetPlayerChunkZ(), 
                GameSettings.ViewDistance + VoxelProperties.featureChunkBuffer)
            );

            CheckViewDistance();
        }
    }

    public void CheckViewDistance() {
        long coord = GetChunkCoordFromVector3(playerTransform.position);
        lastPlayerChunkCoord = coord;

        RemoveOutOfRangeChunks(false);
        StartCoroutine(GenerateChunksAroundPlayer(GetPlayerChunkX(), GetPlayerChunkZ(), true));
    }

    public void ForceRemoveOutOfRangeChunks() {
        RemoveOutOfRangeChunks(true);
    }

    public void RemoveOutOfRangeChunks(bool finalize) {
        for(int i = addedChunks.Count - 1; i >= 0; i--) {
            long chunk = addedChunks[i];
            
            if(IsChunkOutOfRange(chunk, 0)) {
                worldEventSystem.InvokeChunkRemove(chunk);
                if(finalize) worldEventSystem.InvokeChunkRemoveFinal(chunk);
            }
        }
    }

    private IEnumerator GenerateChunksAroundPlayer(int originX, int originZ, bool async) {
        worldEventSystem.InvokePlaceFeatures(new int3(originX, originZ, GameSettings.ViewDistance + VoxelProperties.featureChunkBuffer));

        int di = 1;
        int dj = 0;

        int segmentLength = 1;
        int segmentPassed = 0;

        int i = 0;
        int j = 0;

        for(int k = 0; k < GameSettings.ViewDistance * GameSettings.ViewDistance * 4 + (GameSettings.ViewDistance * 4); ++k) {
            long coord = ChunkPositionHelper.GetChunkPos(originX + i, originZ + j);

            bool shouldAddChunk = (
                !WorldAllocator.IsChunkOutsideOfWorld(coord) &&
                !IsChunkInWorld(coord) &&
                !IsChunkOutOfRange(coord, 0)
            );

            if(shouldAddChunk) {
                if(async) yield return shortWait;
                worldEventSystem.InvokeChunkAdd(coord);
            }

            i += di;
            j += dj;

            ++segmentPassed;

            if(segmentPassed == segmentLength) {
                segmentPassed = 0;

                int buffer = di;
                di = -dj;
                dj = buffer;
            
                if(dj == 0) {
                    ++segmentLength;
                }
            }
        }
    }

    private int GetAmountOfChunksInViewDistance() {
        int originX = GetPlayerChunkX();
        int originZ = GetPlayerChunkZ();

        int chunkCount = 0;

        for(int x = -GameSettings.ViewDistance + originX; x < GameSettings.ViewDistance + originX; x++) {
            for(int z = -GameSettings.ViewDistance + originZ; z < GameSettings.ViewDistance + originZ; z++) {
                long coord = ChunkPositionHelper.GetChunkPos(x, z);

                if(WorldAllocator.IsChunkOutsideOfWorld(coord)) continue;
                if(IsChunkOutOfRange(coord, 0)) continue;

                chunkCount++;
            }
        }

        return chunkCount;
    }

    public void AddChunkToAddedChunks(object sender, long coord) {
        addedChunks.Add(coord);
    }

    private bool IsChunkInWorld(long coord) {
        return addedChunks.Contains(coord);
    }

    public void RemoveChunk(object sender, long coord) {
        addedChunks.Remove(coord);
    }

    public bool IsChunkOutOfRange(long coord, int viewDistanceModifier) {
        int worldX = ChunkPositionHelper.GetChunkPosWX(coord);
        int worldZ = ChunkPositionHelper.GetChunkPosWZ(coord);

        worldX = worldX + (VoxelProperties.chunkWidth / 2) - 1;
        worldZ = worldZ + (VoxelProperties.chunkWidth / 2) - 1;

        Vector3 playerTransformPosition = new Vector3(playerTransform.position.x, 0.0f, playerTransform.position.z);
        Vector3 worldPosition = new Vector3(worldX, 0.0f, worldZ);

        return Vector3.Distance(playerTransformPosition, worldPosition) > (GameSettings.ViewDistance + viewDistanceModifier) * VoxelProperties.chunkWidth;
    }

    public bool IsFeatureChunkOutOfRange(long coord) {
        return IsChunkOutOfRange(coord, VoxelProperties.featureChunkBuffer);
    }

    private long GetChunkCoordFromVector3(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x / VoxelProperties.chunkWidth);
        int z = Mathf.FloorToInt(pos.z / VoxelProperties.chunkWidth);

        return ChunkPositionHelper.GetChunkPos(x, z);
    }

    public Vector2 GetTerrainNoiseOffset() {
        UnityEngine.Random.InitState(WorldHandler.CurrentWorld.Seed);
        return new Vector2(UnityEngine.Random.value, UnityEngine.Random.value);
    }

    private int GetPlayerChunkX() {
        return (int) playerTransform.position.x >> VoxelProperties.chunkBitShift;
    }

    private int GetPlayerChunkZ() {
        return (int) playerTransform.position.z >> VoxelProperties.chunkBitShift;
    }

    private void OnDestroy() {
        nativeFrequencies.Dispose();
        nativeAmplitudes.Dispose();
    }
}
