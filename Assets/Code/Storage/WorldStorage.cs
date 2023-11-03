using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.IO;

public static class WorldStorage
{
    private static Dictionary<long, WorldRegion> regionMap = new Dictionary<long, WorldRegion>();
    private static List<long> awaitingRegions = new List<long>();

    public static void AddChunk(long coord, NativeArray<ushort> voxelMap) {
        WorldRegion region = GetRegionAtChunkCoord(coord);
        region.AddChunk(coord, voxelMap);
    }

    public static void SetChunk(long coord, NativeArray<ushort> voxelMap) {
        WorldRegion region = GetRegionAtChunkCoord(coord);
        region.SetChunk(coord, voxelMap);
    }

    public static NativeArray<ushort> GetChunk(long coord) {
        WorldRegion region = GetRegionAtChunkCoord(coord);
        return region.GetChunk(coord);
    }

    public static bool DoesChunkExist(long coord) {
        long regionPos = RegionPositionHelper.ChunkPosToRegionPos(coord);
        if(!DoesRegionExist(regionPos)) return false;

        WorldRegion region = regionMap[regionPos];
        return region.DoesChunkExist(coord);
    }

    public static void SetRegion(long regionCoord, WorldRegion region) {
        regionMap[regionCoord].Destroy();
        regionMap[regionCoord] = region;
    }

    public static void SaveRegions(World world) {
        foreach(var regionPair in regionMap) {
            long regionPos = regionPair.Key;
            WorldRegion regionData = regionPair.Value;

            string regionName = GetRegionPosName(world, regionPos);
            WorldRegionSaveLoad.SaveRegion(world, regionName, regionData);
        }
    }

    public static bool IsWaitingForRegion(long regionCoord) {
        return awaitingRegions.Contains(regionCoord);
    }

    public static bool DoesRegionExist(long regionCoord) {
        return regionMap.ContainsKey(regionCoord);
    }

    public static WorldRegion GetRegionAt(long regionPos) {
        return regionMap[regionPos];
    }

    private static WorldRegion GetRegionAtChunkCoord(long chunkCoord) {
        long regionPos = RegionPositionHelper.ChunkPosToRegionPos(chunkCoord);
        return GetRegionAt(regionPos);
    }

    public static void CreateRegionAt(long regionPos) {
        WorldRegion worldRegion = new WorldRegion(false);
        regionMap.Add(regionPos, worldRegion);
    }

    public static void LoadRegionToMap(World world, long regionPos) {
        WorldRegion loadedRegion = WorldRegionSaveLoad.LoadRegion(GetRegionPosName(world, regionPos));
        regionMap.Add(regionPos, loadedRegion);

        awaitingRegions.Remove(regionPos);
    }

    private static bool DoesRegionsFileExist(string regionsFile) {
        return WorldSaveLoad.DoesFileExist(regionsFile);
    }

    public static bool IsRegionSaved(World world, long regionPos) {
        return WorldSaveLoad.DoesFileExist(GetRegionPosName(world, regionPos));
    }

    private static string GetRegionPosName(World world, long regionPos) {
        return WorldStorageProperties.savesFolderName + world.Name + Path.DirectorySeparatorChar + "region" + Path.DirectorySeparatorChar + regionPos.ToString() + ".dat";
    }

    public static void Destroy() {
        int disposedCount = 0;

        foreach(var regionPair in regionMap) disposedCount += regionPair.Value.Destroy();
        Debug.Log("Disposed unused region data: " + disposedCount);
    }
}
