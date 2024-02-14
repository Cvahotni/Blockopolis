using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Collections;

public class WorldRegionSaveLoad
{
    private static int chunkElementsSize = VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth;
    private static int chunksInRegionAmount = (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift) * (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift);

    private static byte[] chunkSaveBytes = new byte[sizeof(ushort) * chunkElementsSize];
    private static byte[] metaSaveDataBytes = new byte[sizeof(long) * 3 * chunksInRegionAmount];

    private static byte[] regionLoadBytes = new byte[sizeof(ushort) * chunkElementsSize * chunksInRegionAmount];
    private static byte[] metaLoadDataBytes = new byte[sizeof(long) * 3 * chunksInRegionAmount];

    private static byte[] chunkBuffer = new byte[sizeof(ushort) * chunkElementsSize];

    private static readonly float regionSavesPerSecond = 12f;
    private static readonly float regionLoadsPerSecond = 12f;

    private static WaitForSeconds regionSaveWaitForSeconds;
    private static WaitForSeconds regionLoadWaitForSeconds;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        regionSaveWaitForSeconds = new WaitForSeconds(1.0f / regionSavesPerSecond);
        regionLoadWaitForSeconds = new WaitForSeconds(1.0f / regionLoadsPerSecond);
    }

    public static void SaveRegion(World world, string path, WorldRegion region, long regionPos) {
        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;

        if(access == null) {
            Debug.Log("Failed to find StaticCoroutineAccess in the scene, can't save region " + path);
            return;
        }

        string directoryCheckPath = WorldStorageProperties.savesFolderName + Path.DirectorySeparatorChar + world.Name + Path.DirectorySeparatorChar + WorldStorageProperties.regionFolderName;
        string worldFolderPath = WorldStorageProperties.savesFolderName + Path.DirectorySeparatorChar + world.Name;
        string metaDataPath = path + ".meta";

        if(!Directory.Exists(WorldStorageProperties.savesFolderName)) Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
        if(!Directory.Exists(worldFolderPath)) Directory.CreateDirectory(worldFolderPath);
        if(!Directory.Exists(directoryCheckPath)) Directory.CreateDirectory(directoryCheckPath);
        
        WorldSaveLoad.EraseFileContents(path);
        WorldSaveLoad.EraseFileContents(metaDataPath);

        WorldStorage.AddWaitingSaveRegion(regionPos);
        access.StartCoroutine(SaveRegionCoroutine(path, metaDataPath, region, regionPos));
    }

    private static IEnumerator SaveRegionCoroutine(string path, string metaDataPath, WorldRegion region, long regionPos) {
        NativeArray<long> keys = region.VoxelStorageMap.GetKeyArray(Allocator.Persistent);
        NativeArray<NativeArray<ushort>> values = region.VoxelStorageMap.GetValueArray(Allocator.Persistent);

        if(keys.Length > chunksInRegionAmount) {
            Debug.LogError("Region is too large: " + keys.Length);
        }

        for(int i = 0; i < region.VoxelStorageMap.Count; i++) {
            long key = keys[i];
            NativeArray<ushort> voxelArray = values[i];

            NativeArrayExtension.ToRawBytes(voxelArray, chunkSaveBytes);

            using (var stream = new FileStream(path, FileMode.Append)) {
                using(var binaryWriter = new BinaryWriter(stream)) {
                    SetMetaDataBytes((long) key, 0, i);
                    SetMetaDataBytes((long) binaryWriter.BaseStream.Length, 1, i);
                    SetMetaDataBytes((long) chunkSaveBytes.Length + binaryWriter.BaseStream.Length, 2, i);

                    binaryWriter.Write(chunkSaveBytes, 0, chunkSaveBytes.Length);
                }
            }
        }

        using (var stream = new FileStream(metaDataPath, FileMode.Append)) {
            using(var binaryWriter = new BinaryWriter(stream)) {
                binaryWriter.Write(metaSaveDataBytes, 0, metaSaveDataBytes.Length);
            }
        }

        yield return regionSaveWaitForSeconds;

        keys.Dispose();
        values.Dispose();

        WorldStorage.IncrementRegionsSaved();
        WorldStorage.RemoveWaitingSaveRegion(regionPos);
    }

    public static void LoadRegion(string path, long regionPos) {
        string metaDataPath = path + ".meta";
        WorldRegion region = new WorldRegion(false);

        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var binaryReader = new BinaryReader(stream);

        using (var metaStream = new FileStream(metaDataPath, FileMode.Open, FileAccess.Read)) {
            using (var metaBinaryReader = new BinaryReader(metaStream)) {
                metaBinaryReader.Read(metaLoadDataBytes, 0, metaLoadDataBytes.Length);
            }
        }

        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;

        if(access == null) {
            Debug.Log("Failed to find StaticCoroutineAccess in the scene, can't load region " + path);
            return;
        }

        WorldStorage.AddWaitingLoadRegion(regionPos);
        access.StartCoroutine(LoadRegionCoroutine(region, stream, binaryReader, path, regionPos));
    }

    private static IEnumerator LoadRegionCoroutine(WorldRegion region, FileStream stream, BinaryReader binaryReader, string path, long regionPos) {
        for(int i = 0; i < chunksInRegionAmount; i++) {
            long chunkCoord = ReadMetaDataBytes(0, i);
            long chunkBytesStart = ReadMetaDataBytes(1, i);
            long chunkBytesEnd = ReadMetaDataBytes(2, i);

            if(chunkCoord == 0 && chunkBytesStart == 0 && chunkBytesEnd == 0) continue;

            long chunkBytesSize = chunkBytesEnd - chunkBytesStart;
            binaryReader.Read(regionLoadBytes, (int) chunkBytesStart, (int) chunkBytesSize);
            
            Array.Copy(regionLoadBytes, chunkBytesStart, chunkBuffer, 0, chunkBytesSize);
            NativeArray<ushort> chunkArray = new NativeArray<ushort>(chunkElementsSize, Allocator.Persistent);

            NativeArrayExtension.FromRawBytes(chunkBuffer, chunkBuffer.Length, chunkArray);
            region.AddChunk(chunkCoord, ref chunkArray);
        }

        yield return regionLoadWaitForSeconds;
        
        stream.Close();
        binaryReader.Dispose();

        WorldStorage.AddRegion(regionPos, ref region);
        WorldStorage.RemoveWaitingLoadRegion(regionPos);
    }

    private static void SetMetaDataBytes(long value, int valueOffset, int chunkOffset) {
        int offset = ((chunkOffset * 3) + valueOffset) * 8;
        
        metaSaveDataBytes[offset + 0] = (byte) value;
        metaSaveDataBytes[offset + 1] = (byte) (value >> 8);
        metaSaveDataBytes[offset + 2] = (byte) (value >> 16);
        metaSaveDataBytes[offset + 3] = (byte) (value >> 24);
        metaSaveDataBytes[offset + 4] = (byte) (value >> 32);
        metaSaveDataBytes[offset + 5] = (byte) (value >> 40);
        metaSaveDataBytes[offset + 6] = (byte) (value >> 48);
        metaSaveDataBytes[offset + 7] = (byte) (value >> 56);
    }

    private static long ReadMetaDataBytes(int valueOffset, int chunkOffset) {
        int offset = ((chunkOffset * 3) + valueOffset) * 8;

        return (((long) metaLoadDataBytes[offset] & 0xFF) | (((long) metaLoadDataBytes[offset + 1] & 0xFF) << 8)
        | (((long) metaLoadDataBytes[offset + 2] & 0xFF) << 16) | (((long) metaLoadDataBytes[offset + 3] & 0xFF) << 24)
        | (((long) metaLoadDataBytes[offset + 4] & 0xFF) << 32) | (((long) metaLoadDataBytes[offset + 5] & 0xFF) << 40)
        | (((long) metaLoadDataBytes[offset + 6] & 0xFF) << 48) | (((long) metaLoadDataBytes[offset + 7] & 0xFF) << 56));
    }
}
