using UnityEngine;
using Unity.Collections;

[System.Serializable]
public struct WorldRegion
{
    private NativeHashMap<long, NativeArray<ushort>> voxelStorageMap;
    public NativeHashMap<long, NativeArray<ushort>> VoxelStorageMap { get { return voxelStorageMap; } }

    private NativeHashMap<long, NativeArray<byte>> lightStorageMap;
    public NativeHashMap<long, NativeArray<byte>> LightStorageMap { get { return lightStorageMap; } }

    private int chunksInRegionAmount;
    private bool disposed;

    public WorldRegion(bool debug) {
        if(debug) Debug.Log("Created WorldRegion");

        chunksInRegionAmount = (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift) * (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift);

        voxelStorageMap = new NativeHashMap<long, NativeArray<ushort>>(chunksInRegionAmount, Allocator.Persistent);
        lightStorageMap = new NativeHashMap<long, NativeArray<byte>>(chunksInRegionAmount, Allocator.Persistent);

        disposed = false;
    }

    public void AddChunk(long coord, ref NativeArray<ushort> voxelMap) {
        if(voxelStorageMap.ContainsKey(coord)) return;

        if(voxelStorageMap.Count >= chunksInRegionAmount) {
            Debug.Log("Tried to add chunk to full region: " + 
            ChunkPositionHelper.GetChunkPosX(coord) + ", " + 
            ChunkPositionHelper.GetChunkPosZ(coord) + ", " +
            coord);
            
            return;
        }

        voxelStorageMap.Add(coord, voxelMap);
    }

    public void SetChunk(long coord, ref NativeArray<ushort> voxelMap) {
        if(voxelStorageMap.Count > chunksInRegionAmount) {
            Debug.Log("Tried to set chunk to full region: " + coord + ", " + voxelStorageMap.ContainsKey(coord) + ", " + voxelStorageMap.Count);
            return;
        }

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

        foreach(var nativeArrayPair in lightStorageMap) {
            nativeArrayPair.Value.Dispose();
        }

        voxelStorageMap.Dispose();
        lightStorageMap.Dispose();

        disposed = true;
        return destroyedCount;
    }
}
