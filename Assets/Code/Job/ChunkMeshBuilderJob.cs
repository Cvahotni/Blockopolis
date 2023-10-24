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
    public NativeHashMap<ushort, BlockType> blockTypes; 

    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;

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
            Vector3 faceDirection = VoxelMeshProperties.faceChecks[f];

            int neighborX = x + Mathf.FloorToInt(faceDirection.x);
            int neighborY = y + Mathf.FloorToInt(faceDirection.y);
            int neighborZ = z + Mathf.FloorToInt(faceDirection.z);

            bool yOutOfRange = y + faceDirection.y < 0 || y + faceDirection.y >= VoxelProperties.chunkHeight;
			bool isVerticalFace = faceDirection.y <= -1.0f || faceDirection.y >= 1.0f;

            uint neighborVoxel = 0;

            if(!yOutOfRange || !isVerticalFace) {
                int neighborVoxelIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, neighborY, neighborZ);
                neighborVoxel = voxelMap[neighborVoxelIndex];
            }

            BlockType blockType = blockTypes[currentVoxel];
            if(neighborVoxel != 0 && blockType.solid) continue;

            float2 blockFaceUVOffset = GetBlockFaceUVOffset(blockType, f);

            Vector3 pos = new Vector3(x, y, z);
            currentVertexIndex = MeshFace(currentVertexIndex, f, pos, faceDirection, blockFaceUVOffset);
        }

        return currentVertexIndex;
    }

    private uint MeshFace(uint vertexIndex, int f, Vector3 pos, Vector3 faceCheck, float2 uvOffset) {
        float textureSize = 1.0f / VoxelProperties.textureAtlasSizeInBlocks;

        Vector3 vertex0 = pos + VoxelMeshProperties.voxelVerts[VoxelMeshProperties.voxelTris[ArrayIndexHelper.GetTriangleIndex(f, 0)]];
        Vector3 vertex1 = pos + VoxelMeshProperties.voxelVerts[VoxelMeshProperties.voxelTris[ArrayIndexHelper.GetTriangleIndex(f, 1)]];
        Vector3 vertex2 = pos + VoxelMeshProperties.voxelVerts[VoxelMeshProperties.voxelTris[ArrayIndexHelper.GetTriangleIndex(f, 2)]];
        Vector3 vertex3 = pos + VoxelMeshProperties.voxelVerts[VoxelMeshProperties.voxelTris[ArrayIndexHelper.GetTriangleIndex(f, 3)]];

        vertices.Add(new ChunkVertex(vertex0, faceCheck, new Vector3((uvOffset.x + VoxelMeshProperties.voxelUvs[0].x) * textureSize, (uvOffset.y + VoxelMeshProperties.voxelUvs[0].y) * textureSize, 0.0f)));
        vertices.Add(new ChunkVertex(vertex1, faceCheck, new Vector3((uvOffset.x + VoxelMeshProperties.voxelUvs[1].x) * textureSize, (uvOffset.y + VoxelMeshProperties.voxelUvs[1].y) * textureSize, 0.0f)));
        vertices.Add(new ChunkVertex(vertex2, faceCheck, new Vector3((uvOffset.x + VoxelMeshProperties.voxelUvs[2].x) * textureSize, (uvOffset.y + VoxelMeshProperties.voxelUvs[2].y) * textureSize, 0.0f)));
        vertices.Add(new ChunkVertex(vertex3, faceCheck, new Vector3((uvOffset.x + VoxelMeshProperties.voxelUvs[3].x) * textureSize, (uvOffset.y + VoxelMeshProperties.voxelUvs[3].y) * textureSize, 0.0f)));

        indices.Add(vertexIndex);
        indices.Add(vertexIndex + 1);
        indices.Add(vertexIndex + 2);
        indices.Add(vertexIndex + 2);
        indices.Add(vertexIndex + 1);
        indices.Add(vertexIndex + 3);

        return vertexIndex + 4;
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
