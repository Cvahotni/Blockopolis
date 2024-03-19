using System.Collections;
using UnityEngine;
using System;
using System.IO;
using Unity.Collections;

public class WorldRegionSaveLoad
{
    private static int chunkElementsSize = VoxelProperties.chunkWidth * VoxelProperties.chunkHeight * VoxelProperties.chunkWidth;
    private static int chunksInRegionAmount = (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift) * (VoxelProperties.regionWidth >> VoxelProperties.chunkBitShift);

    private static byte[] chunkVoxelSaveBytes = new byte[(sizeof(ushort) * chunkElementsSize)];
    private static byte[] chunkVoxelLoadBytes = new byte[(sizeof(ushort) * chunkElementsSize)];

    private static byte[] chunkLightSaveBytes = new byte[(sizeof(byte) * chunkElementsSize)];
    private static byte[] chunkLightLoadBytes = new byte[(sizeof(byte) * chunkElementsSize)];

    private static byte[] chunkVoxelBuffer = new byte[sizeof(ushort) * chunkElementsSize];
    private static byte[] chunkLightBuffer = new byte[sizeof(byte) * chunkElementsSize];

    private static byte[] metaSaveDataBytes = new byte[sizeof(long)];
    private static byte[] metaLoadDataBytes = new byte[sizeof(long)];

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

        if(!Directory.Exists(WorldStorageProperties.savesFolderName)) Directory.CreateDirectory(WorldStorageProperties.savesFolderName);
        if(!Directory.Exists(worldFolderPath)) Directory.CreateDirectory(worldFolderPath);
        if(!Directory.Exists(directoryCheckPath)) Directory.CreateDirectory(directoryCheckPath);
        
        WorldSaveLoad.EraseFileContents(path);

