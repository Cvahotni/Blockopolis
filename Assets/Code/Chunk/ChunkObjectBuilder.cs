using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

[RequireComponent(typeof(ChunkObjectPool))]
public class ChunkObjectBuilder : MonoBehaviour
{
    private VertexAttributeDescriptor positionDescriptor = new VertexAttributeDescriptor(VertexAttribute.Position);
	private VertexAttributeDescriptor texCoordDescriptor = new VertexAttributeDescriptor(VertexAttribute.TexCoord0);
	private VertexAttributeDescriptor normalDescriptor = new VertexAttributeDescriptor(VertexAttribute.Normal);
    private readonly string chunkTagName = "Ground";

    public static ChunkObjectBuilder Instance { get; private set; }

    private ChunkObjectPool chunkObjectPool;
    
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
        chunkObjectPool = ChunkObjectPool.Instance;
        SetLayer();
    }

    private void SetLayer() {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    public void BuildChunkObject(object sender, BuiltChunkData builtChunkData) {
        GameObject chunkGameObject = chunkObjectPool.GetFromPool();
        chunkGameObject.SetActive(true);    

        int chunkPositionX = ChunkPositionHelper.GetChunkPosWX(builtChunkData.coord);
        int chunkPositionZ = ChunkPositionHelper.GetChunkPosWZ(builtChunkData.coord);

        chunkGameObject.name = builtChunkData.coord.ToString();    
        chunkGameObject.transform.position = new Vector3(chunkPositionX, 0.0f, chunkPositionZ);

        MeshFilter meshFilter = chunkGameObject.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        meshFilter.mesh.SetVertexBufferParams(builtChunkData.vertices.Length,
            positionDescriptor,
            normalDescriptor,
            texCoordDescriptor
        );

        meshFilter.mesh.MarkDynamic();
        meshFilter.mesh.SetIndexBufferParams(builtChunkData.indices.Length, IndexFormat.UInt32);

        meshFilter.mesh.SetVertexBufferData<ChunkVertex>(builtChunkData.vertices, 0, 0, builtChunkData.vertices.Length, 0, MeshUpdateFlags.DontValidateIndices);
        meshFilter.mesh.SetIndexBufferData<uint>(builtChunkData.indices, 0, 0, builtChunkData.indices.Length, MeshUpdateFlags.DontValidateIndices);

        meshFilter.mesh.SetSubMesh(0, new SubMeshDescriptor(0, builtChunkData.indices.Length));
        meshFilter.mesh.RecalculateBounds();

        MeshRenderer meshRenderer = chunkGameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = terrainMaterial;

        MeshCollider meshCollider = chunkGameObject.AddComponent<MeshCollider>();
        
        meshCollider.sharedMesh = meshFilter.mesh;
        meshCollider.sharedMaterial = physicsMaterial;

        chunkGameObject.layer = groundLayer;
        chunkGameObject.tag = chunkTagName;

        chunkGameObject.SetActive(true);
    }
}
