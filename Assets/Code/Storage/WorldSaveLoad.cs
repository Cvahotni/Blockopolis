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
        string path = WorldStorageProperties.savesFolderName + world.Name + WorldStorageProperties.worldInfoFileName;
        
        if(DoesFileExist(path)) EraseFileContents(path);
        
        else {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + world.Name);
        }

        using (StreamWriter writer = new StreamWriter(path, append: true)) {
            writer.WriteLine("name: " + world.Name);
            writer.WriteLine("seed: " + world.Seed);
        }
    }

    public static World LoadWorldInfo(string path) {
        string line;

        string name = "";
        int seed = -1;

        string loadPath = WorldStorageProperties.savesFolderName + path + WorldStorageProperties.worldInfoFileName;

        if(!DoesFileExist(WorldStorageProperties.savesFolderName)) {
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
            Directory.CreateDirectory(WorldStorageProperties.savesFolderName + path);
        }

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
