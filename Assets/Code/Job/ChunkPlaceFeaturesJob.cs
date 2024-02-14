using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkPlaceFeaturesJob : IJob
{
    public NativeArray<long> coord;
    public NativeArray<ushort> voxelMap;

    public NativeArray<FeaturePlacement> featurePlacements;
    public NativeParallelHashMap<FeaturePlacement, ushort> featureData;
    public NativeParallelHashMap<ushort, FeatureSettings> featureSettings;

    public void Execute() {
        PlaceFeatures();
    }

    private void PlaceFeatures() {
        int chunkWX = ChunkPositionHelper.GetChunkPosWX(coord[0]);
        int chunkWZ = ChunkPositionHelper.GetChunkPosWZ(coord[0]);

        foreach(FeaturePlacement placement in featurePlacements) {
            FeatureSettings currentFeatureSettings = featureSettings[placement.id];

            int minX = -(currentFeatureSettings.size.x / 2);
            int maxX = (currentFeatureSettings.size.x / 2);

            int minY = 0;
            int maxY = currentFeatureSettings.size.y - 1;

            int minZ = -(currentFeatureSettings.size.z / 2);
            int maxZ = (currentFeatureSettings.size.z / 2);

            minX += placement.x;
            maxX += placement.x;

            minY += placement.y;
            maxY += placement.y;

            minZ += placement.z;
            maxZ += placement.z;
            
            if(!AABB.IsOverlapping(
                minX, minY, minZ,
                maxX, maxY, maxZ,

                chunkWX - 1, 0, chunkWZ - 1,

                chunkWX + VoxelProperties.chunkWidth + 1, 
                VoxelProperties.chunkHeight, 
                chunkWZ + VoxelProperties.chunkWidth + 1)) {
                    
                continue;
            }

            for(int x = minX; x <= maxX; x++) {
                for(int y = minY; y <= maxY; y++) {
                    for(int z = minZ; z <= maxZ; z++) {
                        int nx = x - chunkWX;
                        int nz = z - chunkWZ;

                        int npx = x - placement.x;
                        int npz = z - placement.z;

                        int fx = nx;
                        int fy = y;
                        int fz = nz;

                        if(fx < 0) fx = VoxelProperties.chunkWidth + x;
                        if(fz < 0) fz = VoxelProperties.chunkWidth + z;

                        if(fx < 0 || fx >= VoxelProperties.chunkWidth ||
                            fy < 0 || fy >= VoxelProperties.chunkHeight ||
                            fz < 0 || fz >= VoxelProperties.chunkWidth) {
                                continue;
                            }

                        int dx = Mathf.Abs(placement.x - (chunkWX + fx));
                        int dz = Mathf.Abs(placement.z - (chunkWZ + fz));

                        if(dx > currentFeatureSettings.size.x) continue;
                        if(dz > currentFeatureSettings.size.z) continue;

                        int checkVoxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(fx, fy, fz);

                        BlockID checkBlockID = new BlockID(voxelMap[checkVoxelMapArrayIndex]);
                        if(!checkBlockID.Equals(currentFeatureSettings.overrideBlock)) continue;

                        FeaturePlacement newPlacement = new FeaturePlacement(npx, y - placement.y, npz, placement.id);
                        if(!featureData.ContainsKey(newPlacement)) continue;

                        ushort id = featureData[newPlacement];
                        if(id == 0) continue;
                        
                        int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(fx, fy, fz);
                        voxelMap[voxelMapArrayIndex] = id;
                    }
                } 
            }
        }
    }
}
