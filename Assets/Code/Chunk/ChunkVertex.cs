using Unity.Mathematics;

public struct ChunkVertex
{
    public float3 vertex;
    public float3 normal;
    public float3 color;
    public float3 uv;

    public ChunkVertex(float3 vertex, float3 normal, float3 uv, float3 color) {
        this.vertex = vertex;
        this.normal = normal;
        this.uv = uv;
        this.color = color;
    }
}
