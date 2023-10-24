using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObjectPool : MonoBehaviour
{
    public static ChunkObjectPool Instance { get; private set; }
    
    [SerializeField]
    private int poolSize = 1024;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        PopulatePool();
    }

    private void PopulatePool() {
        for(int i = 0; i < poolSize; i++) {
            GameObject gameObject = CreateGameObject();
            ReturnToPool(gameObject);
        }
    }

    public GameObject GetFromPool() {
        GameObject gameObject = poolQueue.Dequeue();
        return gameObject;
    }

    public void ReturnToPool(GameObject gameObject) {
        if(gameObject == null) return;

        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        Destroy(meshCollider);

        gameObject.SetActive(false);
        poolQueue.Enqueue(gameObject);
    }

    private GameObject CreateGameObject() {
        GameObject gameObject = new GameObject("Pooled Object");

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();

        return gameObject;
    }
}
