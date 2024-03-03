using Unity.Mathematics;
using Unity.Collections;

public struct BlockModelData
{
    public NativeList<float3> voxelVerts;
    public NativeList<uint> voxelTris;
    public NativeList<float2> voxelUVs;

    public void Dispose() {
        voxelVerts.Dispose();
        voxelTris.Dispose();
        voxelUVs.Dispose();
    }
}
