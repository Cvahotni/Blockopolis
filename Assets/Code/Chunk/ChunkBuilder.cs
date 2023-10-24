using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;

public static class ChunkBuilder
{
    private static readonly ProfilerMarker buildChunkNoiseMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkNoiseMap3D");
    private static readonly ProfilerMarker buildChunkVoxelMapMarker = new ProfilerMarker("ChunkBuilder.BuildChunkVoxelMap");
    private static readonly ProfilerMarker buildChunkMeshMarker = new ProfilerMarker("ChunkBuilder.BuildChunkMesh");

    private static ChunkObjectBuilder chunkObjectBuilder;
    private static WorldAllocator worldAllocator;
    private static EndlessTerrain endlessTerrain;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        chunkObjectBuilder = ChunkObjectBuilder.Instance;
        worldAllocator = WorldAllocator.Instance;
        endlessTerrain = EndlessTerrain.Instance;
    }

    public static void BuildChunk(long chunkCoord) {
        NativeArray<long> chunkPos = new NativeArray<long>(1, Allocator.Persistent);

        NativeList<ChunkVertex> vertices = new NativeList<ChunkVertex>(Allocator.Persistent);
        NativeList<uint> indices = new NativeList<uint>(Allocator.Persistent);

        NativeArray<ushort> voxelMap = GetVoxelMap(chunkCoord);
        NativeList<EncodedVoxelMapEntry> encodedVoxelMap = new NativeList<EncodedVoxelMapEntry>(Allocator.Persistent);

        NativeList<float> frequencies = new NativeList<float>(Allocator.Persistent);
        NativeList<float> amplitudes = new NativeList<float>(Allocator.Persistent);

        NativeArray<float> noiseOffset = new NativeArray<float>(2, Allocator.Persistent);
        NativeArray<float> noiseMap3D = CreateNewNoiseMap();

        foreach(float frequency in endlessTerrain.Frequencies) frequencies.Add(frequency);
        foreach(float amplitude in endlessTerrain.Amplitudes) amplitudes.Add(amplitude);

        ChunkBuildData buildData = new ChunkBuildData(
            chunkPos, vertices,
            indices, voxelMap, encodedVoxelMap,
            frequencies, amplitudes, noiseOffset,
            noiseMap3D
        );

        Vector2 terrainNoiseOffset = endlessTerrain.GetTerrainNoiseOffset();
        buildData.chunkPos[0] = chunkCoord;

        buildData.noiseOffset[0] = terrainNoiseOffset.x;
        buildData.noiseOffset[1] = terrainNoiseOffset.y;

        if(!WorldStorage.DoesChunkExist(chunkCoord)) {
            BuildChunkNoiseMap3D(buildData); //This is for testing purposes
            BuildChunkVoxelMap(buildData);
        }

        BuildChunkMesh(buildData);
        chunkObjectBuilder.BuildChunkObject(vertices, indices, chunkPos[0]);

        SaveChunkVoxelMap(chunkCoord, voxelMap);
        buildData.Dispose();
    }

    private static void BuildChunkNoiseMap3D(ChunkBuildData chunkBuildData) {
        buildChunkNoiseMapMarker.Begin();

        var noiseMapBuildJob = new ChunkNoiseMapBuildJob() {
            noiseOffset = chunkBuildData.noiseOffset,
            noiseMap = chunkBuildData.noiseMap3D
        };

        JobHandle noiseMapBuildJobHandle = noiseMapBuildJob.Schedule();
        noiseMapBuildJobHandle.Complete();

        buildChunkNoiseMapMarker.End();
    }

    private static void BuildChunkVoxelMap(ChunkBuildData chunkBuildData) {
        buildChunkVoxelMapMarker.Begin();

        var chunkVoxelJob = new ChunkVoxelBuilderJob() {
            voxelMap = chunkBuildData.voxelMap,
            coord = chunkBuildData.chunkPos,

            frequencies = chunkBuildData.frequencies,
            amplitudes = chunkBuildData.amplitudes,

            noiseOffset = chunkBuildData.noiseOffset
        };

        JobHandle chunkVoxelJobHandle = chunkVoxelJob.Schedule();
        chunkVoxelJobHandle.Complete();

        buildChunkVoxelMapMarker.End();
    }

    private static void BuildChunkMesh(ChunkBuildData chunkBuildData) {
        buildChunkMeshMarker.Begin();

        var chunkMeshJob = new ChunkMeshBuilderJob() {
            voxelMap = chunkBuildData.voxelMap,
            blockTypes = BlockRegistry.BlockTypeDictionary,

            vertices = chunkBuildData.vertices,
            indices = chunkBuildData.indices
        };

        JobHandle chunkMeshJobHandle = chunkMeshJob.Schedule();
        chunkMeshJobHandle.Complete();

        buildChunkMeshMarker.End();
    }

    public static NativeArray<ushort> GetVoxelMap(long chunkCoord) {
        if(WorldStorage.DoesChunkExist(chunkCoord)) return WorldStorage.GetChunk(chunkCoord);
        else return CreateNewVoxelMap();
    }

    public static void SaveChunkVoxelMap(long chunkCoord, NativeArray<ushort> voxelMap) {
        if(WorldStorage.DoesChunkExist(chunkCoord)) WorldStorage.SetChunk(chunkCoord, voxelMap);
        WorldStorage.AddChunk(chunkCoord, voxelMap);
    }

    public static NativeArray<ushort> CreateNewVoxelMap() {
        return new NativeArray<ushort>((VoxelProperties.chunkWidth + 2) * VoxelProperties.chunkHeight * (VoxelProperties.chunkWidth + 2), Allocator.Persistent);
    }

    public static NativeArray<float> CreateNewNoiseMap() {
        return new NativeArray<float>(((VoxelProperties.chunkWidth / 2) + 2) * (VoxelProperties.chunkHeight / 2) * ((VoxelProperties.chunkWidth / 2) + 2), Allocator.Persistent);
    }
}
