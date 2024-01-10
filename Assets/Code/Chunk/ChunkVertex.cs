using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ChunkVertex
{
    Vector3 vertex;
    Vector3 normal;
    Vector3 uv;

    public ChunkVertex(Vector3 vertex, Vector3 normal, Vector3 uv) {
        this.vertex = vertex;
        this.normal = normal;
        this.uv = uv;
    }
}
