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
    private static BlockModelData blockModelData;

    private static bool disposed = false;

    public static NativeParallelHashMap<ushort, BlockType> BlockTypeDictionary {
        get { return blockTypeDictionary; }
    }

    public static BlockModelData BlockModelData {
        get { return blockModelData; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        CreateNativeCollections();
        LoadBlockTypesFromFolder();
        PopulateBlockTypes();
        PopulateBlockTypeDictionary();
    }

    private static void CreateNativeCollections() {
        blockTypeDictionary = new NativeParallelHashMap<ushort, BlockType>(blockTypes.Count, Allocator.Persistent);

        blockModelData = new BlockModelData();
        blockModelData.voxelVerts = new NativeList<float3>(Allocator.Persistent);
        blockModelData.voxelTris = new NativeList<uint>(Allocator.Persistent);
        blockModelData.voxelUVs = new NativeList<float2>(Allocator.Persistent);
    }

    private static void LoadBlockTypesFromFolder() {
        Object[] objects = Resources.LoadAll("Block Types");
        foreach(Object currentObject in objects) blockTypeObjects.Add((BlockTypeObject) currentObject);
    }

    private static void PopulateBlockTypes() {
        uint vertsStart = 0;
        uint vertsEnd = 0;

        uint trisStart = 0;
        uint trisEnd = 0;

        uint uvsStart = 0;
        uint uvsEnd = 0;

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

            for(int f = 0; f < 6; f++) {
                if(f >= blockTypeObject.faces.Count) continue;

                BlockFace face = blockTypeObject.faces[f];
                BlockFaceDirection direction = face.direction;

                vertsEnd = vertsStart + (uint) face.faceVerts.Count;
                trisEnd = trisStart + (uint) face.faceTris.Count;
                uvsEnd = uvsStart + (uint) face.faceUVs.Count;

                blockType = RegisterBlockFaceModel(f, direction, blockType, face, vertsStart, vertsEnd, trisStart, trisEnd);

                vertsStart += (uint) face.faceVerts.Count;
                trisStart += (uint) face.faceTris.Count;
                uvsStart += (uint) face.faceUVs.Count;
            }

            blockTypes.Add(blockType);
        }
    }

    private static BlockType RegisterBlockFaceModel(int f, BlockFaceDirection direction, BlockType blockType, BlockFace face, uint vertsStart, uint vertsEnd, uint trisStart, uint trisEnd) {
        BlockType newBlockType = blockType;

        switch(direction) {
            case BlockFaceDirection.Front: {
                newBlockType.frontVertsStart = vertsStart;
                newBlockType.frontVertsEnd = vertsEnd;

                newBlockType.frontTrisStart = trisStart;
                newBlockType.frontTrisEnd = trisEnd;

                newBlockType.cullFront = face.cullFace;
                break;
            }

            case BlockFaceDirection.Back: {
                newBlockType.backVertsStart = vertsStart;
                newBlockType.backVertsEnd = vertsEnd;

                newBlockType.backTrisStart = trisStart;
                newBlockType.backTrisEnd = trisEnd;

                newBlockType.cullBack = face.cullFace;
                break;
            }

            case BlockFaceDirection.Up: {
                newBlockType.upVertsStart = vertsStart;
                newBlockType.upVertsEnd = vertsEnd;

                newBlockType.upTrisStart = trisStart;
                newBlockType.upTrisEnd = trisEnd;
                
                newBlockType.cullUp = face.cullFace;
                break;
            }

            case BlockFaceDirection.Down: {
                newBlockType.bottomVertsStart = vertsStart;
                newBlockType.bottomVertsEnd = vertsEnd;

                newBlockType.bottomTrisStart = trisStart;
                newBlockType.bottomTrisEnd = trisEnd;

                newBlockType.cullBottom = face.cullFace;
                break;
            }

            case BlockFaceDirection.Left: {
                newBlockType.leftVertsStart = vertsStart;
                newBlockType.leftVertsEnd = vertsEnd;

                newBlockType.leftTrisStart = trisStart;
                newBlockType.leftTrisEnd = trisEnd;

                newBlockType.cullLeft = face.cullFace;
                break;
            }

            case BlockFaceDirection.Right: {
                newBlockType.rightVertsStart = vertsStart;
                newBlockType.rightVertsEnd = vertsEnd;

                newBlockType.rightTrisStart = trisStart;
                newBlockType.rightTrisEnd = trisEnd;

                newBlockType.cullRight = face.cullFace;
                break;
            }
        }

        foreach(float3 vertex in face.faceVerts) {
            blockModelData.voxelVerts.Add(vertex);
        }

        foreach(uint tri in face.faceTris) {
            blockModelData.voxelTris.Add(tri);
        }

        foreach(float2 uv in face.faceUVs) {
            blockModelData.voxelUVs.Add(uv);
        }
        
        return newBlockType;
    }

    private static void PopulateBlockTypeDictionary() {
        foreach(BlockType blockType in blockTypes) blockTypeDictionary.Add(blockType.id, blockType);
    }

    private static void DestroyBlockTypeDictionary() {
        blockTypeDictionary.Dispose();
        blockModelData.Dispose();

        disposed = true;
    }

    public static BlockMaterial GetMaterialForBlock(ushort id) {
        return blockTypeDictionary[id].material;
    }

    public static void OnDestroy() {
        if(disposed) return;
        DestroyBlockTypeDictionary();
    }
}
