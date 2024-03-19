using UnityEngine;

public class VoxelProperties {
    public static readonly int chunkWidth = 16;
    public static readonly int chunkHeight = 256;

	public static readonly int noiseMapWidth = 16;
    public static readonly int noiseMapHeight = 256;

	public static readonly int worldMaxY = 1024;
	public static readonly int worldMinY = -128;

	public static readonly int featureChunkBuffer = 2;

	public static readonly int chunkWidthLowRes = chunkWidth / 2;
    public static readonly int chunkHeightLowRes = chunkHeight / 2;
	public static readonly int chunkSizeMultiplication = 2;

    public static readonly int chunkBitShift = 4;
	public static readonly int textureAtlasSizeInBlocks = 16;

	public static readonly int worldSize = 15000;
	public static readonly int worldSizeHalved = worldSize / 2;
	public static readonly int worldSizeInChunks = worldSize / chunkWidth;
	public static readonly int worldSizeInChunksHalved = worldSizeInChunks / 2;

	public static readonly int regionWidth = 256;
	public static readonly int regionWidthInChunks = regionWidth >> chunkBitShift;
	public static readonly int worldSizeInRegions = worldSize / regionWidth;
	public static readonly int worldSizeInRegionsHalved = worldSizeInRegions / 2;

	public static readonly int regionBitShift = 8;
	public static readonly int featurePlacementBuffer = 3;

	public static readonly byte lightFalloff = 1;

	public static readonly Vector3[] faceChecks = new Vector3[6] {
		new Vector3(0.0f, 0.0f, -1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 1.0f, 0.0f),
		new Vector3(0.0f, -1.0f, 0.0f),
		new Vector3(-1.0f, 0.0f, 0.0f),
		new Vector3(1.0f, 0.0f, 0.0f)
	};
}
