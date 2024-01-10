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

            long chunkPos = BlockPositionToChunkPos(position.x, position.z);

            if(!WorldStorage.DoesChunkExist(chunkPos)) {
                Debug.Log("Tried to modify the world at an invalid chunk position: " + chunkPos);
                continue;
            }

            int relativeX = GetRelativeX(position.x);
            int relativeZ = GetRelativeZ(position.z);

            ModifyChunkVoxelMap(chunkPos, relativeX, position.y, relativeZ, id);

            if(IsBlockOnChunkEdge(relativeX, relativeZ)) {
                ChunkBorder.ChunkBorderDirection borderDirection = GetChunkBorderDirection(relativeX, relativeZ);

                ModifyNeighborChunkVoxels(chunkPos, id, relativeX, position.y, relativeZ, borderDirection);
                AddNeighborChunks(chunkPos, chunksToAddToQueue);
            }
            
            if(!chunksToAddToQueue.Contains(chunkPos)) chunksToAddToQueue.Add(chunkPos);
        }

        foreach(long chunkPos in chunksToAddToQueue) worldAllocator.AddImmidiateChunkToQueue(chunkPos);
    }

    private static void ModifyNeighborChunkVoxels(long currentChunk, ushort currentBlock, int relativeX, int positionY, int relativeZ, ChunkBorder.ChunkBorderDirection borderDirection) {
        Vector2Int chunkBorderDirectionVector = ChunkBorder.Axes[(int) borderDirection];

        long xNeighborChunk = ChunkPositionHelper.ModifyChunkPos(currentChunk, chunkBorderDirectionVector.x, 0);
        long yNeighborChunk = ChunkPositionHelper.ModifyChunkPos(currentChunk, 0, chunkBorderDirectionVector.y);

        int edgePositionX = relativeX;
        int edgePositionZ = relativeZ;

        if(chunkBorderDirectionVector.x == -1) edgePositionX = VoxelProperties.chunkWidth;
        if(chunkBorderDirectionVector.x == 1) edgePositionX = -1;

        if(chunkBorderDirectionVector.y == -1) edgePositionZ = VoxelProperties.chunkWidth;
        if(chunkBorderDirectionVector.y == 1) edgePositionZ = -1;

        ModifyChunkVoxelMap(xNeighborChunk, edgePositionX, positionY, relativeZ, currentBlock);
        ModifyChunkVoxelMap(yNeighborChunk, relativeX, positionY, edgePositionZ, currentBlock);
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

    private static ChunkBorder.ChunkBorderDirection GetChunkBorderDirection(int relativeX, int relativeZ) {
        bool relativeXIsNotOnEdge = relativeX != 0 && relativeX != VoxelProperties.chunkWidth - 1;
        bool relativeZIsNotOnEdge = relativeZ != 0 && relativeZ != VoxelProperties.chunkWidth - 1;

        bool relativeXIsMin = relativeX == 0;
        bool relativeZIsMin = relativeZ == 0;

        int chunkWidthSubtracted = VoxelProperties.chunkWidth - 1;

        bool relativeXIsMax = relativeX == chunkWidthSubtracted;
        bool relativeZIsMax = relativeZ == chunkWidthSubtracted;

        if(relativeXIsMin && relativeZIsNotOnEdge) return ChunkBorder.ChunkBorderDirection.LEFT;
        if(relativeXIsMax && relativeZIsNotOnEdge) return ChunkBorder.ChunkBorderDirection.RIGHT;

        if(relativeZIsMin && relativeXIsNotOnEdge) return ChunkBorder.ChunkBorderDirection.UP;
        if(relativeZIsMax && relativeXIsNotOnEdge) return ChunkBorder.ChunkBorderDirection.DOWN;

        if(relativeXIsMin && relativeZIsMin) return ChunkBorder.ChunkBorderDirection.UP_LEFT;
        if(relativeXIsMax && relativeZIsMin) return ChunkBorder.ChunkBorderDirection.UP_RIGHT;

        if(relativeXIsMin && relativeZIsMax) return ChunkBorder.ChunkBorderDirection.DOWN_LEFT;
        if(relativeXIsMax && relativeZIsMax) return ChunkBorder.ChunkBorderDirection.DOWN_RIGHT;

        return ChunkBorder.ChunkBorderDirection.UNDEFINED;
    }

    public static ushort GetBlockAt(Vector3 pos) {
        int x = (int) pos.x;
        int y = (int) pos.y;
        int z = (int) pos.z;

        Debug.Log(x + ", " + y + ", " + z);
        return GetBlockAt(x, y, z);
    }

    public static ushort GetBlockAt(int worldX, int worldY, int worldZ) {
        long chunkPos = BlockPositionToChunkPos(worldX, worldZ);
        if(WorldAllocator.IsChunkOutsideOfWorld(chunkPos)) return 0;

        if(!WorldStorage.DoesChunkExist(chunkPos)) return 0;

        int relativeX = GetRelativeX(worldX);
        int relativeZ = GetRelativeZ(worldZ);

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

    private static bool IsBlockOnChunkEdge(int relativeX, int relativeZ) {
        bool isEdgeX = relativeX == 0 || relativeX == VoxelProperties.chunkWidth - 1;
        bool isEdgeZ = relativeZ == 0 || relativeZ == VoxelProperties.chunkWidth - 1;

        return isEdgeX || isEdgeZ;
    }

    private static long BlockPositionToChunkPos(int worldX, int worldZ) {
        int chunkX = worldX >> VoxelProperties.chunkBitShift;
        int chunkZ = worldZ >> VoxelProperties.chunkBitShift;

        return ChunkPositionHelper.GetChunkPos(chunkX, chunkZ);
    }

    private static int GetRelativeX(int worldX) {
        int chunkX = worldX >> VoxelProperties.chunkBitShift;
        int chunkXMultiplied = chunkX << VoxelProperties.chunkBitShift;

        return Mathf.Abs(worldX - chunkXMultiplied);
    }

    private static int GetRelativeZ(int worldZ) {
        int chunkZ = worldZ >> VoxelProperties.chunkBitShift;
        int chunkZMultiplied = chunkZ << VoxelProperties.chunkBitShift;

        return Mathf.Abs(worldZ - chunkZMultiplied);
    }
}
