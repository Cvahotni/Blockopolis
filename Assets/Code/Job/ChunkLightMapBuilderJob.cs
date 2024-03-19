using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkLightMapBuilderJob : IJob
{
    public NativeArray<ushort> voxelMap;
    public NativeArray<byte> lightMap;

    public NativeParallelHashMap<ushort, BlockState> blockStates;

    public void Execute() {
        BuildIntialLightMap();
        EnhanceLightMap();
    }

    private void BuildIntialLightMap() {
        for(int x = 0; x < VoxelProperties.chunkWidth * 3; x++) {
            for(int z = 0; z < VoxelProperties.chunkWidth * 3; z++) {
                int cx = x >= VoxelProperties.chunkWidth ? VoxelProperties.chunkWidth - 1 : x;
                int cz = z >= VoxelProperties.chunkWidth ? VoxelProperties.chunkWidth - 1 : z;

                byte lightLevel = 15;

                for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
                    int arrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(cx, y, cz);

                    ushort block = voxelMap[arrayIndex];
                    byte blockID = BlockIDHelper.ID(block);
                    
                    if(blockID == 0 || blockID == 10) {
                        continue;
                    }

                    byte transparency = blockStates[block].transparency;

                    if(transparency < lightLevel) {
                        lightLevel = transparency;
                    }

                    lightMap[arrayIndex] = LightIDHelper.Pack(lightLevel, 0);
                }
            }
        }
    }

    private void EnhanceLightMap() {
        for(int x = 0; x < VoxelProperties.chunkWidth; x++) {
            for(int z = 0; z < VoxelProperties.chunkWidth; z++) {
                int cx = x >= VoxelProperties.chunkWidth ? VoxelProperties.chunkWidth - 1 : x;
                int cz = z >= VoxelProperties.chunkWidth ? VoxelProperties.chunkWidth - 1: z;

                for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
                    int arrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(cx, y, cz);

                    ushort block = voxelMap[arrayIndex];
                    byte blockID = BlockIDHelper.ID(block);

                    if(blockID == 0 || blockID == 10) {
                        continue;
                    }
                    
                    for(byte f = 0; f < 6; f++) {
                        int3 direction = GetDirection(f);

                        int neighborX = cx + direction.x; 
                        int neighborY = y + direction.y;
                        int neighborZ = cz + direction.z;

                        if(neighborX < 0 || neighborX >= VoxelProperties.chunkWidth ||
                            neighborY < 0 || neighborY >= VoxelProperties.chunkHeight ||
                            neighborZ < 0 || neighborZ >= VoxelProperties.chunkWidth) {
                                continue;
                            }  

                        int currentIndex = ArrayIndexHelper.GetVoxelArrayIndex(cx, y, cz);
                        int neighborIndex = ArrayIndexHelper.GetVoxelArrayIndex(neighborX, neighborY, neighborZ);

                        byte currentLevel = LightIDHelper.Level(lightMap[currentIndex]);
                        byte neighborLevel = LightIDHelper.Level(lightMap[neighborIndex]);

                        byte calculatedLevel = (byte) (currentLevel - VoxelProperties.lightFalloff);

                        if(neighborLevel < calculatedLevel) {
                            lightMap[neighborIndex] = LightIDHelper.Pack(calculatedLevel, 0);
                        }
                    }
                }
            }
        }
    }

    private int3 GetDirection(byte f) {
        switch(f) {
            case 0: {
                return new int3(0, 0, -1);
            }

            case 1: {
                return new int3(0, 0, 1);
            }

            case 2: {
                return new int3(0, 1, 0);
            }

            case 3: {
                return new int3(0, -1, 0);
            }

            case 4: {
                return new int3(-1, 0, 0);
            }

            case 5: {
                return new int3(1, 0, 0);
            }
        }

        return new int3(0, 0, 0);
    }
}
