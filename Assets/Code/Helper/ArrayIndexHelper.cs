using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class ArrayIndexHelper
{
    public static int GetVoxelArrayIndex(int x, int y, int z) {
        return ((z + 1) * (VoxelProperties.chunkWidth + 2) * VoxelProperties.chunkHeight) + (y * (VoxelProperties.chunkWidth + 2)) + (x + 1);
    }

    public static int GetVoxelArrayIndexWithResolution(int x, int y, int z, int r) {
        return ((z + 1) * ((VoxelProperties.chunkWidth / r) + 2) * (VoxelProperties.chunkHeight / r)) + (y * ((VoxelProperties.chunkWidth / r) + 2)) + (x + 1);
    }

    public static int GetTriangleIndex(int x, int z) {
        return x * 4 + z;
    }
}
