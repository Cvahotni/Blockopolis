using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public struct ChunkBuildData
{
    public NativeArray<long> chunkPos;

    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;

    public NativeArray<ushort> voxelMap;
    public NativeList<EncodedVoxelMapEntry> encodedVoxelMap;

    public NativeList<float> frequencies;
    public NativeList<float> amplitudes;

    public NativeArray<float> noiseOffset;
    public NativeArray<float> noiseMap3D;

    public ChunkBuildData(ref NativeArray<long> chunkPos, ref NativeList<ChunkVertex> vertices, 
                          ref NativeList<uint> indices, ref NativeArray<ushort> voxelMap, ref NativeList<EncodedVoxelMapEntry> encodedVoxelMap, 
                          ref NativeList<float> frequencies, ref NativeList<float> amplitudes, ref NativeArray<float> noiseOffset, ref NativeArray<float> noiseMap3D) {
                            this.chunkPos = chunkPos;
                            this.vertices = vertices;
                            this.indices = indices;
                            this.voxelMap = voxelMap;
                            this.encodedVoxelMap = encodedVoxelMap;
                            this.frequencies = frequencies;
                            this.amplitudes = amplitudes;
                            this.noiseOffset = noiseOffset;
                            this.noiseMap3D = noiseMap3D;
    }

    public void Dispose() {
        chunkPos.Dispose();
        vertices.Dispose();
        indices.Dispose();
        encodedVoxelMap.Dispose();
        frequencies.Dispose();
        amplitudes.Dispose();
        noiseOffset.Dispose();
        noiseMap3D.Dispose();
    }
}
