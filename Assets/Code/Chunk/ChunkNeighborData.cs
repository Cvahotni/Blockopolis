using Unity.Collections;

public struct ChunkNeighborData
{
    public NativeArray<ushort> leftVoxelMap;
    public NativeArray<ushort> rightVoxelMap;
    public NativeArray<ushort> backVoxelMap;
    public NativeArray<ushort> forwardVoxelMap;

    public NativeArray<ushort> backLeftVoxelMap;
    public NativeArray<ushort> backRightVoxelMap;
    public NativeArray<ushort> frontLeftVoxelMap;
    public NativeArray<ushort> frontRightVoxelMap;

    public ChunkNeighborData(ref NativeArray<ushort> leftVoxelMap, ref NativeArray<ushort> rightVoxelMap,
                            ref NativeArray<ushort> backVoxelMap, ref NativeArray<ushort> forwardVoxelMap,
                            ref NativeArray<ushort> backLeftVoxelMap, ref NativeArray<ushort> backRightVoxelMap,
                            ref NativeArray<ushort> frontLeftVoxelMap, ref NativeArray<ushort> frontRightVoxelMap) {
                                this.leftVoxelMap = leftVoxelMap;
                                this.rightVoxelMap = rightVoxelMap;
                                this.backVoxelMap = backVoxelMap;
                                this.forwardVoxelMap = forwardVoxelMap;

                                this.backLeftVoxelMap = backLeftVoxelMap;
                                this.backRightVoxelMap = backRightVoxelMap;
                                this.frontLeftVoxelMap = frontLeftVoxelMap;
                                this.frontRightVoxelMap = frontRightVoxelMap;
                            }
}
