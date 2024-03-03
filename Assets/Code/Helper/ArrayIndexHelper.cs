public class ArrayIndexHelper
{
    public static int GetVoxelArrayIndex(int x, int y, int z) {
        return (z * VoxelProperties.chunkWidth * VoxelProperties.chunkHeight) + (y * VoxelProperties.chunkWidth) + x;
    }

    public static int GetVoxelArrayIndexWithResolution(int x, int y, int z, int r) {
        return ((z + 1) * ((VoxelProperties.chunkWidth / r) + 2) * (VoxelProperties.chunkHeight / r)) + (y * ((VoxelProperties.chunkWidth / r) + 2)) + x + 1;
    }

    public static int GetTriangleIndex(int x, int z) {
        return x * 4 + z;
    }
}
