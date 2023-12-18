using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class BlockRegistry
{
    private static List<BlockTypeObject> blockTypeObjects = new List<BlockTypeObject>();
    private static List<BlockType> blockTypes = new List<BlockType>();

    private static NativeParallelHashMap<ushort, BlockType> blockTypeDictionary;
    private static bool disposed = false;

    public static NativeParallelHashMap<ushort, BlockType> BlockTypeDictionary {
        get { return blockTypeDictionary; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        blockTypeDictionary = new NativeParallelHashMap<ushort, BlockType>(blockTypes.Count, Allocator.Persistent);

        LoadBlockTypesFromFolder();
        PopulateBlockTypes();
        PopulateBlockTypeDictionary();
    }

    private static void LoadBlockTypesFromFolder() {
        Object[] objects = Resources.LoadAll("Block Types");
        foreach(Object currentObject in objects) blockTypeObjects.Add((BlockTypeObject) currentObject);
    }

    private static void PopulateBlockTypes() {
        foreach(BlockTypeObject blockTypeObject in blockTypeObjects) {
            BlockType blockType = new BlockType();

            blockType.id = blockTypeObject.id;
            blockType.solid = blockTypeObject.solid;

            blockType.material = blockTypeObject.material;
            blockType.hardness = blockTypeObject.hardness;

            blockType.frontTexture = new float2(blockTypeObject.frontTexture.x, blockTypeObject.frontTexture.y);
            blockType.backTexture = new float2(blockTypeObject.backTexture.x, blockTypeObject.backTexture.y);
            blockType.upTexture = new float2(blockTypeObject.upTexture.x, blockTypeObject.upTexture.y);
            blockType.downTexture = new float2(blockTypeObject.downTexture.x, blockTypeObject.downTexture.y);
            blockType.leftTexture = new float2(blockTypeObject.leftTexture.x, blockTypeObject.leftTexture.y);
            blockType.rightTexture = new float2(blockTypeObject.rightTexture.x, blockTypeObject.rightTexture.y);

            blockTypes.Add(blockType);
        }
    }

    private static void PopulateBlockTypeDictionary() {
        foreach(BlockType blockType in blockTypes) blockTypeDictionary.Add(blockType.id, blockType);
    }

    private static void DestroyBlockTypeDictionary() {
        blockTypeDictionary.Dispose();
        disposed = true;
    }

    public static void OnDestroy() {
        if(disposed) return;
        DestroyBlockTypeDictionary();
    }
}
