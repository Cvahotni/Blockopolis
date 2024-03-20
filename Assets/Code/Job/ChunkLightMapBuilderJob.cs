using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct ChunkLightMapBuilderJob : IJob
{
    public NativeArray<ushort> voxelMap;
    public NativeArray<ushort> voxelMapForward;
    public NativeArray<ushort> voxelMapBack;
    public NativeArray<ushort> voxelMapLeft;
    public NativeArray<ushort> voxelMapRight;
    public NativeArray<ushort> voxelMapForwardLeft;
    public NativeArray<ushort> voxelMapForwardRight;
    public NativeArray<ushort> voxelMapBackLeft;
    public NativeArray<ushort> voxelMapBackRight;

    public NativeArray<byte> lightMap;
    public NativeArray<byte> lightMapForward;
    public NativeArray<byte> lightMapBack;
    public NativeArray<byte> lightMapLeft;
    public NativeArray<byte> lightMapRight;
    public NativeArray<byte> lightMapForwardLeft;
    public NativeArray<byte> lightMapForwardRight;
    public NativeArray<byte> lightMapBackLeft;
    public NativeArray<byte> lightMapBackRight;

    public NativeParallelHashMap<ushort, BlockState> blockStates;

    public void Execute() {
        BuildIntialLightMap();
        EnhanceLightMap();
    }

    private void BuildIntialLightMap() {
        for(int x = -VoxelProperties.chunkWidth; x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth; x++) {
            for(int z = -VoxelProperties.chunkWidth; z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth; z++) {
                byte lightLevel = 15;
                
                for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
                    ushort block = GetVoxel(x, y, z);
                    byte blockID = BlockIDHelper.ID(block);
                    
                    if(blockID == 0 || blockID == 10) {
                        continue;
                    }

                    byte transparency = blockStates[block].transparency;

                    if(transparency < lightLevel) {
                        lightLevel = transparency;
                    }

                    if(x < -VoxelProperties.chunkWidth || x >= VoxelProperties.chunkWidth + VoxelProperties.chunkWidth ||
                        y < 0 || y >= VoxelProperties.chunkHeight ||
                        z < -VoxelProperties.chunkWidth || z >= VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
                        continue;
                    }

                    SetLight(x, y, z, LightIDHelper.Pack(lightLevel, 0));
                }
            }
        }
    }

    private void EnhanceLightMap() {
        for(int x = -VoxelProperties.chunkWidth; x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth; x++) {
            for(int z = -VoxelProperties.chunkWidth; z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth; z++) {
                for(int y = VoxelProperties.chunkHeight - 1; y >= 0; y--) {
                    ushort block = GetVoxel(x, y, z);
                    byte blockID = BlockIDHelper.ID(block);

                    if(blockID == 0 || blockID == 10) {
                        continue;
                    }
                    
                    for(byte f = 0; f < 6; f++) {
                        int3 direction = GetDirection(f);

                        int neighborX = x + direction.x; 
                        int neighborY = y + direction.y;
                        int neighborZ = z + direction.z;

                        if(neighborX < -VoxelProperties.chunkWidth || neighborX >= VoxelProperties.chunkWidth + VoxelProperties.chunkWidth ||
                            neighborY < 0 || neighborY >= VoxelProperties.chunkHeight ||
                            neighborZ < -VoxelProperties.chunkWidth || neighborZ >= VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
                                continue;
                            }  

                        byte currentLevel = LightIDHelper.Level(GetLight(x, y, z));
                        byte neighborLevel = LightIDHelper.Level(GetLight(neighborX, neighborY, neighborZ));

                        byte calculatedLevel = (byte) (currentLevel - VoxelProperties.lightFalloff);

                        if(neighborLevel < calculatedLevel) {
                            SetLight(neighborX, neighborY, neighborZ, LightIDHelper.Pack(calculatedLevel, 0));
                        }
                    }
                }
            }
        }
    }

    private ushort GetVoxel(int x, int y, int z) {
        //Current
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= 0 && z < VoxelProperties.chunkWidth) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
            return voxelMap[voxelMapArrayIndex];
        }

        //Front
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z - VoxelProperties.chunkWidth);
            return voxelMapForward[voxelMapArrayIndex];
        }

        //Back
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= -VoxelProperties.chunkWidth && z < 0) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z + VoxelProperties.chunkWidth);
            return voxelMapBack[voxelMapArrayIndex];
        }

        //Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= 0 && z < VoxelProperties.chunkWidth) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z);
            return voxelMapLeft[voxelMapArrayIndex];
        }

        //Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= 0 && z < VoxelProperties.chunkWidth) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z);
            return voxelMapRight[voxelMapArrayIndex];
        }

        //Back Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= -VoxelProperties.chunkWidth && z < 0) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z + VoxelProperties.chunkWidth);
            return voxelMapBackLeft[voxelMapArrayIndex];
        }

        //Back Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= -VoxelProperties.chunkWidth && z < 0) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z + VoxelProperties.chunkWidth);
            return voxelMapBackRight[voxelMapArrayIndex];
        }

        //Front Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z - VoxelProperties.chunkWidth);
            return voxelMapForwardLeft[voxelMapArrayIndex];
        }

        //Front Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int voxelMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z - VoxelProperties.chunkWidth);
            return voxelMapForwardRight[voxelMapArrayIndex];
        }

        return 0;
    }

    private byte GetLight(int x, int y, int z) {
        //Current
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= 0 && z < VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
            return lightMap[lightMapArrayIndex];
        }

        //Front
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z - VoxelProperties.chunkWidth);
            return lightMapForward[lightMapArrayIndex];
        }

        //Back
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= -VoxelProperties.chunkWidth && z < 0) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z + VoxelProperties.chunkWidth);
            return lightMapBack[lightMapArrayIndex];
        }

        //Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= 0 && z < VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z);
            return lightMapLeft[lightMapArrayIndex];
        }

        //Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= 0 && z < VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z);
            return lightMapRight[lightMapArrayIndex];
        }

        //Back Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= -VoxelProperties.chunkWidth && z < 0) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z + VoxelProperties.chunkWidth);
            return lightMapBackLeft[lightMapArrayIndex];
        }

        //Back Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= -VoxelProperties.chunkWidth && z < 0) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z + VoxelProperties.chunkWidth);
            return lightMapBackRight[lightMapArrayIndex];
        }

        //Front Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z - VoxelProperties.chunkWidth);
            return lightMapForwardLeft[lightMapArrayIndex];
        }

        //Front Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z - VoxelProperties.chunkWidth);
            return lightMapForwardRight[lightMapArrayIndex];
        }

        return 0;
    }

    private void SetLight(int x, int y, int z, byte value) {
        //Current
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= 0 && z < VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z);
            lightMap[lightMapArrayIndex] = value;
        }

        //Front
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z - VoxelProperties.chunkWidth);
            lightMapForward[lightMapArrayIndex] = value;
        }

        //Back
        if(x >= 0 && x < VoxelProperties.chunkWidth && z >= -VoxelProperties.chunkWidth && z < 0) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x, y, z + VoxelProperties.chunkWidth);
            lightMapBack[lightMapArrayIndex] = value;
        }

        //Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= 0 && z < VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z);
            lightMapLeft[lightMapArrayIndex] = value;
        }

        //Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= 0 && z < VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z);
            lightMapRight[lightMapArrayIndex] = value;
        }

        //Back Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= -VoxelProperties.chunkWidth && z < 0) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z + VoxelProperties.chunkWidth);
            lightMapBackLeft[lightMapArrayIndex] = value;
        }

        //Back Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= -VoxelProperties.chunkWidth && z < 0) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z + VoxelProperties.chunkWidth);
            lightMapBackRight[lightMapArrayIndex] = value;
        }

        //Front Left
        if(x >= -VoxelProperties.chunkWidth && x < 0 && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x + VoxelProperties.chunkWidth, y, z - VoxelProperties.chunkWidth);
            lightMapForwardLeft[lightMapArrayIndex] = value;
        }

        //Front Right
        if(x >= VoxelProperties.chunkWidth && x < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth && z >= VoxelProperties.chunkWidth && z < VoxelProperties.chunkWidth + VoxelProperties.chunkWidth) {
            int lightMapArrayIndex = ArrayIndexHelper.GetVoxelArrayIndex(x - VoxelProperties.chunkWidth, y, z - VoxelProperties.chunkWidth);
            lightMapForwardRight[lightMapArrayIndex] = value;
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
