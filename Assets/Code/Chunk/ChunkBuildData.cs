using Unity.Collections;

public struct ChunkBuildData
{
    public NativeArray<long> chunkPos;

    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;

    public BlockModelData modelData;

    public NativeList<uint> transparentIndices;
    public NativeList<uint> cutoutIndices;

    public ChunkBuildData(ref NativeArray<long> chunkPos, ref NativeList<ChunkVertex> vertices, 
                          ref NativeList<uint> indices,
                          ref BlockModelData modelData, ref NativeList<uint> transparentIndices,
                          ref NativeList<uint> cutoutIndices) {
                            this.chunkPos = chunkPos;
                            this.vertices = vertices;
                            this.indices = indices;
                            this.modelData = modelData;
                            this.transparentIndices = transparentIndices;
                            this.cutoutIndices = cutoutIndices;
    }

    public void Dispose() {
        chunkPos.Dispose();
        vertices.Dispose();
        indices.Dispose();
        transparentIndices.Dispose();
        cutoutIndices.Dispose();
    }
}
