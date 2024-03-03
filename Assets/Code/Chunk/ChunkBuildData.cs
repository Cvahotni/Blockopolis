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

    public BlockModelData modelData;

    public NativeList<uint> transparentIndices;
    public NativeList<uint> cutoutIndices;

    public ChunkBuildData(ref NativeArray<long> chunkPos, ref NativeList<ChunkVertex> vertices, 
                          ref NativeList<uint> indices,
                          ref NativeArray<ushort> leftVoxelMap, ref NativeArray<ushort> rightVoxelMap,
                          ref NativeArray<ushort> backVoxelMap, ref NativeArray<ushort> forwardVoxelMap,
                          ref BlockModelData modelData, ref NativeList<uint> transparentIndices,
                          ref NativeList<uint> cutoutIndices) {
                            this.chunkPos = chunkPos;
                            this.vertices = vertices;
                            this.indices = indices;
                            this.leftVoxelMap = leftVoxelMap;
                            this.rightVoxelMap = rightVoxelMap;
                            this.backVoxelMap = backVoxelMap;
                            this.forwardVoxelMap = forwardVoxelMap;
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
