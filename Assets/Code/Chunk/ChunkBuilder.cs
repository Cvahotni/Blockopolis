using System.Collections;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using System;

[RequireComponent(typeof(EndlessTerrain))]
[RequireComponent(typeof(WorldFeatures))]
public class ChunkBuilder : MonoBehaviour
{
    public static ChunkBuilder Instance { get; private set; }

    private readonly ProfilerMarker buildChunkNoiseMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkNoiseMap3D");
    private readonly ProfilerMarker buildChunkVoxelMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkVoxelMap");
    private readonly ProfilerMarker buildChunkMeshMarker = new ProfilerMarker("ChunkBuilder.BuildChunkMesh");
    private readonly ProfilerMarker placeChunkFeaturesMarker = new ProfilerMarker("ChunkBuilder.PlaceChunkFeatures");
    private readonly ProfilerMarker placeChunkDecorationsMarker = new ProfilerMarker("ChunkBuilder.PlaceChunkDecorations");

    private WorldEventSystem worldEventSystem;
    private EndlessTerrain endlessTerrain;
    private WorldFeatures worldFeatures;
    private NativeParallelHashMap<FeaturePlacement, ushort> featureData = FeatureRegistry.FeatureData;
    private NativeParallelHashMap<ushort, FeatureSettings> featureSettings = FeatureRegistry.FeatureSettings;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        if(!WorldHandler.IsCurrentWorldInfoValid()) {
            Debug.LogError("ChunkBuilder can't find a valid world to use.");
            enabled = false;

            return;
        }

