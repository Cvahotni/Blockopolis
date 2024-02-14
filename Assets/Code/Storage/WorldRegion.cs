using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

[System.Serializable]
public struct WorldRegion
{
    private NativeHashMap<long, NativeArray<ushort>> voxelStorageMap;
    public NativeHashMap<long, NativeArray<ushort>> VoxelStorageMap { get { return voxelStorageMap; } }

    private int chunksInRegionAmount;
    private bool disposed;

    public WorldRegion(bool debug) {
        if(debug) Debug.Log("Created WorldRegion");

        chunksInRegionAmount = (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift) * (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift);
        voxelStorageMap = new NativeHashMap<long, NativeArray<ushort>>(chunksInRegionAmount, Allocator.Persistent);
        disposed = false;
    }

    public void AddChunk(long coord, ref NativeArray<ushort> voxelMap) {
        if(voxelStorageMap.ContainsKey(coord)) return;
        voxelStorageMap.Add(coord, voxelMap);

        if(voxelStorageMap.Count > chunksInRegionAmount) {
            int x = ChunkPositionHelper.GetChunkPosX(coord);
            int z = ChunkPositionHelper.GetChunkPosZ(coord);

            Debug.Log("Added chunk to oversized region: " + x + ", " + z + ", " + voxelStorageMap.Count);
        }
    }

    public void SetChunk(long coord, ref NativeArray<ushort> voxelMap) {
        voxelStorageMap[coord] = voxelMap;
    }

    public NativeArray<ushort> GetChunk(long coord) {
        return voxelStorageMap[coord];
    }

    public void RemoveChunk(long coord) {
        voxelStorageMap.Remove(coord);
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

        voxelStorageMap.Dispose();

        disposed = true;
        return destroyedCount;
    }
}
