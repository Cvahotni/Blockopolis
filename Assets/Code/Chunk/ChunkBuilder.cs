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

    private readonly ProfilerMarker buildChunkVoxelMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkVoxelMap");
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

        ChunkData currentChunkData = GetChunkDataWithOffset(chunkCoord, 0, 0);
        ChunkData forwardVoxelMap = GetChunkDataWithOffset(chunkCoord, 0, 1);
        ChunkData backVoxelMap = GetChunkDataWithOffset(chunkCoord, 0, -1);
        ChunkData rightVoxelMap = GetChunkDataWithOffset(chunkCoord, 1, 0);
        ChunkData leftVoxelMap = GetChunkDataWithOffset(chunkCoord, -1, 0);
        ChunkData backLeftVoxelMap = GetChunkDataWithOffset(chunkCoord, -1, -1);
        ChunkData backRightVoxelMap = GetChunkDataWithOffset(chunkCoord, 1, -1);
        ChunkData frontLeftVoxelMap = GetChunkDataWithOffset(chunkCoord, -1, 1);
        ChunkData frontRightVoxelMap = GetChunkDataWithOffset(chunkCoord, 1, 1);

        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);

        NativeList<float> nativeFrequencies = endlessTerrain.NativeFrequencies;
        NativeList<float> nativeAmplitudes = endlessTerrain.NativeAmplitudes;

        BlockModelData modelData = BlockModelRegistry.BlockModelData;

        ChunkBuildData chunkBuildData = new ChunkBuildData(
            ref chunkPos, ref vertices,
            ref indices,
            ref modelData, ref transparentIndices,
            ref cutoutIndices
        );

        ChunkVoxelBuildData chunkVoxelBuildData = new ChunkVoxelBuildData(
            ref chunkPos, ref currentChunkData.voxelMap, ref currentChunkData.lightMap,
            ref nativeFrequencies, ref nativeAmplitudes,
            ref noiseOffset
        );

        ChunkNeighborData chunkNeighborData = new ChunkNeighborData(
            ref leftVoxelMap.voxelMap, ref rightVoxelMap.voxelMap,
            ref backVoxelMap.voxelMap, ref forwardVoxelMap.voxelMap,

            ref backLeftVoxelMap.voxelMap, ref backRightVoxelMap.voxelMap,
            ref frontLeftVoxelMap.voxelMap, ref frontRightVoxelMap.voxelMap
        );

        Vector2 terrainNoiseOffset = endlessTerrain.NoiseOffset;

        chunkBuildData.chunkPos[0] = chunkCoord;
        chunkVoxelBuildData.chunkPos[0] = chunkCoord;

        chunkVoxelBuildData.noiseOffset[0] = terrainNoiseOffset.x;
        chunkVoxelBuildData.noiseOffset[1] = terrainNoiseOffset.y;

        BuiltChunkData builtChunkData = new BuiltChunkData(ref vertices, ref indices, ref transparentIndices, ref cutoutIndices, chunkPos[0]);

        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;
        access.StartCoroutine(BuildChunkLightData(chunkCoord, chunkVoxelBuildData, chunkNeighborData, chunkBuildData, builtChunkData));
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
    private IEnumerator BuildChunkLightData(long chunkCoord, ChunkVoxelBuildData chunkVoxelBuildData, ChunkNeighborData chunkNeighborData, ChunkBuildData chunkBuildData, BuiltChunkData builtChunkData) {
        NativeParallelHashMap<ushort, BlockState> blockStateDictionary = new NativeParallelHashMap<ushort, BlockState>(1, Allocator.Persistent);
        foreach(var pair in BlockRegistry.BlockStateDictionary) blockStateDictionary.Add(pair.Key, pair.Value);

        NativeArray<ushort> voxelMap = new NativeArray<ushort>(chunkVoxelBuildData.voxelMap, Allocator.Persistent);
        NativeArray<byte> lightMap = new NativeArray<byte>(chunkVoxelBuildData.lightMap, Allocator.Persistent);

        NativeArray<ushort> forwardVoxelMap = new NativeArray<ushort>(chunkNeighborData.forwardVoxelMap, Allocator.Persistent);
        NativeArray<ushort> backVoxelMap = new NativeArray<ushort>(chunkNeighborData.backVoxelMap, Allocator.Persistent);
        NativeArray<ushort> rightVoxelMap = new NativeArray<ushort>(chunkNeighborData.rightVoxelMap, Allocator.Persistent);
        NativeArray<ushort> leftVoxelMap = new NativeArray<ushort>(chunkNeighborData.leftVoxelMap, Allocator.Persistent);

        NativeArray<ushort> backLeftVoxelMap = new NativeArray<ushort>(chunkNeighborData.backLeftVoxelMap, Allocator.Persistent);
        NativeArray<ushort> backRightVoxelMap = new NativeArray<ushort>(chunkNeighborData.backRightVoxelMap, Allocator.Persistent);
        NativeArray<ushort> frontLeftVoxelMap = new NativeArray<ushort>(chunkNeighborData.frontLeftVoxelMap, Allocator.Persistent);
        NativeArray<ushort> frontRightVoxelMap = new NativeArray<ushort>(chunkNeighborData.frontRightVoxelMap, Allocator.Persistent);

        NativeArray<byte> forwardLightMap = CreateEmptyLightMap();
        NativeArray<byte> backLightMap = CreateEmptyLightMap();
        NativeArray<byte> leftLightMap = CreateEmptyLightMap();
        NativeArray<byte> rightLightMap = CreateEmptyLightMap();

        NativeArray<byte> backLeftLightMap = CreateEmptyLightMap();
        NativeArray<byte> backRightLightMap = CreateEmptyLightMap();
        NativeArray<byte> forwardLeftLightMap = CreateEmptyLightMap();
        NativeArray<byte> forwardRightLightMap = CreateEmptyLightMap();

        var chunkBuildLightMapJob = new ChunkLightMapBuilderJob() {
            voxelMap = voxelMap,

            voxelMapForward = forwardVoxelMap,
            voxelMapBack = backVoxelMap,
            voxelMapLeft = leftVoxelMap,
            voxelMapRight = rightVoxelMap,

            voxelMapBackLeft = backLeftVoxelMap,
            voxelMapBackRight = backRightVoxelMap,
            voxelMapForwardLeft = frontLeftVoxelMap,
            voxelMapForwardRight = frontRightVoxelMap,

            lightMapForward = forwardLightMap,
            lightMapBack = backLightMap,
            lightMapLeft = leftLightMap,
            lightMapRight = rightLightMap,

            lightMapBackLeft = backLeftLightMap,
            lightMapBackRight = backRightLightMap,
            lightMapForwardLeft = forwardLeftLightMap,
            lightMapForwardRight = forwardRightLightMap,

            lightMap = lightMap,
            blockStates = blockStateDictionary
        };

        JobHandle chunkBuildLightMapJobHandle = chunkBuildLightMapJob.Schedule();

        yield return new WaitUntil(() => {
            return chunkBuildLightMapJobHandle.IsCompleted;
        });

        chunkBuildLightMapJobHandle.Complete();
        SaveChunkVoxelMap(chunkCoord, chunkVoxelBuildData.voxelMap, chunkVoxelBuildData.lightMap);

        ChunkNeighborData newChunkNeighborData = new ChunkNeighborData(
            ref leftVoxelMap, ref rightVoxelMap,
            ref backVoxelMap, ref forwardVoxelMap,

            ref backLeftVoxelMap, ref backRightVoxelMap,
            ref frontLeftVoxelMap, ref frontRightVoxelMap
        );

        ChunkVoxelBuildData newChunkVoxelBuildData = new ChunkVoxelBuildData(
            ref chunkVoxelBuildData.chunkPos, ref voxelMap, ref lightMap,
            ref chunkVoxelBuildData.frequencies, ref chunkVoxelBuildData.amplitudes,
            ref chunkVoxelBuildData.noiseOffset
        );

        NativeArray<byte> newLightMap = new NativeArray<byte>(
            lightMap, Allocator.Persistent);

        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;
        access.StartCoroutine(BuildChunkMesh(chunkCoord, chunkBuildData, newChunkVoxelBuildData, builtChunkData, chunkNeighborData, chunkBuildLightMapJobHandle));
    }

    [Obsolete]
    private IEnumerator BuildChunkMesh(long chunkCoord, ChunkBuildData chunkBuildData, ChunkVoxelBuildData chunkVoxelBuildData, BuiltChunkData builtChunkData, ChunkNeighborData chunkNeighborData, JobHandle jobHandle) {
        NativeArray<ushort> voxelMap = new NativeArray<ushort>(chunkVoxelBuildData.voxelMap, Allocator.Persistent);
        NativeArray<byte> lightMap = new NativeArray<byte>(chunkVoxelBuildData.lightMap, Allocator.Persistent);
        NativeArray<ushort> forwardVoxelMap = new NativeArray<ushort>(chunkNeighborData.forwardVoxelMap, Allocator.Persistent);
        NativeArray<ushort> backVoxelMap = new NativeArray<ushort>(chunkNeighborData.backVoxelMap, Allocator.Persistent);
        NativeArray<ushort> rightVoxelMap = new NativeArray<ushort>(chunkNeighborData.rightVoxelMap, Allocator.Persistent);
        NativeArray<ushort> leftVoxelMap = new NativeArray<ushort>(chunkNeighborData.leftVoxelMap, Allocator.Persistent);

        NativeArray<float3> voxelVerts = new NativeArray<float3>(chunkBuildData.modelData.voxelVerts, Allocator.Persistent);
        NativeArray<uint> voxelTris = new NativeArray<uint>(chunkBuildData.modelData.voxelTris, Allocator.Persistent);
        NativeArray<float2> voxelUVs = new NativeArray<float2>(chunkBuildData.modelData.voxelUVs, Allocator.Persistent);

        NativeParallelHashMap<ushort, BlockState> blockStateDictionary = new NativeParallelHashMap<ushort, BlockState>(1, Allocator.Persistent);
        foreach(var pair in BlockRegistry.BlockStateDictionary) blockStateDictionary.Add(pair.Key, pair.Value);

        NativeParallelHashMap<byte, BlockStateModel> blockModelDictionary = new NativeParallelHashMap<byte, BlockStateModel>(1, Allocator.Persistent);
        foreach(var pair in BlockModelRegistry.BlockModelDictionary) blockModelDictionary.Add(pair.Key, pair.Value);

        var chunkMeshJob = new ChunkMeshBuilderJob()
        {
            voxelMap = voxelMap,
            lightMap = lightMap,

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
            voxelUVs = voxelUVs
        };

        JobHandle chunkMeshJobHandle = chunkMeshJob.Schedule();

        yield return new WaitUntil(() => {
            return chunkMeshJobHandle.IsCompleted;
        });

        JobHandle.CombineDependencies(chunkMeshJobHandle, jobHandle).Complete();
        worldEventSystem.InvokeChunkObjectBuild(builtChunkData);

        chunkBuildData.Dispose();
        chunkVoxelBuildData.Dispose();

        voxelMap.Dispose();
        lightMap.Dispose();
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

        return GetChunkDataWithOffset(chunkCoord, 0, 0).voxelMap;
    }
    
    [Obsolete]
    public ChunkData GetChunkDataWithOffset(long chunkCoord, int offsetX, int offsetZ) {
        long chunkPos = ChunkPositionHelper.ModifyChunkPos(chunkCoord, offsetX, offsetZ);

        if(WorldStorage.DoesChunkExist(chunkPos)) {
            NativeArray<ushort> voxelMap = WorldStorage.GetChunkVoxelMap(chunkPos);
            NativeArray<byte> lightMap = WorldStorage.GetChunkLightMap(chunkPos);

            ChunkData chunkData = new ChunkData(
                ref voxelMap, ref lightMap
            );

            return chunkData;
        }

        else return CreateNewChunkData(chunkPos);
    }

    public void SaveChunkVoxelMap(long chunkCoord, NativeArray<ushort> voxelMap, NativeArray<byte> lightMap) {
        if(WorldStorage.DoesChunkExist(chunkCoord)) WorldStorage.SetChunk(chunkCoord, ref voxelMap);
        else WorldStorage.AddChunk(chunkCoord, ref voxelMap, ref lightMap);
    }

    [Obsolete]
    public ChunkData CreateNewChunkData(long chunkCoord) {
        Vector2 terrainNoiseOffset = endlessTerrain.NoiseOffset;

        NativeArray<long> chunkPos = new NativeArray<long>(1, Allocator.Persistent);
        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);

        NativeArray<ushort> voxelMap = CreateEmptyVoxelMap();
        NativeArray<byte> lightMap = CreateEmptyLightMap();

        NativeList<float> nativeFrequencies = endlessTerrain.NativeFrequencies;
        NativeList<float> nativeAmplitudes = endlessTerrain.NativeAmplitudes;

        ChunkVoxelBuildData chunkVoxelBuildData = new ChunkVoxelBuildData(
            ref chunkPos, ref voxelMap, ref lightMap,
            ref nativeFrequencies, ref nativeAmplitudes,
            ref noiseOffset
        );

        chunkVoxelBuildData.chunkPos[0] = chunkCoord;

        chunkVoxelBuildData.noiseOffset[0] = terrainNoiseOffset.x;
        chunkVoxelBuildData.noiseOffset[1] = terrainNoiseOffset.y;

        BuildChunkVoxelData(chunkVoxelBuildData);
        SaveChunkVoxelMap(chunkCoord, chunkVoxelBuildData.voxelMap, chunkVoxelBuildData.lightMap);

        chunkVoxelBuildData.Dispose();
        return new ChunkData(ref voxelMap, ref lightMap);
    }

    public NativeArray<ushort> CreateEmptyVoxelMap() {
        return new NativeArray<ushort>(VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth, Allocator.Persistent);
    }

    public NativeArray<byte> CreateEmptyLightMap() {
        return new NativeArray<byte>(VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth, Allocator.Persistent);
    }
}
