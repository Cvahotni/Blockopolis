using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public struct ChunkVoxelBuildData
{
    public NativeArray<long> chunkPos;
    public NativeArray<ushort> voxelMap;

    public NativeList<float> frequencies;
    public NativeList<float> amplitudes;

    public NativeArray<float> noiseOffset;

    public ChunkVoxelBuildData(ref NativeArray<long> chunkPos,
                          ref NativeArray<ushort> voxelMap,
                          ref NativeList<float> frequencies, ref NativeList<float> amplitudes, 
                          ref NativeArray<float> noiseOffset) {
                            this.chunkPos = chunkPos;
                            this.voxelMap = voxelMap;
                            this.frequencies = frequencies;
                            this.amplitudes = amplitudes;
                            this.noiseOffset = noiseOffset;
    }

    public void Dispose() {
        noiseOffset.Dispose();
    }
}