        worldEventSystem = WorldEventSystem.Instance;
        endlessTerrain = EndlessTerrain.Instance;
        worldFeatures = WorldFeatures.Instance;
    }

    [Obsolete]
    public void BuildChunk(object sender, long chunkCoord) {
        BuildChunk(chunkCoord);
    }

    [Obsolete]
    private void BuildChunk(long chunkCoord) {
        NativeArray<long> chunkPos = new NativeArray<long>(1, Allocator.Persistent);

        NativeList<ChunkVertex> vertices = new NativeList<ChunkVertex>(Allocator.Persistent);
        NativeList<uint> indices = new NativeList<uint>(Allocator.Persistent);
        NativeList<uint> transparentIndices = new NativeList<uint>(Allocator.Persistent);
        NativeList<uint> cutoutIndices = new NativeList<uint>(Allocator.Persistent);

        NativeArray<ushort> voxelMap = GetVoxelMap(chunkCoord);
        NativeArray<ushort> forwardVoxelMap = GetVoxelMapWithOffset(chunkCoord, 0, 1);
        NativeArray<ushort> backVoxelMap = GetVoxelMapWithOffset(chunkCoord, 0, -1);
        NativeArray<ushort> rightVoxelMap = GetVoxelMapWithOffset(chunkCoord, 1, 0);
        NativeArray<ushort> leftVoxelMap = GetVoxelMapWithOffset(chunkCoord, -1, 0);

        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);

        NativeList<float> nativeFrequencies = endlessTerrain.NativeFrequencies;
        NativeList<float> nativeAmplitudes = endlessTerrain.NativeAmplitudes;

        BlockModelData modelData = BlockModelRegistry.BlockModelData;

        ChunkBuildData chunkBuildData = new ChunkBuildData(
            ref chunkPos, ref vertices,
            ref indices,
            ref leftVoxelMap, ref rightVoxelMap,
            ref backVoxelMap, ref forwardVoxelMap,
            ref modelData, ref transparentIndices,
            ref cutoutIndices
        );

        ChunkVoxelBuildData chunkVoxelBuildData = new ChunkVoxelBuildData(
            ref chunkPos, ref voxelMap,
            ref nativeFrequencies, ref nativeAmplitudes,
            ref noiseOffset
        );

        Vector2 terrainNoiseOffset = endlessTerrain.NoiseOffset;
        chunkBuildData.chunkPos[0] = chunkCoord;
        chunkVoxelBuildData.chunkPos[0] = chunkCoord;

        chunkVoxelBuildData.noiseOffset[0] = terrainNoiseOffset.x;
        chunkVoxelBuildData.noiseOffset[1] = terrainNoiseOffset.y;

        BuiltChunkData builtChunkData = new BuiltChunkData(ref vertices, ref indices, ref transparentIndices, ref cutoutIndices, chunkPos[0]);

        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;
        access.StartCoroutine(BuildChunkMesh(chunkCoord, chunkBuildData, chunkVoxelBuildData, builtChunkData));
    }

    [Obsolete]
    private void BuildChunkVoxelData(ChunkVoxelBuildData chunkVoxelBuildData) {
        buildChunkVoxelMapMarker.Begin();

        var chunkVoxelBuilderJob = new ChunkVoxelBuilderJob() {
            voxelMap = chunkVoxelBuildData.voxelMap,
            coord = chunkVoxelBuildData.chunkPos,

            frequencies = chunkVoxelBuildData.frequencies,
            amplitudes = chunkVoxelBuildData.amplitudes,

            noiseOffset = chunkVoxelBuildData.noiseOffset
        };

        JobHandle chunkVoxelJobHandle = chunkVoxelBuilderJob.Schedule();
        chunkVoxelJobHandle.Complete();

        buildChunkVoxelMapMarker.End();
        placeChunkFeaturesMarker.Begin();

        var chunkPlaceFeaturesJob = new ChunkPlaceFeaturesJob() {
            voxelMap = chunkVoxelBuildData.voxelMap,
            coord = chunkVoxelBuildData.chunkPos,

            frequencies = chunkVoxelBuildData.frequencies,
            amplitudes = chunkVoxelBuildData.amplitudes,

            noiseOffset = chunkVoxelBuildData.noiseOffset,

            featurePlacements = worldFeatures.GetPlacements(),
            featureData = featureData,
            featureSettings = featureSettings
        };

        JobHandle placeFeaturesJobHandle = chunkPlaceFeaturesJob.Schedule();
        placeFeaturesJobHandle.Complete();

        placeChunkFeaturesMarker.End();
        placeChunkDecorationsMarker.Begin();

        var chunkPlaceDecorationsJob = new ChunkDecorationBuilderJob() {
            voxelMap = chunkVoxelBuildData.voxelMap,
            coord = chunkVoxelBuildData.chunkPos,

            frequencies = chunkVoxelBuildData.frequencies,
            amplitudes = chunkVoxelBuildData.amplitudes,

            noiseOffset = chunkVoxelBuildData.noiseOffset
        };

        JobHandle chunkPlaceDecorationsJobHandle = chunkPlaceDecorationsJob.Schedule();
        
        chunkPlaceDecorationsJobHandle.Complete();
        placeChunkDecorationsMarker.End();
    }

    [Obsolete]
    private IEnumerator BuildChunkMesh(long chunkCoord, ChunkBuildData chunkBuildData, ChunkVoxelBuildData chunkVoxelBuildData, BuiltChunkData builtChunkData) {
        NativeArray<ushort> voxelMap = new NativeArray<ushort>(chunkVoxelBuildData.voxelMap, Allocator.Persistent);
        NativeArray<ushort> forwardVoxelMap = new NativeArray<ushort>(chunkBuildData.forwardVoxelMap, Allocator.Persistent);
        NativeArray<ushort> backVoxelMap = new NativeArray<ushort>(chunkBuildData.backVoxelMap, Allocator.Persistent);
        NativeArray<ushort> rightVoxelMap = new NativeArray<ushort>(chunkBuildData.rightVoxelMap, Allocator.Persistent);
        NativeArray<ushort> leftVoxelMap = new NativeArray<ushort>(chunkBuildData.leftVoxelMap, Allocator.Persistent);

        NativeArray<float3> voxelVerts = new NativeArray<float3>(chunkBuildData.modelData.voxelVerts, Allocator.Persistent);
        NativeArray<uint> voxelTris = new NativeArray<uint>(chunkBuildData.modelData.voxelTris, Allocator.Persistent);
        NativeArray<float2> voxelUVs = new NativeArray<float2>(chunkBuildData.modelData.voxelUVs, Allocator.Persistent);

        NativeParallelHashMap<ushort, BlockState> blockStateDictionary = new NativeParallelHashMap<ushort, BlockState>(1, Allocator.Persistent);
        foreach(var pair in BlockRegistry.BlockStateDictionary) blockStateDictionary.Add(pair.Key, pair.Value);

        NativeParallelHashMap<byte, BlockStateModel> blockModelDictionary = new NativeParallelHashMap<byte, BlockStateModel>(1, Allocator.Persistent);
        foreach(var pair in BlockModelRegistry.BlockModelDictionary) blockModelDictionary.Add(pair.Key, pair.Value);

        var chunkMeshJob = new ChunkMeshBuilderJob() {
            voxelMap = voxelMap,

            leftVoxelMap = leftVoxelMap,
            rightVoxelMap = rightVoxelMap,
            backVoxelMap = backVoxelMap,
            forwardVoxelMap = forwardVoxelMap,

            blockStates = blockStateDictionary,
            blockModels = blockModelDictionary,

            vertices = chunkBuildData.vertices,
            indices = chunkBuildData.indices,
            transparentIndices = chunkBuildData.transparentIndices,
            cutoutIndices = chunkBuildData.cutoutIndices,

            voxelVerts = voxelVerts,
            voxelTris = voxelTris,
            voxelUVs = voxelUVs,
        };

        JobHandle chunkMeshJobHandle = chunkMeshJob.Schedule();

        yield return new WaitUntil(() => {
            return chunkMeshJobHandle.IsCompleted;
        });

        chunkMeshJobHandle.Complete();
        worldEventSystem.InvokeChunkObjectBuild(builtChunkData);

        chunkBuildData.Dispose();
        chunkVoxelBuildData.Dispose();

        voxelMap.Dispose();
        forwardVoxelMap.Dispose();
        backVoxelMap.Dispose();
        rightVoxelMap.Dispose();
        leftVoxelMap.Dispose();

        voxelVerts.Dispose();
        voxelTris.Dispose();
        voxelUVs.Dispose();

        blockStateDictionary.Dispose();
        blockModelDictionary.Dispose();
    }

    [Obsolete]
    public NativeArray<ushort> GetVoxelMap(long chunkCoord) {
        if(!enabled) {
            throw new InvalidOperationException("ChunkBuilder is disabled! Cannot continue.");
        }

        return GetVoxelMapWithOffset(chunkCoord, 0, 0);
    }

    [Obsolete]
    public NativeArray<ushort> GetVoxelMapWithOffset(long chunkCoord, int offsetX, int offsetZ) {
        long chunkPos = ChunkPositionHelper.ModifyChunkPos(chunkCoord, offsetX, offsetZ);

        if(WorldStorage.DoesChunkExist(chunkPos)) return WorldStorage.GetChunk(chunkPos);
        else return CreateNewVoxelMap(chunkPos);
    }

    public void SaveChunkVoxelMap(long chunkCoord, NativeArray<ushort> voxelMap) {
        if(WorldStorage.DoesChunkExist(chunkCoord)) WorldStorage.SetChunk(chunkCoord, ref voxelMap);
        else WorldStorage.AddChunk(chunkCoord, ref voxelMap);
    }

    [Obsolete]
    public NativeArray<ushort> CreateNewVoxelMap(long chunkCoord) {
        Vector2 terrainNoiseOffset = endlessTerrain.NoiseOffset;

        NativeArray<long> chunkPos = new NativeArray<long>(1, Allocator.Persistent);
        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);
        NativeArray<ushort> voxelMap = CreateEmptyVoxelMap();

        NativeList<float> nativeFrequencies = endlessTerrain.NativeFrequencies;
        NativeList<float> nativeAmplitudes = endlessTerrain.NativeAmplitudes;

        ChunkVoxelBuildData chunkVoxelBuildData = new ChunkVoxelBuildData(
            ref chunkPos, ref voxelMap,
            ref nativeFrequencies, ref nativeAmplitudes,
            ref noiseOffset
        );

        chunkVoxelBuildData.chunkPos[0] = chunkCoord;

        chunkVoxelBuildData.noiseOffset[0] = terrainNoiseOffset.x;
        chunkVoxelBuildData.noiseOffset[1] = terrainNoiseOffset.y;

        BuildChunkVoxelData(chunkVoxelBuildData);
        SaveChunkVoxelMap(chunkCoord, chunkVoxelBuildData.voxelMap);

        chunkVoxelBuildData.Dispose();
        return voxelMap;
    }

    public NativeArray<ushort> CreateEmptyVoxelMap() {
        return new NativeArray<ushort>(VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth, Allocator.Persistent);
    }
}
