using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

[BurstCompile]
public struct ChunkPlaceFeaturesJob : IJob
{
    public NativeArray<long> coord;
    public NativeArray<ushort> voxelMap;

    public NativeList<float> frequencies;
    public NativeList<float> amplitudes;
    public NativeArray<float> noiseOffset;

    public NativeArray<FeaturePlacement> featurePlacements;
    public NativeParallelHashMap<FeaturePlacement, ushort> featureData;
    public NativeParallelHashMap<ushort, FeatureSettings> featureSettings;

    public void Execute() {
        PlaceFeatures();
    }

    private void PlaceFeatures() {
        int chunkWX = ChunkPositionHelper.GetChunkPosWX(coord[0]);
        int chunkWZ = ChunkPositionHelper.GetChunkPosWZ(coord[0]);

        for(int p = 0; p < 2; p++) {
            foreach(FeaturePlacement placement in featurePlacements) {
                FeatureSettings currentFeatureSettings = featureSettings[placement.id];
                if((int) currentFeatureSettings.priority != p) continue;

                int minX = -(currentFeatureSettings.size.x / 2);
                int maxX = currentFeatureSettings.size.x / 2;

                int minY = 0;
                int maxY = currentFeatureSettings.size.y - 1;

                int minZ = -(currentFeatureSettings.size.z / 2);
                int maxZ = currentFeatureSettings.size.z / 2;

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

                            int surface = currentFeatureSettings.placeType == FeaturePlaceType.SurfaceHeight ? GetYLevel(fx, fz) : fy;

                            int checkVoxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(fx, fy, fz);
                            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(fx, surface, fz);

                            FeaturePlacement newPlacement = new FeaturePlacement(npx, y - placement.y, npz, placement.id);
                            if(!featureData.ContainsKey(newPlacement)) continue;

                            BlockID worldCheckBlockID = new BlockID(voxelMap[checkVoxelMapArrayIndex]);
                            if(!worldCheckBlockID.Equals(currentFeatureSettings.overrideBlock)) continue;

                            ushort id = featureData[newPlacement];
                            BlockID featureBlock = new BlockID(id);

                            if(featureBlock.IsAir()) continue;
                            voxelMap[voxelMapArrayIndex] = id;
                        }
                    } 
                }
            }
        }
    }
    
    private int GetYLevel(int x, int z) {
        float worldX = WorldPositionHelper.GetWorldX(x, coord[0]);
        float worldZ = WorldPositionHelper.GetWorldZ(z, coord[0]);

        float noiseLevel = Noise.Get2DNoise(worldX, worldZ, noiseOffset[0], noiseOffset[1], frequencies, amplitudes) + WorldUtil.YOffset;
        return (int) noiseLevel + 1;
    }
}
