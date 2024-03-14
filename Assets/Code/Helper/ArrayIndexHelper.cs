public class ArrayIndexHelper
{
    public static int GetVoxelArrayIndex(int x, int y, int z) {
        return (z * VoxelProperties.chunkWidth * VoxelProperties.chunkHeight) + (y * VoxelProperties.chunkWidth) + x;
    }

    public static int GetVoxelArrayIndexWithResolution(int x, int y, int z, int r) {
        return (z * (VoxelProperties.chunkWidth / r) * (VoxelProperties.chunkHeight / r)) + (y * (VoxelProperties.chunkWidth / r)) + x;
    }

    public static int GetTriangleIndex(int x, int z) {
        return x * 4 + z;
    }
}
