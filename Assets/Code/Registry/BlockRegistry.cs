using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class BlockRegistry
{
    private static List<BlockStateObject> blockStateObjects = new List<BlockStateObject>();
    private static List<BlockState> blockStates = new List<BlockState>();

    private static NativeParallelHashMap<ushort, BlockState> blockStateDictionary;
    private static Dictionary<byte, BlockType> blockTypes = new Dictionary<byte, BlockType>();

    private static bool disposed = false;

    public static Dictionary<byte, BlockType> BlockTypes {
        get { return blockTypes; }
    }

    public static NativeParallelHashMap<ushort, BlockState> BlockStateDictionary {
        get { return blockStateDictionary; }
    }

    public static List<BlockStateObject> BlockStates {
        get { return blockStateObjects; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Start() {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        CreateNativeCollections();
        LoadBlockTypesFromFolder();
        LoadBlockStatesFromFolder();
        PopulateBlockStates();
        PopulateBlockStateDictionary();

        watch.Stop();
        float timeTaken = watch.ElapsedTicks * 1000 / System.Diagnostics.Stopwatch.Frequency;

        Debug.Log("Block Registry finished: " + timeTaken + " ms");
    }

    private static void CreateNativeCollections() {
        blockStateDictionary = new NativeParallelHashMap<ushort, BlockState>(1, Allocator.Persistent);
    }

    private static void LoadBlockStatesFromFolder() {
        Object[] objects = Resources.LoadAll("Block States");
        foreach(Object currentObject in objects) blockStateObjects.Add((BlockStateObject) currentObject);
    }

    private static void LoadBlockTypesFromFolder() {
        Object[] objects = Resources.LoadAll("Block Types");

        foreach(Object currentObject in objects) {
            BlockType blockType = (BlockType) currentObject;
            blockTypes.Add(blockType.id, blockType);
        }
    }

    private static void PopulateBlockStates() {
        foreach(BlockStateObject blockStateObject in blockStateObjects) {
            BlockState blockState = new BlockState();

            blockState.id = blockStateObject.id;
            blockState.variant = blockStateObject.variant;

            blockState.solid = blockStateObject.solid;
            blockState.transparent = blockStateObject.transparent;
            blockState.cutout = blockStateObject.cutout;

            blockState.model = blockStateObject.model.id;

            blockState.frontTexture = new float2(blockStateObject.frontTexture.x, blockStateObject.frontTexture.y);
            blockState.backTexture = new float2(blockStateObject.backTexture.x, blockStateObject.backTexture.y);
            blockState.upTexture = new float2(blockStateObject.upTexture.x, blockStateObject.upTexture.y);
            blockState.downTexture = new float2(blockStateObject.downTexture.x, blockStateObject.downTexture.y);
            blockState.leftTexture = new float2(blockStateObject.leftTexture.x, blockStateObject.leftTexture.y);
            blockState.rightTexture = new float2(blockStateObject.rightTexture.x, blockStateObject.rightTexture.y);

            blockStates.Add(blockState);
        }
    }

    private static void PopulateBlockStateDictionary() {
        foreach(BlockState blockState in blockStates) blockStateDictionary.Add(BlockIDHelper.Pack(blockState.id, blockState.variant), blockState);
    }

    private static void DestroyDictionaries() {
        blockStateDictionary.Dispose();
        disposed = true;
    }

    public static BlockMaterial GetMaterialForBlock(byte id) {
        return blockTypes[id].material;
    }

    public static void OnDestroy() {
        if(disposed) return;
        DestroyDictionaries();
    }
}
