using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelProperties {
    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 256;

	public static readonly int worldMaxY = 1024;
	public static readonly int worldMinY = -128;

	public static readonly int featureChunkMultplier = 3;
	public static readonly int featureChunkWidth = chunkWidth * featureChunkMultplier;

	public static readonly int chunkWidthLowRes = chunkWidth / 2;
    public static readonly int chunkHeightLowRes = chunkHeight / 2;
	public static readonly int chunkSizeMultiplication = 2;

    public static readonly int chunkBitShift = 4;
	public static readonly int textureAtlasSizeInBlocks = 16;

	public static readonly int worldSize = 15000;
	public static readonly int worldSizeHalved = worldSize / 2;
	public static readonly int worldSizeInChunks = worldSize / chunkWidth;
	public static readonly int worldSizeInChunksHalved = worldSizeInChunks / 2;

	public static readonly int regionWidth = 64;
	public static readonly int worldSizeInRegions = worldSize / regionWidth;
	public static readonly int worldSizeInRegionsHalved = worldSizeInRegions / 2;
}
