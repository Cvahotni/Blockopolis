using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObjectPool : MonoBehaviour
{
    public static ChunkObjectPool Instance { get; private set; }
    private WorldEventSystem worldEventSystem;

    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        worldEventSystem = WorldEventSystem.Instance;
        PopulatePool();
    }

    private void PopulatePool() {
        for(int i = 0; i < GameSettings.ChunkPoolSize; i++) {
            GameObject gameObject = CreateGameObject();
            ReturnToPool(gameObject);
        }
    }

    public GameObject GetFromPool() {
        GameObject gameObject = poolQueue.Dequeue();
        return gameObject;
    }

    public void ReturnToPool(object sender, BuiltChunkData data) {
        ReturnToPool(sender, data.coord);
    }

    public void ReturnToPool(object sender, long coord) {
        GameObject gameObject = GameObject.Find("" + coord);
        if(gameObject == null) return;

        worldEventSystem.InvokeChunkRemoveFinal(coord);
        ReturnToPool(gameObject);
    }

    public void ReturnToPool(GameObject gameObject) {
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
