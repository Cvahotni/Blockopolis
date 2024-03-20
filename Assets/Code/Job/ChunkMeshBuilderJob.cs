using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;
using System;

[BurstCompile]
public struct ChunkMeshBuilderJob : IJob
{
    public NativeArray<ushort> voxelMap;
    public NativeArray<byte> lightMap;

    public NativeArray<ushort> leftVoxelMap;
    public NativeArray<ushort> rightVoxelMap;
    public NativeArray<ushort> forwardVoxelMap;
    public NativeArray<ushort> backVoxelMap;

    public NativeParallelHashMap<ushort, BlockState> blockStates; 
    public NativeParallelHashMap<byte, BlockStateModel> blockModels; 

    public NativeList<ChunkVertex> vertices;
    public NativeList<uint> indices;
    public NativeList<uint> transparentIndices;
    public NativeList<uint> cutoutIndices;

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

                    if(BlockIDHelper.ID(currentVoxel) == 0) continue;
                    vertexIndex = MeshVoxel(currentVoxel, vertexIndex, x, y, z);
                }
            }
        }
    }

    private uint MeshVoxel(ushort currentVoxel, uint vertexIndex, int x, int y, int z) {
        uint currentVertexIndex = vertexIndex;

        BlockState blockState = blockStates[currentVoxel];
        BlockStateModel currentModel = blockModels[blockState.model];

        for(int f = 0; f < 6; f++) {
            Vector3 faceDirection = VoxelProperties.faceChecks[f];

            int neighborX = x + Mathf.FloorToInt(faceDirection.x);
            int neighborY = y + Mathf.FloorToInt(faceDirection.y);
            int neighborZ = z + Mathf.FloorToInt(faceDirection.z);

            bool yOutOfRange = y + faceDirection.y < 0 || y + faceDirection.y >= VoxelProperties.chunkHeight;
			bool isVerticalFace = faceDirection.y <= -1.0f || faceDirection.y >= 1.0f;

            ushort neighborVoxel = BlockIDHelper.PackEmpty();

            if(!yOutOfRange || !isVerticalFace) {
                neighborVoxel = GetNeighborVoxel(neighborX, neighborY, neighborZ);
            }

            BlockState neighborBlockState = blockStates[neighborVoxel];

            bool shouldCullFace = ShouldCullFace(currentModel, f);
            bool neighborCheck1 = blockState.transparent && neighborBlockState.solid && shouldCullFace;
            bool neighborCheck2 = blockState.transparent && neighborBlockState.transparent && !shouldCullFace;

            if(neighborCheck1 || neighborCheck2) continue;
            if(neighborBlockState.solid && !neighborBlockState.transparent && blockState.solid && shouldCullFace) continue;

            float2 blockFaceUVOffset = GetBlockFaceUVOffset(blockState, f);

            Vector3 pos = new Vector3(x, y, z);
            currentVertexIndex = MeshFace(currentModel, currentVertexIndex, f, pos, faceDirection, blockFaceUVOffset, blockState.transparent, blockState.cutout);
        }

        return currentVertexIndex;
    }

    private bool ShouldCullFace(BlockStateModel model, int f) {
        switch(f) {
            case 0: return model.cullBack;
            case 1: return model.cullFront;
            case 2: return model.cullUp;
            case 3: return model.cullBottom;
            case 4: return model.cullLeft;
            case 5: return model.cullRight;
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

    private uint MeshFace(BlockStateModel model, uint vertexIndex, int f, float3 pos, float3 faceCheck, float2 uvOffset, bool transparent, bool cutout) {
        uint currentVertexIndex = vertexIndex;
        
        switch(f) {
            case 0: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, model.backVertsStart, model.backVertsEnd, model.backTrisStart, model.backTrisEnd, transparent, cutout);
                break;
            }

            case 1: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, model.frontVertsStart, model.frontVertsEnd, model.frontTrisStart, model.frontTrisEnd, transparent, cutout);
                break;
            }

            case 2: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, model.upVertsStart, model.upVertsEnd, model.upTrisStart, model.upTrisEnd, transparent, cutout);
                break;
            }

            case 3: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, model.bottomVertsStart, model.bottomVertsEnd, model.bottomTrisStart, model.bottomTrisEnd, transparent, cutout);
                break;
            }

            case 4: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, model.leftVertsStart, model.leftVertsEnd, model.leftTrisStart, model.leftTrisEnd, transparent, cutout);
                break;
            }

            case 5: {
                currentVertexIndex = BuildFace(vertexIndex, pos, faceCheck, uvOffset, model.rightVertsStart, model.rightVertsEnd, model.rightTrisStart, model.rightTrisEnd, transparent, cutout);
                break;
            }
        }

        return currentVertexIndex;
    }
    
    private uint BuildFace(uint vertexIndex, float3 pos, float3 faceCheck, float2 uvOffset, uint startVertexIndex, uint endVertexIndex, uint startTriIndex, uint endTriIndex, bool transparent, bool cutout) {
        uint newVertexIndex = 0;
        float textureSize = 1.0f / VoxelProperties.textureAtlasSizeInBlocks;

        int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(
            (int) pos.x,
            (int) pos.y,
            (int) pos.z
        );

        byte lightLevel = LightIDHelper.Level(lightMap[lightMapArrayIndex]);
        float3 light = (float) (lightLevel / 15.0f);

        for(uint v = startVertexIndex; v < endVertexIndex; v++) {
            float3 vertex = pos + voxelVerts[(int) v];
            float3 uv = new float3((uvOffset.x + voxelUVs[(int) v].x) * textureSize, (uvOffset.y + voxelUVs[(int) v].y) * textureSize, 0);

            vertices.Add(new ChunkVertex(vertex, faceCheck, uv, 1.0f - light));
            newVertexIndex++;
        }

        for(uint t = startTriIndex; t < endTriIndex; t++) {
            if(transparent) transparentIndices.Add(vertexIndex + voxelTris[(int) t]);
            else if(cutout) cutoutIndices.Add(vertexIndex + voxelTris[(int) t]);
            else indices.Add(vertexIndex + voxelTris[(int) t]);
        }

        return vertexIndex + newVertexIndex;
    }

    private float2 GetBlockFaceUVOffset(BlockState state, int f) {
        switch(f) {
            case 0: return state.backTexture;
            case 1: return state.frontTexture;
            case 2: return state.upTexture;
            case 3: return state.downTexture;
            case 4: return state.leftTexture;
            case 5: return state.rightTexture;
        }

        return new float2(0.0f);
    }

    internal JobHandle Schedule(JobHandle dependency)
    {
        throw new NotImplementedException();
    }
}
