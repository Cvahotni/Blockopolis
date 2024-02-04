using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkMeshBuilderJob : IJob
{
    public NativeArray<ushort> voxelMap;

    public NativeArray<ushort> leftVoxelMap;
    public NativeArray<ushort> rightVoxelMap;
    public NativeArray<ushort> forwardVoxelMap;
    public NativeArray<ushort> backVoxelMap;

    public NativeParallelHashMap<ushort, BlockType> blockTypes; 

    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;
    public NativeList<uint> transparentIndices;

    public NativeArray<float3> voxelVerts;
    public NativeArray<uint> voxelTris;
    public NativeArray<float2> voxelUVs;

    public void Execute() {
        BuildMesh();
    }
    
    private void BuildMesh() {
        uint vertexIndex = 0;
        
        for(int x = 0; x < VoxelProperties.chunkWidth; x++) {
            for(int y = 0; y < VoxelProperties.chunkHeight; y++) {
                for(int z = 0; z < VoxelProperties.chunkWidth; z++) {
                    int currentVoxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
                    ushort currentVoxel = voxelMap[currentVoxelIndex];

                    if(currentVoxel == 0) continue;
                    vertexIndex = MeshVoxel(currentVoxel, vertexIndex, x, y, z);
                }
            }
        }
    }

    private uint MeshVoxel(ushort currentVoxel, uint vertexIndex, int x, int y, int z) {
        uint currentVertexIndex = vertexIndex;

        for(int f = 0; f < 6; f++) {
            Vector3 faceDirection = VoxelProperties.faceChecks[f];

            int neighborX = x + Mathf.FloorToInt(faceDirection.x);
            int neighborY = y + Mathf.FloorToInt(faceDirection.y);
            int neighborZ = z + Mathf.FloorToInt(faceDirection.z);

            bool yOutOfRange = y + faceDirection.y < 0 || y + faceDirection.y >= VoxelProperties.chunkHeight;
			bool isVerticalFace = faceDirection.y <= -1.0f || faceDirection.y >= 1.0f;

            ushort neighborVoxel = 0;

            if(!yOutOfRange || !isVerticalFace) {
                neighborVoxel = GetNeighborVoxel(neighborX, neighborY, neighborZ);
            }

            BlockType blockType = blockTypes[currentVoxel];
            BlockType neighborBlockType = blockTypes[neighborVoxel];

            bool shouldCullFace = ShouldCullFace(blockType, f);
            bool neighborCheck1 = (blockType.transparent && neighborBlockType.solid && shouldCullFace);
            bool neighborCheck2 = (blockType.transparent && neighborBlockType.transparent && !shouldCullFace);

            if(neighborCheck1 || neighborCheck2) continue;
            if(neighborBlockType.solid && !neighborBlockType.transparent && blockType.solid && shouldCullFace) continue;

            float2 blockFaceUVOffset = GetBlockFaceUVOffset(blockType, f);

            Vector3 pos = new Vector3(x, y, z);
            currentVertexIndex = MeshFace(blockType, currentVertexIndex, f, pos, faceDirection, blockFaceUVOffset, blockType.transparent);
        }

        return currentVertexIndex;
    }

    private bool ShouldCullFace(BlockType type, int f) {
        switch(f) {
            case 0: return type.cullBack;
            case 1: return type.cullFront;
            case 2: return type.cullUp;
            case 3: return type.cullBottom;
            case 4: return type.cullLeft;
            case 5: return type.cullRight;
        }

        return false;
    }

    private ushort GetNeighborVoxel(int x, int y, int z) {
        int neighborX = x;
        int neighborZ = z;

        if(x < 0) neighborX = VoxelProperties.chunkWidth - 1;
        if(z < 0) neighborZ = VoxelProperties.chunkWidth - 1;

        if(x >= VoxelProperties.chunkWidth) neighborX = 0;
        if(z >= VoxelProperties.chunkWidth) neighborZ = 0;

        if(x < 0) {
            int voxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, y, neighborZ);
            return leftVoxelMap[voxelIndex];
        } 

        if(x >= VoxelProperties.chunkWidth) {
            int voxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, y, neighborZ);
            return rightVoxelMap[voxelIndex];
        }

        if(z < 0) {
            int voxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, y, neighborZ);
            return backVoxelMap[voxelIndex];
        } 

        if(z >= VoxelProperties.chunkWidth) {
            int voxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, y, neighborZ);
            return forwardVoxelMap[voxelIndex];
        }

        int currentVoxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, y, neighborZ);
        return voxelMap[currentVoxelIndex];
    }

    private uint MeshFace(BlockType type, uint vertexIndex, int f, float3 pos, float3 faceCheck, float2 uvOffset, bool transparent) {
        uint currentVertexIndex = vertexIndex;
        
        switch(f) {
            case 0: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, type.backVertsStart, type.backVertsEnd, type.backTrisStart, type.backTrisEnd, transparent);
                break;
            }

            case 1: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, type.frontVertsStart, type.frontVertsEnd, type.frontTrisStart, type.frontTrisEnd, transparent);
                break;
            }

            case 2: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, type.upVertsStart, type.upVertsEnd, type.upTrisStart, type.upTrisEnd, transparent);
                break;
            }

            case 3: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, type.bottomVertsStart, type.bottomVertsEnd, type.bottomTrisStart, type.bottomTrisEnd, transparent);
                break;
            }

            case 4: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, type.leftVertsStart, type.leftVertsEnd, type.leftTrisStart, type.leftTrisEnd, transparent);
                break;
            }

            case 5: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, type.rightVertsStart, type.rightVertsEnd, type.rightTrisStart, type.rightTrisEnd, transparent);
                break;
            }
        }

        return currentVertexIndex;
    }
    
    private uint BuildFace(uint vertexIndex, float3 pos, float3 faceCheck, float2 uvOffset, uint startVertexIndex, uint endVertexIndex, uint startTriIndex, uint endTriIndex, bool transparent) {
        uint newVertexIndex = 0;
        float textureSize = 1.0f / VoxelProperties.textureAtlasSizeInBlocks;

        for(uint v = startVertexIndex; v < endVertexIndex; v++) {
            float3 vertex = pos + voxelVerts[(int) v];
            float3 uv = new float3((uvOffset.x + voxelUVs[(int) v].x) * textureSize, (uvOffset.y + voxelUVs[(int) v].y) * textureSize, 0);

            vertices.Add(new ChunkVertex(vertex, faceCheck, uv));
            newVertexIndex++;
        }

        for(uint t = startTriIndex; t < endTriIndex; t++) {
            if(transparent) transparentIndices.Add(vertexIndex + voxelTris[(int) t]);
            else indices.Add(vertexIndex + voxelTris[(int) t]);
        }

        return vertexIndex + newVertexIndex;
    }

    private float2 GetBlockFaceUVOffset(BlockType blockType, int f) {
        switch(f) {
            case 0: return blockType.backTexture;
            case 1: return blockType.frontTexture;
            case 2: return blockType.upTexture;
            case 3: return blockType.downTexture;
            case 4: return blockType.leftTexture;
            case 5: return blockType.rightTexture;
        }

        return new float2(0.0f);
    }
}
