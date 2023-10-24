using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;

[BurstCompile]
public struct ChunkNoiseMapBuildJob : IJob
{
    public NativeArray<float> noiseOffset;
    public NativeArray<float> noiseMap;

    public void Execute() {
        BuildNoiseMap();
    }

    private void BuildNoiseMap() {
        for(int x = 0; x < VoxelProperties.chunkWidthLowRes; x++) {
            for(int z = 0; z < VoxelProperties.chunkWidthLowRes; z++) {
                for(int y = 0; y < VoxelProperties.chunkHeightLowRes; y++) {
                    int currentX = x * VoxelProperties.chunkSizeMultiplication;
                    int currentZ = z * VoxelProperties.chunkSizeMultiplication;
                    int currentY = y * VoxelProperties.chunkSizeMultiplication;

                    int noiseMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndexWithResolution(x, y, z, 2);
                    float densityNoiseLevel = Get3DNoise(currentX, currentY, currentZ);

                    noiseMap[noiseMapArrayIndex] = densityNoiseLevel;
                }
            }
        }
    }

    private float Get3DNoise(float worldX, float y, float worldZ) {
        float terrainNoiseOffsetX = noiseOffset[0];
        float terrainNoiseOffsetZ = noiseOffset[1];

        return Noise.Get3DNoiseAt(terrainNoiseOffsetX, 0.0f, terrainNoiseOffsetZ, worldX, y, worldZ, 15.0f, 0.0225f);
    }
}
