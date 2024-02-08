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
    private static BlockModelData blockModelData;

    private static Dictionary<int, BlockFaceIndexEntry> blockFaceEntries = new Dictionary<int, BlockFaceIndexEntry>();
    private static NativeParallelHashMap<byte, BlockStateModel> blockModelDictionary;

    private static List<byte> addedBlockModels = new List<byte>();
    private static bool disposed = false;

    private static uint vertsStart = 0;
    private static uint vertsEnd = 0;

    private static uint trisStart = 0;
    private static uint trisEnd = 0;

    public static NativeParallelHashMap<ushort, BlockState> BlockStateDictionary {
        get { return blockStateDictionary; }
    }

    public static NativeParallelHashMap<byte, BlockStateModel> BlockModelDictionary {
        get { return blockModelDictionary; }
    }

    public static BlockModelData BlockModelData {
        get { return blockModelData; }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        CreateNativeCollections();
        LoadBlockStatesFromFolder();
        PopulateBlockStates();
        PopulateBlockStateDictionary();
        RegisterBlockStateModels();
    }

    private static void CreateNativeCollections() {
        blockStateDictionary = new NativeParallelHashMap<ushort, BlockState>(1, Allocator.Persistent);
        blockModelDictionary = new NativeParallelHashMap<byte, BlockStateModel>(1, Allocator.Persistent);

        blockModelData = new BlockModelData();
        blockModelData.voxelVerts = new NativeList<float3>(Allocator.Persistent);
        blockModelData.voxelTris = new NativeList<uint>(Allocator.Persistent);
        blockModelData.voxelUVs = new NativeList<float2>(Allocator.Persistent);
    }

    private static void LoadBlockStatesFromFolder() {
        Object[] objects = Resources.LoadAll("Block States");
        foreach(Object currentObject in objects) blockStateObjects.Add((BlockStateObject) currentObject);
    }

    private static void PopulateBlockStates() {
        foreach(BlockStateObject blockStateObject in blockStateObjects) {
            BlockState blockState = new BlockState();

            blockState.id = blockStateObject.id;
            blockState.variant = blockStateObject.variant;

            blockState.solid = blockStateObject.solid;
            blockState.transparent = blockStateObject.transparent;

            blockState.material = blockStateObject.material;
            blockState.hardness = blockStateObject.hardness;

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

    private static void RegisterBlockStateModels() {
        foreach(BlockStateObject blockStateObject in blockStateObjects) {
            byte id = blockStateObject.model.id;
            if(addedBlockModels.Contains(id)) continue;

            BlockStateModel model = RegisterBlockModel(blockStateObject.model);

            blockModelDictionary.Add(id, model);
            addedBlockModels.Add(id);
        }
    } 

    private static BlockStateModel RegisterBlockModel(BlockStateModelObject model) {
        BlockStateModel blockModel = new BlockStateModel();

        for(int f = 0; f < 6; f++) {
            if(f >= model.faces.Count) continue;

            BlockFace face = model.faces[f];
            BlockFaceDirection direction = face.direction;

            vertsEnd = vertsStart + (uint) face.faceVerts.Count;
            trisEnd = trisStart + (uint) face.faceTris.Count;

            blockModel = RegisterBlockFaceModel(direction, blockModel, face, vertsStart, vertsEnd, trisStart, trisEnd, true);
            BlockFaceIndexEntry foundEntry = new BlockFaceIndexEntry(vertsStart, vertsEnd, trisStart, trisEnd);

            if(blockFaceEntries.ContainsKey(face.id)) continue;
            blockFaceEntries.Add(face.id, foundEntry);

            vertsStart += (uint) face.faceVerts.Count;
            trisStart += (uint) face.faceTris.Count;
        }

        return blockModel;
    }

    private static BlockStateModel RegisterBlockFaceModel(BlockFaceDirection direction, BlockStateModel blockModel, BlockFace face, uint currentVertsStart, uint currentVertsEnd, uint currentTrisStart, uint currentTrisEnd, bool addModelData) {
        BlockStateModel blockStateModel = blockModel;
        
        switch(direction) {
            case BlockFaceDirection.Front: {
                blockStateModel.frontVertsStart = currentVertsStart;
                blockStateModel.frontVertsEnd = currentVertsEnd;

                blockStateModel.frontTrisStart = currentTrisStart;
                blockStateModel.frontTrisEnd = currentTrisEnd;

                blockStateModel.cullFront = face.cullFace;
                break;
            }

            case BlockFaceDirection.Back: {
                blockStateModel.backVertsStart = currentVertsStart;
                blockStateModel.backVertsEnd = currentVertsEnd;

                blockStateModel.backTrisStart = currentTrisStart;
                blockStateModel.backTrisEnd = currentTrisEnd;

                blockStateModel.cullBack = face.cullFace;
                break;
            }

            case BlockFaceDirection.Up: {
                blockStateModel.upVertsStart = currentVertsStart;
                blockStateModel.upVertsEnd = currentVertsEnd;

                blockStateModel.upTrisStart = currentTrisStart;
                blockStateModel.upTrisEnd = currentTrisEnd;
                
                blockStateModel.cullUp = face.cullFace;
                break;
            }

            case BlockFaceDirection.Down: {
                blockStateModel.bottomVertsStart = currentVertsStart;
                blockStateModel.bottomVertsEnd = currentVertsEnd;

                blockStateModel.bottomTrisStart = currentTrisStart;
                blockStateModel.bottomTrisEnd = currentTrisEnd;

                blockStateModel.cullBottom = face.cullFace;
                break;
            }

            case BlockFaceDirection.Left: {
                blockStateModel.leftVertsStart = currentVertsStart;
                blockStateModel.leftVertsEnd = currentVertsEnd;

                blockStateModel.leftTrisStart = currentTrisStart;
                blockStateModel.leftTrisEnd = currentTrisEnd;

                blockStateModel.cullLeft = face.cullFace;
                break;
            }

            case BlockFaceDirection.Right: {
                blockStateModel.rightVertsStart = currentVertsStart;
                blockStateModel.rightVertsEnd = currentVertsEnd;

                blockStateModel.rightTrisStart = currentTrisStart;
                blockStateModel.rightTrisEnd = currentTrisEnd;

                blockStateModel.cullRight = face.cullFace;
                break;
            }
        }

        if(addModelData) {
            foreach(float3 vertex in face.faceVerts) {
                blockModelData.voxelVerts.Add(vertex);
            }

            foreach(uint tri in face.faceTris) {
                blockModelData.voxelTris.Add(tri);
            }

            foreach(float2 uv in face.faceUVs) {
                blockModelData.voxelUVs.Add(uv);
            }
        }

        return blockStateModel;
    }

    private static void PopulateBlockStateDictionary() {
        foreach(BlockState blockState in blockStates) blockStateDictionary.Add(BlockIDHelper.Pack(blockState.id, blockState.variant), blockState);
    }

    private static void DestroyDictionaries() {
        blockStateDictionary.Dispose();
        blockModelData.Dispose();
        blockModelDictionary.Dispose();

        disposed = true;
    }

    public static BlockMaterial GetMaterialForBlock(byte id) {
        return blockStateDictionary[BlockIDHelper.Pack(id, 0)].material;
    }

    public static void OnDestroy() {
        if(disposed) return;
        DestroyDictionaries();
    }
}
