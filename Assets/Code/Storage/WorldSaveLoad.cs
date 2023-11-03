using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Unity.Collections;

public class WorldSaveLoad
{
    private static byte[] regionBytes = new byte[65536 * 16];
    private static byte[] chunkBytes = new byte[65536];

    public static void SaveRegion(World world, string path, WorldRegion region) {
        string directoryCheckPath = WorldStorageProperties.savesFolderName + Path.DirectorySeparatorChar + world.Name + Path.DirectorySeparatorChar + WorldStorageProperties.regionFolderName;
        string worldFolderPath = WorldStorageProperties.savesFolderName + Path.DirectorySeparatorChar + world.Name;

        if(!Directory.Exists(WorldStorageProperties.savesFolderName)) Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
        if(!Directory.Exists(worldFolderPath)) Directory.CreateDirectory(worldFolderPath);
        if(!Directory.Exists(directoryCheckPath)) Directory.CreateDirectory(directoryCheckPath);

        string metaDataPath = GetMetadataPath(path);
        
        EraseFileContents(path);
        EraseFileContents(metaDataPath);

        foreach(var regionPair in region.VoxelStorageMap) {
            long key = regionPair.Key;

            NativeArray<ushort> voxelArray = regionPair.Value;

            NativeList<EncodedVoxelMapEntry> array = new NativeList<EncodedVoxelMapEntry>(Allocator.Persistent);
            ChunkEncoderDecoder.Encode(voxelArray, array);

            byte[] bytes = NativeArrayExtension.ToRawBytes<EncodedVoxelMapEntry>(array);
            array.Dispose();

            using (var stream = new FileStream(path, FileMode.Append)) {
                WriteRegionMetaData(metaDataPath, key, stream.Length, bytes.Length);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        Debug.Log("Saved region: " + path);
    }

    private static void WriteRegionMetaData(string path, long key, long currentByteCount, long bytesCount) {
        using (StreamWriter writer = new StreamWriter(path, append: true)) writer.WriteLine(key + "." + currentByteCount + "." + bytesCount);
    }

    public static void EraseFileContents(string path) {
        FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

        stream.SetLength(0);
        stream.Close();
    }

    public static WorldRegion LoadRegion(string path) { 
        Debug.Log("Attempting Region Load: " + path);
        ChunkBuilder chunkBuilder = ChunkBuilder.Instance;

        Dictionary<long, NativeArray<ushort>> voxelStorageMap = new Dictionary<long, NativeArray<ushort>>();

        string metadataPath = GetMetadataPath(path);
        NativeList<EncodedChunkMeta> decodedChunkMetadata = DecodeRegionMetaData(metadataPath);

        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
            fileStream.Read(regionBytes, 0, (int) fileStream.Length);

            foreach(EncodedChunkMeta meta in decodedChunkMetadata) {
                long key = meta.key;
                long currentByteCount = meta.currentByteCount;
                long bytesCount = meta.bytesCount;

                Array.Copy(regionBytes, currentByteCount, chunkBytes, 0, bytesCount);

                NativeArray<EncodedVoxelMapEntry> encodedVoxelMap = NativeArrayExtension.FromRawBytes<EncodedVoxelMapEntry>(chunkBytes, (int) bytesCount, Allocator.Persistent);
                NativeList<EncodedVoxelMapEntry> encodedVoxelMapList = new NativeList<EncodedVoxelMapEntry>(Allocator.Persistent);

                encodedVoxelMapList.AddRange(encodedVoxelMap);
                NativeArray<ushort> voxelMap = ChunkEncoderDecoder.Decode(encodedVoxelMapList, ref chunkBuilder);

                voxelStorageMap.Add(key, voxelMap);
                encodedVoxelMap.Dispose();
            }
        }

        decodedChunkMetadata.Dispose();

        Debug.Log("Loaded region: " + path);
        return new WorldRegion(voxelStorageMap);
    }

    private static NativeList<EncodedChunkMeta> DecodeRegionMetaData(string metaDataPath) {
        NativeList<EncodedChunkMeta> metadata = new NativeList<EncodedChunkMeta>(Allocator.Persistent);
        string line;

        using (StreamReader reader = new StreamReader(metaDataPath)) while((line = reader.ReadLine()) != null) {
            string[] splitString = line.Split(".");

            long key = long.Parse(splitString[0]);
            long currentByteCount = long.Parse(splitString[1]);
            long bytesCount = long.Parse(splitString[2]);

            metadata.Add(new EncodedChunkMeta(key, currentByteCount, bytesCount));
        }

        return metadata;
    }

    public static void SaveWorldInfo(World world) {
        string path = CheckWorldSaveFile(world, WorldStorageProperties.worldInfoFileName);

        using (StreamWriter writer = new StreamWriter(path, append: true)) {
            writer.WriteLine("name: " + world.Name);
            writer.WriteLine("seed: " + world.Seed);
        }
    }

    public static void SaveWorldInventory(World world, Inventory inventory) {
        string path = CheckWorldSaveFile(world, WorldStorageProperties.inventoryFileName);

        using (StreamWriter writer = new StreamWriter(path, append: true)) {
            for(int i = 0; i < inventory.Slots.Length; i++) {
                UiItemSlot uiSlot = inventory.Slots[i];
                ItemSlot itemSlot = uiSlot.ItemSlot;
                ItemStack stack = itemSlot.Stack;

                string line = GetSlotLine(i);

                string idLine = line + ".id." + stack.ID;
                string amountLine = line + ".amount." + stack.Amount;

                writer.WriteLine(idLine);
                writer.WriteLine(amountLine);
            }
        }
    }

    public static void LoadWorldInventory(World world, Inventory inventory) {
        string loadPath = CheckWorldLoadFile(world.Name, WorldStorageProperties.inventoryFileName);
        string line;

        int inventorySize = inventory.Slots.Length;
        
        bool readID = false;
        bool readAmount = false;

        ushort currentID = 0;
        ushort currentAmount = 0;

        int currentIndex = -1;

        using (StreamReader reader = new StreamReader(loadPath)) while((line = reader.ReadLine()) != null) {
            string[] splitString = line.Split(".");

            int index = Int32.Parse(splitString[1]);
            string typeData = splitString[3];

            if(readID && readAmount) {
                UiItemSlot uiSlot = inventory.Slots[currentIndex];
                ItemSlot itemSlot = uiSlot.ItemSlot;

                itemSlot.Stack = new ItemStack(currentID, currentAmount);
                itemSlot.UpdateEmptyStatus();

                uiSlot.UpdateSlot(true);

                readID = false;
                readAmount = false;
            }

            if(line.Contains("id")) {
                currentID = UInt16.Parse(typeData);
                readID = true;
            }

            if(line.Contains("amount")) {
                currentAmount = UInt16.Parse(typeData);
                readAmount = true;
            }

            currentIndex = index;
        }
    }

    private static string GetSlotLine(int i) {
        return "slot." + i;
    }

    private static string CheckWorldSaveFile(World world, string fileName) {
        string path = WorldStorageProperties.savesFolderName + world.Name + fileName;
        if(DoesFileExist(path)) EraseFileContents(path);
        
        else {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + world.Name);
        }

        return path;
    }

    public static string CheckWorldLoadFile(string path, string fileName) {
        string loadPath = WorldStorageProperties.savesFolderName + path + fileName;

        if(!DoesFileExist(WorldStorageProperties.savesFolderName)) {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + path);
        }

        if(!File.Exists(loadPath)) {
            using (File.Create(loadPath));
        }

        return loadPath;
    }

    public static World LoadWorldInfo(string path) {
        string line;

        string name = "";
        int seed = -1;

        string loadPath = CheckWorldLoadFile(path, WorldStorageProperties.worldInfoFileName);

        using (StreamReader reader = new StreamReader(loadPath)) while((line = reader.ReadLine()) != null) {
            string[] splitString = line.Split(": ");

            if(line.Contains("name")) name = splitString[1];
            if(line.Contains("seed")) seed = int.Parse(splitString[1]);
        }

        return new World(name, seed);
    }

    public static bool DoesFileExist(string path) {
        return File.Exists(path);
    }

    private static string GetMetadataPath(string path) {
        return path + WorldStorageProperties.metadataExtension;
    }
}
