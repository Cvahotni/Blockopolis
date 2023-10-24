using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

[RequireComponent(typeof(ChunkObjectPool))]
[RequireComponent(typeof(WorldAllocator))]
[RequireComponent(typeof(EndlessTerrain))]
public class ChunkObjectBuilder : MonoBehaviour
{
    private VertexAttributeDescriptor positionDescriptor = new VertexAttributeDescriptor(VertexAttribute.Position);
	private VertexAttributeDescriptor texCoordDescriptor = new VertexAttributeDescriptor(VertexAttribute.TexCoord0);
	private VertexAttributeDescriptor normalDescriptor = new VertexAttributeDescriptor(VertexAttribute.Normal);

    public static ChunkObjectBuilder Instance { get; private set; }

    private WorldAllocator worldAllocator;
    private ChunkObjectPool chunkObjectPool;
    private EndlessTerrain endlessTerrain;
    
    [SerializeField]
    private Material terrainMaterial;
    
    [SerializeField]
    private PhysicMaterial physicsMaterial;

    private LayerMask groundLayer;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldAllocator = WorldAllocator.Instance;
        chunkObjectPool = ChunkObjectPool.Instance;

        SetLayer();
    }

    private void SetLayer() {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    public void BuildChunkObject(NativeList<ChunkVertex> vertices, NativeList<uint> indices, long coord) {
        worldAllocator.RemoveChunkFromScene(coord);
        
        GameObject chunkGameObject = chunkObjectPool.GetFromPool();
        chunkGameObject.SetActive(true);    

        int chunkPositionX = ChunkPositionHelper.GetChunkPosWX(coord);
        int chunkPositionZ = ChunkPositionHelper.GetChunkPosWZ(coord);

        chunkGameObject.name = coord.ToString();    
        chunkGameObject.transform.position = new Vector3(chunkPositionX, 0.0f, chunkPositionZ);

        MeshFilter meshFilter = chunkGameObject.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        meshFilter.mesh.SetVertexBufferParams(vertices.Length,
            positionDescriptor,
            normalDescriptor,
            texCoordDescriptor
        );

        meshFilter.mesh.MarkDynamic();
        meshFilter.mesh.SetIndexBufferParams(indices.Length, IndexFormat.UInt32);

        meshFilter.mesh.SetVertexBufferData<ChunkVertex>(vertices, 0, 0, vertices.Length, 0, MeshUpdateFlags.DontValidateIndices);
        meshFilter.mesh.SetIndexBufferData<uint>(indices, 0, 0, indices.Length, MeshUpdateFlags.DontValidateIndices);

        meshFilter.mesh.SetSubMesh(0, new SubMeshDescriptor(0, indices.Length));
        meshFilter.mesh.RecalculateBounds();

        MeshRenderer meshRenderer = chunkGameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = terrainMaterial;

        MeshCollider meshCollider = chunkGameObject.AddComponent<MeshCollider>();
        
        meshCollider.sharedMesh = meshFilter.mesh;
        meshCollider.sharedMaterial = physicsMaterial;

        chunkGameObject.layer = groundLayer;
        chunkGameObject.SetActive(true);
    }
}
