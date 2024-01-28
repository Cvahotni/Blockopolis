using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public struct BuiltChunkData
{
    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;
    public NativeList<uint> transparentIndices;

    public long coord;

    public BuiltChunkData(ref NativeList<ChunkVertex> vertices, ref NativeList<uint> indices, ref NativeList<uint> transparentIndices, long coord) {
        this.vertices = vertices;
        this.indices = indices;
        this.transparentIndices = transparentIndices;
        this.coord = coord;
    }
}
