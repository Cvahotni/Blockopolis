using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;

[RequireComponent(typeof(EndlessTerrain))]
[RequireComponent(typeof(WorldFeatures))]
public class ChunkBuilder : MonoBehaviour
{
    public static ChunkBuilder Instance { get; private set; }

    private readonly ProfilerMarker buildChunkNoiseMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkNoiseMap3D");
    private readonly ProfilerMarker buildChunkVoxelMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkVoxelMap");
    private readonly ProfilerMarker buildChunkMeshMarker = new ProfilerMarker("ChunkBuilder.BuildChunkMesh");
    private readonly ProfilerMarker placeChunkFeaturesMarker = new ProfilerMarker("ChunkBuilder.PlaceChunkFeatures");

    private WorldEventSystem worldEventSystem;
    private EndlessTerrain endlessTerrain;
    private WorldFeatures worldFeatures;

    private NativeList<FeaturePlacement> featurePlacements;
    private NativeParallelHashMap<FeaturePlacement, ushort> featureData = FeatureRegistry.FeatureData;
    private NativeParallelHashMap<ushort, FeatureSettings> featureSettings = FeatureRegistry.FeatureSettings;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        endlessTerrain = EndlessTerrain.Instance;
        worldFeatures = WorldFeatures.Instance;

        featurePlacements = worldFeatures.GetPlacements();
    }

    public void BuildChunk(object sender, long chunkCoord) {
        BuildChunk(chunkCoord);
    }

    private void BuildChunk(long chunkCoord) {
        NativeArray<long> chunkPos = new NativeArray<long>(1, Allocator.Persistent);

        NativeList<ChunkVertex> vertices = new NativeList<ChunkVertex>(Allocator.Persistent);
        NativeList<uint> indices = new NativeList<uint>(Allocator.Persistent);

        NativeArray<ushort> voxelMap = GetVoxelMap(chunkCoord);
        NativeArray<ushort> forwardVoxelMap = GetVoxelMapWithOffset(chunkCoord, 0, 1);
        NativeArray<ushort> backVoxelMap = GetVoxelMapWithOffset(chunkCoord, 0, -1);
        NativeArray<ushort> rightVoxelMap = GetVoxelMapWithOffset(chunkCoord, 1, 0);
        NativeArray<ushort> leftVoxelMap = GetVoxelMapWithOffset(chunkCoord, -1, 0);

        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);

        NativeList<float> nativeFrequencies = endlessTerrain.NativeFrequencies;
        NativeList<float> nativeAmplitudes = endlessTerrain.NativeAmplitudes;

        BlockModelData modelData = BlockRegistry.BlockModelData;

        ChunkBuildData chunkBuildData = new ChunkBuildData(
            ref chunkPos, ref vertices,
            ref indices,
            ref leftVoxelMap, ref rightVoxelMap,
            ref backVoxelMap, ref forwardVoxelMap,
            ref modelData
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

        BuildChunkMesh(chunkBuildData, chunkVoxelBuildData);
        worldEventSystem.InvokeChunkObjectBuild(new BuiltChunkData(ref vertices, ref indices, chunkPos[0]));

        SaveChunkVoxelMap(chunkCoord, chunkVoxelBuildData.voxelMap);

        chunkBuildData.Dispose();
        chunkVoxelBuildData.Dispose();
    }

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

            featurePlacements = featurePlacements,
            featureData = featureData,
            featureSettings = featureSettings
        };

        JobHandle placeFeaturesJobHandle = chunkPlaceFeaturesJob.Schedule();
        placeFeaturesJobHandle.Complete();

        placeChunkFeaturesMarker.End();
    }

    private void BuildChunkMesh(ChunkBuildData chunkBuildData, ChunkVoxelBuildData chunkVoxelBuildData) {
        buildChunkMeshMarker.Begin();

        var chunkMeshJob = new ChunkMeshBuilderJob() {
            voxelMap = chunkVoxelBuildData.voxelMap,

            leftVoxelMap = chunkBuildData.leftVoxelMap,
            rightVoxelMap = chunkBuildData.rightVoxelMap,
            backVoxelMap = chunkBuildData.backVoxelMap,
            forwardVoxelMap = chunkBuildData.forwardVoxelMap,

            blockTypes = BlockRegistry.BlockTypeDictionary,

            vertices = chunkBuildData.vertices,
            indices = chunkBuildData.indices,

            voxelVerts = chunkBuildData.modelData.voxelVerts,
            voxelTris = chunkBuildData.modelData.voxelTris,
            voxelUVs = chunkBuildData.modelData.voxelUVs
        };

        JobHandle chunkMeshJobHandle = chunkMeshJob.Schedule();
        chunkMeshJobHandle.Complete();

        buildChunkMeshMarker.End();
    }

    public NativeArray<ushort> GetVoxelMap(long chunkCoord) {
        return GetVoxelMapWithOffset(chunkCoord, 0, 0);
    }

    public NativeArray<ushort> GetVoxelMapWithOffset(long chunkCoord, int offsetX, int offsetZ) {
        long chunkPos = ChunkPositionHelper.ModifyChunkPos(chunkCoord, offsetX, offsetZ);

        if(WorldStorage.DoesChunkExist(chunkPos)) return WorldStorage.GetChunk(chunkPos);
        else return CreateNewVoxelMap(chunkPos);
    }

    public void SaveChunkVoxelMap(long chunkCoord, NativeArray<ushort> voxelMap) {
        if(WorldStorage.DoesChunkExist(chunkCoord)) WorldStorage.SetChunk(chunkCoord, ref voxelMap);
        else WorldStorage.AddChunk(chunkCoord, ref voxelMap);
    }

    public NativeArray<ushort> CreateNewVoxelMap(long chunkCoord) {
        Vector2 terrainNoiseOffset = endlessTerrain.NoiseOffset;

        NativeArray<long> chunkPos = new NativeArray<long>(1, Allocator.Persistent);
        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);
        NativeArray<ushort> voxelMap = CreateFreshVoxelMap();

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

        chunkVoxelBuildData.Dispose();
        return voxelMap;
    }

    public NativeArray<ushort> CreateFreshVoxelMap() {
        return new NativeArray<ushort>((VoxelProperties.chunkWidth) * VoxelProperties.chunkHeight * (VoxelProperties.chunkWidth), Allocator.Persistent);
    }
}
