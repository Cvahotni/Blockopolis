using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public struct ChunkBuildData
{
    public NativeArray<long> chunkPos;

    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;

    public NativeArray<ushort> leftVoxelMap;
    public NativeArray<ushort> rightVoxelMap;
    public NativeArray<ushort> backVoxelMap;
    public NativeArray<ushort> forwardVoxelMap;

    public ChunkBuildData(ref NativeArray<long> chunkPos, ref NativeList<ChunkVertex> vertices, 
                          ref NativeList<uint> indices,
                          ref NativeArray<ushort> leftVoxelMap, ref NativeArray<ushort> rightVoxelMap,
                          ref NativeArray<ushort> backVoxelMap, ref NativeArray<ushort> forwardVoxelMap) {
                            this.chunkPos = chunkPos;
                            this.vertices = vertices;
                            this.indices = indices;
                            this.leftVoxelMap = leftVoxelMap;
                            this.rightVoxelMap = rightVoxelMap;
                            this.backVoxelMap = backVoxelMap;
                            this.forwardVoxelMap = forwardVoxelMap;
    }

    public void Dispose() {
        chunkPos.Dispose();
        vertices.Dispose();
        indices.Dispose();
    }
}
