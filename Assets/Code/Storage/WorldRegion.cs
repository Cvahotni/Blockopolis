using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

[System.Serializable]
public struct WorldRegion
{
    private Dictionary<long, NativeArray<ushort>> voxelStorageMap;
    public Dictionary<long, NativeArray<ushort>> VoxelStorageMap { get { return voxelStorageMap; } }

    private bool disposed;

    public WorldRegion(bool debug) {
        if(debug) Debug.Log("Created WorldRegion");

        voxelStorageMap = new Dictionary<long, NativeArray<ushort>>();
        disposed = false;
    }

    public WorldRegion(Dictionary<long, NativeArray<ushort>> data) {
        voxelStorageMap = data;
        disposed = false;
    }

    public void AddChunk(long coord, NativeArray<ushort> voxelMap) {
        if(voxelStorageMap.ContainsKey(coord)) return;
        voxelStorageMap.Add(coord, voxelMap);
    }

    public void SetChunk(long coord, NativeArray<ushort> voxelMap) {
        voxelStorageMap[coord] = voxelMap;
    }

    public NativeArray<ushort> GetChunk(long coord) {
        return voxelStorageMap[coord];
    }

    public bool DoesChunkExist(long coord) {
        return voxelStorageMap.ContainsKey(coord);
    }

    public int Destroy() {
        if(disposed) return 0;
        int destroyedCount = 0;

        foreach(var nativeArrayPair in voxelStorageMap) {
            nativeArrayPair.Value.Dispose();
            destroyedCount++;
        }

        disposed = true;
        return destroyedCount;
    }
}
