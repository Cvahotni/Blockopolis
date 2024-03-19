using Unity.Collections;

public struct ChunkData
{
    public NativeArray<ushort> voxelMap;
    public NativeArray<byte> lightMap;

    public ChunkData(ref NativeArray<ushort> voxelMap, ref NativeArray<byte> lightMap) {
        this.voxelMap = voxelMap;
        this.lightMap = lightMap;
    }
}
