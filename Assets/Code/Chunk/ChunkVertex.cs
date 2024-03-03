using UnityEngine;
using Unity.Mathematics;

public struct ChunkVertex
{
    public float3 vertex;
    public float3 normal;
    public float3 uv;

    public ChunkVertex(Vector3 vertex, Vector3 normal, Vector3 uv) {
        this.vertex = vertex;
        this.normal = normal;
        this.uv = uv;
    }
}
