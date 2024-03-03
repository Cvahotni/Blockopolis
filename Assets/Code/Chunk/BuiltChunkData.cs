using Unity.Collections;

public struct BuiltChunkData
{
    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;
    public NativeList<uint> transparentIndices;
    public NativeList<uint> cutoutIndices;

    public long coord;

    public BuiltChunkData(ref NativeList<ChunkVertex> vertices, ref NativeList<uint> indices, ref NativeList<uint> transparentIndices, ref NativeList<uint> cutoutIndices, long coord) {
        this.vertices = vertices;
        this.indices = indices;
        this.transparentIndices = transparentIndices;
        this.cutoutIndices = cutoutIndices;
        this.coord = coord;
    }
}
