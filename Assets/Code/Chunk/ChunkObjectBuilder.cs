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

    private readonly int subMeshCount = 3;
    private readonly string chunkTagName = "Ground";

    public static ChunkObjectBuilder Instance { get; private set; }

    private ChunkObjectPool chunkObjectPool;
    private Material[] materials;

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
        materials = new Material[subMeshCount];
        
        SetLayer();
        SetMaterials();
    }

    private void SetLayer() {
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void SetMaterials() {
        materials[0] = terrainMaterial;
        materials[1] = transparentMaterial;
        materials[2] = terrainMaterial;
    }

    public void BuildChunkObject(object sender, BuiltChunkData builtChunkData) {
        RemoveExistingChunkObject(builtChunkData.coord);
        if(chunkObjectPool.IsEmpty) return;

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
        int cutoutIndicesLength = builtChunkData.cutoutIndices.Length;

        builtChunkData.indices.AddRange(builtChunkData.transparentIndices);
        builtChunkData.indices.AddRange(builtChunkData.cutoutIndices);

        meshFilter.mesh.MarkDynamic();
        meshFilter.mesh.SetIndexBufferParams(builtChunkData.indices.Length, IndexFormat.UInt32);

        meshFilter.mesh.SetVertexBufferData<ChunkVertex>(builtChunkData.vertices, 0, 0, builtChunkData.vertices.Length, 0, MeshUpdateFlags.DontValidateIndices);
        meshFilter.mesh.SetIndexBufferData<uint>(builtChunkData.indices, 0, 0, builtChunkData.indices.Length, MeshUpdateFlags.DontValidateIndices);

        meshFilter.mesh.subMeshCount = subMeshCount;
        meshFilter.mesh.SetSubMesh(0, new SubMeshDescriptor(0, originalIndicesLength));

        CreateChunkObjectCollider(chunkGameObject, meshFilter);
        meshFilter.mesh.SetSubMesh(1, new SubMeshDescriptor(originalIndicesLength, transparentIndicesLength));
        meshFilter.mesh.SetSubMesh(2, new SubMeshDescriptor(originalIndicesLength + transparentIndicesLength, cutoutIndicesLength));

        meshFilter.mesh.RecalculateBounds();

        MeshRenderer meshRenderer = chunkGameObject.GetComponent<MeshRenderer>();
        meshRenderer.materials = materials;

        chunkGameObject.layer = groundLayer;
        chunkGameObject.tag = chunkTagName;

        chunkGameObject.SetActive(true);
    }

    private void CreateChunkObjectCollider(GameObject chunkGameObject, MeshFilter meshFilter) {
        MeshCollider meshCollider = chunkGameObject.AddComponent<MeshCollider>();

        meshCollider.sharedMesh = meshFilter.mesh;
        meshCollider.sharedMaterial = physicsMaterial;
    }

    private void RemoveExistingChunkObject(long coord) {
        GameObject existingChunkObject = GameObject.Find("" + coord);
        if(existingChunkObject == null) return;

        chunkObjectPool.ReturnToPool(existingChunkObject);
    }
}