        WorldStorage.AddWaitingSaveRegion(regionPos);
        access.StartCoroutine(SaveRegionCoroutine(path, region, regionPos));
    }

    private static IEnumerator SaveRegionCoroutine(string path, WorldRegion region, long regionPos) {
        NativeArray<long> keys = region.VoxelStorageMap.GetKeyArray(Allocator.Persistent);

        NativeArray<NativeArray<ushort>> voxelValues = region.VoxelStorageMap.GetValueArray(Allocator.Persistent);
        NativeArray<NativeArray<byte>> lightValues = region.LightStorageMap.GetValueArray(Allocator.Persistent);

        if(keys.Length > chunksInRegionAmount) {
            Debug.LogError("Region is too large: " + keys.Length);
        }

        for(int i = 0; i < region.VoxelStorageMap.Count; i++) {
            long chunkCoord = keys[i];

            NativeArray<ushort> voxelArray = voxelValues[i];
            NativeArray<byte> lightArray = lightValues[i];

            NativeArrayExtension.ToRawBytes(voxelArray, chunkVoxelSaveBytes);
            NativeArrayExtension.ToRawBytes(lightArray, chunkLightSaveBytes);

            using (var stream = new FileStream(path, FileMode.Append)) {
                using(var binaryWriter = new BinaryWriter(stream)) {
                    SetMetaDataBytes(chunkCoord);
                    long newRegionPos = RegionPositionHelper.ChunkPosToRegionPos(chunkCoord);

                    binaryWriter.Write(metaSaveDataBytes, 0, metaSaveDataBytes.Length);
                }
            }

            using(var stream = new FileStream(path, FileMode.Append)) {
                using(var binaryWriter = new BinaryWriter(stream)) {
                    binaryWriter.Write(chunkVoxelSaveBytes, 0, chunkVoxelSaveBytes.Length);
                }
            }

            using(var stream = new FileStream(path, FileMode.Append)) {
                using(var binaryWriter = new BinaryWriter(stream)) {
                    binaryWriter.Write(chunkLightSaveBytes, 0, chunkLightSaveBytes.Length);
                }
            }
        }

        yield return regionSaveWaitForSeconds;

        keys.Dispose();

        voxelValues.Dispose();
        lightValues.Dispose();

        WorldStorage.IncrementRegionsSaved();
        WorldStorage.RemoveWaitingSaveRegion(regionPos);
    }

    public static void LoadRegion(string path, long regionPos) {
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read);

        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;

        if(access == null) {
            Debug.Log("Failed to find StaticCoroutineAccess in the scene, can't load region " + path);
            return;
        }

        WorldStorage.AddWaitingLoadRegion(regionPos);
        access.StartCoroutine(LoadRegionCoroutine(stream, path, regionPos));
    }

    private static IEnumerator LoadRegionCoroutine(FileStream stream, string path, long regionPos) {
        var binaryReader = new BinaryReader(stream);
        WorldRegion region = new WorldRegion(false);

        for(int i = 0; i < chunksInRegionAmount; i++) {
            int metaStartBytes = i * ((sizeof(ushort) * chunkElementsSize) + sizeof(byte) + chunkElementsSize + 8);
            int metaEndBytes = metaStartBytes + 8;

            int chunkBytesStart = metaEndBytes;
            int chunkBytesEnd = chunkBytesStart + (sizeof(ushort) * chunkElementsSize);

            int lightBytesStart = chunkBytesEnd;
            int lightBytesEnd = lightBytesStart + (sizeof(byte) * chunkElementsSize);

            long metaBytesSize = metaEndBytes - metaStartBytes;
            long chunkBytesSize = chunkBytesEnd - chunkBytesStart;
            long lightBytesSize = lightBytesEnd - lightBytesStart;

            binaryReader.Read(metaLoadDataBytes, 0, (int) metaBytesSize);
            binaryReader.Read(chunkVoxelLoadBytes, 0, (int) chunkBytesSize);
            binaryReader.Read(chunkLightLoadBytes, 0, (int) lightBytesSize);

            long chunkCoord = ReadMetaDataBytes();
            
            Array.Copy(chunkVoxelLoadBytes, 0, chunkVoxelBuffer, 0, chunkBytesSize);
            Array.Copy(chunkLightLoadBytes, 0, chunkLightBuffer, 0, lightBytesSize);
            
            NativeArray<ushort> chunkDataArray = new NativeArray<ushort>(chunkElementsSize, Allocator.Persistent);
            NativeArray<byte> lightDataArray = new NativeArray<byte>(chunkElementsSize, Allocator.Persistent);

            NativeArrayExtension.FromRawBytes(chunkVoxelBuffer, chunkVoxelBuffer.Length, chunkDataArray);
            NativeArrayExtension.FromRawBytes(chunkLightBuffer, chunkLightBuffer.Length, lightDataArray);

            region.AddChunk(chunkCoord, ref chunkDataArray, ref lightDataArray);
        }

        yield return regionLoadWaitForSeconds;
        
        stream.Close();
        binaryReader.Dispose();

        WorldStorage.AddRegion(regionPos, ref region);
        WorldStorage.RemoveWaitingLoadRegion(regionPos);
    }

    private static void SetMetaDataBytes(long value) {
        metaSaveDataBytes[0] = (byte) value;
        metaSaveDataBytes[1] = (byte) (value >> 8);
        metaSaveDataBytes[2] = (byte) (value >> 16);
        metaSaveDataBytes[3] = (byte) (value >> 24);
        metaSaveDataBytes[4] = (byte) (value >> 32);
        metaSaveDataBytes[5] = (byte) (value >> 40);
        metaSaveDataBytes[6] = (byte) (value >> 48);
        metaSaveDataBytes[7] = (byte) (value >> 56);
    }

    private static long ReadMetaDataBytes() {
        return ((long) metaLoadDataBytes[0] & 0xFF) | (((long) metaLoadDataBytes[1] & 0xFF) << 8)
        | (((long) metaLoadDataBytes[2] & 0xFF) << 16) | (((long) metaLoadDataBytes[3] & 0xFF) << 24)
        | (((long) metaLoadDataBytes[4] & 0xFF) << 32) | (((long) metaLoadDataBytes[5] & 0xFF) << 40)
        | (((long) metaLoadDataBytes[6] & 0xFF) << 48) | (((long) metaLoadDataBytes[7] & 0xFF) << 56);
    }
}
