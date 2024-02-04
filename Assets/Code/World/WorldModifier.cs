using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class WorldModifier
{
    public static void ModifyBlocks(List<VoxelModification> modifications) {
        WorldAllocator worldAllocator = WorldAllocator.Instance;
        List<long> chunksToAddToQueue = new List<long>();

        foreach(VoxelModification modification in modifications) {
            Vector3Int position = new Vector3Int(modification.X, modification.Y, modification.Z);
            ushort id = modification.ID;

            long chunkPos = WorldPositionHelper.BlockPositionToChunkPos(position.x, position.z);

            if(!WorldStorage.DoesChunkExist(chunkPos)) {
                Debug.LogError("Tried to modify the world at an invalid chunk position: " + chunkPos);
                continue;
            }

            int relativeX = WorldPositionHelper.GetRelativeX(position.x);
            int relativeZ = WorldPositionHelper.GetRelativeZ(position.z);

            ModifyChunkVoxelMap(chunkPos, relativeX, position.y, relativeZ, id);

            if(WorldPositionHelper.IsBlockOnChunkEdge(relativeX, relativeZ)) {
                Border.BorderDirection borderDirection = WorldPositionHelper.GetBorderDirection(relativeX, relativeZ, VoxelProperties.chunkWidth);
                AddNeighborChunks(chunkPos, chunksToAddToQueue);
            }
            
            if(!chunksToAddToQueue.Contains(chunkPos)) chunksToAddToQueue.Add(chunkPos);
        }

        foreach(long chunkPos in chunksToAddToQueue) worldAllocator.AddImmidiateChunkToQueue(chunkPos);
    }

    private static void ModifyChunkVoxelMap(long chunk, int relativeX, int relativeY, int relativeZ, ushort currentBlock) {
        ChunkBuilder chunkBuilder = ChunkBuilder.Instance;
        
        if(chunkBuilder == null) {
            Debug.LogError("The ChunkBuilder script must be present in the scene to use ModifyChunkVoxelMap");
            return;
        }

        if(WorldAllocator.IsChunkOutsideOfWorld(chunk)) return;
        if(!WorldStorage.DoesChunkExist(chunk)) return;

        NativeArray<ushort> voxelMap = chunkBuilder.GetVoxelMap(chunk);

        int voxelMapIndex = ArrayIndexHelper.GetVoxelArrayIndex(relativeX, relativeY, relativeZ);
        voxelMap[voxelMapIndex] = currentBlock;
    }

    private static void AddNeighborChunks(long currentChunk, List<long> chunksToAddToQueue) {
        long chunkPosXPositive = ChunkPositionHelper.ModifyChunkPos(currentChunk, 1, 0);
        long chunkPosXNegative = ChunkPositionHelper.ModifyChunkPos(currentChunk, -1, 0);

        long chunkPosZPositive = ChunkPositionHelper.ModifyChunkPos(currentChunk, 0, 1);
        long chunkPosZNegative = ChunkPositionHelper.ModifyChunkPos(currentChunk, 0, -1);

        if(!chunksToAddToQueue.Contains(chunkPosXPositive)) chunksToAddToQueue.Add(chunkPosXPositive);
        if(!chunksToAddToQueue.Contains(chunkPosXNegative)) chunksToAddToQueue.Add(chunkPosXNegative);
        
        if(!chunksToAddToQueue.Contains(chunkPosZPositive)) chunksToAddToQueue.Add(chunkPosZPositive);
        if(!chunksToAddToQueue.Contains(chunkPosZNegative)) chunksToAddToQueue.Add(chunkPosZNegative);
    }

    public static ushort GetBlockAt(Vector3 pos) {
        int x = (int) pos.x;
        int y = (int) pos.y;
        int z = (int) pos.z;

        return GetBlockAt(x, y, z);
    }

    public static ushort GetBlockAt(int worldX, int worldY, int worldZ) {
        long chunkPos = WorldPositionHelper.BlockPositionToChunkPos(worldX, worldZ);
        if(WorldAllocator.IsChunkOutsideOfWorld(chunkPos)) return 0;

        if(!WorldStorage.DoesChunkExist(chunkPos)) return 0;

        int relativeX = WorldPositionHelper.GetRelativeX(worldX);
        int relativeZ = WorldPositionHelper.GetRelativeZ(worldZ);

        ChunkBuilder chunkBuilder = ChunkBuilder.Instance;

        if(chunkBuilder == null) {
            Debug.LogError("The ChunkBuilder script must be present in the scene to use GetBlockAt");
            return 0;
        }

        if(!WorldStorage.DoesChunkExist(chunkPos)) return 0;
        NativeArray<ushort> voxelMap = chunkBuilder.GetVoxelMap(chunkPos);

        int voxelMapIndex = ArrayIndexHelper.GetVoxelArrayIndex(relativeX, worldY, relativeZ);
        if(worldY < 0 || worldY >= VoxelProperties.chunkHeight) return 0;

        return voxelMap[voxelMapIndex];
    }
}
