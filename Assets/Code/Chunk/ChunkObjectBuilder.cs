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
    private Material[] materials = new Material[2];
    
    [SerializeField]
    private Material terrainMaterial;

    [SerializeField]
    private Material transparentMaterial;
    
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
        SetMaterials();
    }

    private void SetLayer() {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void SetMaterials() {
        materials[0] = terrainMaterial;
        materials[1] = transparentMaterial;
    }

    public void BuildChunkObject(object sender, BuiltChunkData builtChunkData) {
        RemoveExistingChunkObject(builtChunkData.coord);

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

        if(builtChunkData.vertices.Length == 0 || builtChunkData.indices.Length == 0) {
            Debug.Log("Tried to build empty chunk: " + builtChunkData.vertices.Length + ", " + builtChunkData.indices.Length + ", " + builtChunkData.coord);
            chunkObjectPool.ReturnToPool(chunkGameObject);
            
            return;
        }

        int originalIndicesLength = builtChunkData.indices.Length;
        int transparentIndicesLength = builtChunkData.transparentIndices.Length;

        builtChunkData.indices.AddRange(builtChunkData.transparentIndices);

        meshFilter.mesh.MarkDynamic();
        meshFilter.mesh.SetIndexBufferParams(builtChunkData.indices.Length, IndexFormat.UInt32);

        meshFilter.mesh.SetVertexBufferData<ChunkVertex>(builtChunkData.vertices, 0, 0, builtChunkData.vertices.Length, 0, MeshUpdateFlags.DontValidateIndices);
        meshFilter.mesh.SetIndexBufferData<uint>(builtChunkData.indices, 0, 0, builtChunkData.indices.Length, MeshUpdateFlags.DontValidateIndices);

        meshFilter.mesh.subMeshCount = 2;

        meshFilter.mesh.SetSubMesh(0, new SubMeshDescriptor(0, originalIndicesLength));
        meshFilter.mesh.SetSubMesh(1, new SubMeshDescriptor(originalIndicesLength, transparentIndicesLength));

        meshFilter.mesh.RecalculateBounds();

        MeshRenderer meshRenderer = chunkGameObject.GetComponent<MeshRenderer>();

        meshRenderer.materials = materials;
        MeshCollider meshCollider = chunkGameObject.AddComponent<MeshCollider>();
        
        meshCollider.sharedMesh = meshFilter.mesh;
        meshCollider.sharedMaterial = physicsMaterial;

        chunkGameObject.layer = groundLayer;
        chunkGameObject.tag = chunkTagName;

        chunkGameObject.SetActive(true);
    }

    private void RemoveExistingChunkObject(long coord) {
        GameObject existingChunkObject = GameObject.Find("" + coord);
        if(existingChunkObject == null) return;

        chunkObjectPool.ReturnToPool(existingChunkObject);
    }
}
