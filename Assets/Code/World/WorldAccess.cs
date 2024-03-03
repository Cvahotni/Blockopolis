using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class WorldAccess
{
    public static void ModifyBlocks(List<VoxelModification> modifications) {
        WorldAllocator worldAllocator = WorldAllocator.Instance;
        List<long> chunksToAddToQueue = new List<long>();

        foreach(VoxelModification modification in modifications) {
            Vector3Int position = new Vector3Int(modification.X, modification.Y, modification.Z);
            BlockID id = modification.ID;

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

    private static void ModifyChunkVoxelMap(long chunk, int relativeX, int relativeY, int relativeZ, BlockID currentBlock) {
        ChunkBuilder chunkBuilder = ChunkBuilder.Instance;
        
        if(chunkBuilder == null) {
            Debug.LogError("The ChunkBuilder script must be present in the scene to use ModifyChunkVoxelMap");
            return;
        }

        if(WorldAllocator.IsChunkOutsideOfWorld(chunk)) return;
        if(!WorldStorage.DoesChunkExist(chunk)) return;

        NativeArray<ushort> voxelMap = chunkBuilder.GetVoxelMap(chunk);

        int voxelMapIndex = ArrayIndexHelper.GetVoxelArrayIndex(relativeX, relativeY, relativeZ);
        voxelMap[voxelMapIndex] = currentBlock.Pack();
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

    public static BlockID GetBlockAt(Vector3 pos) {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        return GetBlockAt(x, y, z);
    }

    public static BlockID GetBlockAt(int worldX, int worldY, int worldZ) {
        return new BlockID(GetPackedBlockAt(worldX, worldY, worldZ));
    }

    public static ushort GetPackedBlockAt(int worldX, int worldY, int worldZ) {
        long chunkPos = WorldPositionHelper.BlockPositionToChunkPos(worldX, worldZ);
        ushort air = new BlockID(0).Pack();

        if(WorldAllocator.IsChunkOutsideOfWorld(chunkPos)) return air;
        if(!WorldStorage.DoesChunkExist(chunkPos)) return air;

        int relativeX = WorldPositionHelper.GetRelativeX(worldX);
        int relativeZ = WorldPositionHelper.GetRelativeZ(worldZ);

        ChunkBuilder chunkBuilder = ChunkBuilder.Instance;

        if(chunkBuilder == null) {
            Debug.LogError("The ChunkBuilder script must be present in the scene to use GetBlockAt");
            return air;
        }

        NativeArray<ushort> voxelMap = chunkBuilder.GetVoxelMap(chunkPos);

        int voxelMapIndex = ArrayIndexHelper.GetVoxelArrayIndex(relativeX, worldY, relativeZ);
        if(worldY < 0 || worldY >= VoxelProperties.chunkHeight) return air;

        return voxelMap[voxelMapIndex];
    }
}
