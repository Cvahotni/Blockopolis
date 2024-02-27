using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System.IO;
using System.Linq;

public static class WorldStorage
{
    private static Dictionary<long, WorldRegion> regionMap = new Dictionary<long, WorldRegion>();
    private static int regionsSaved = 0;

    private static float regionSavesPerSecond = 64;
    private static float regionSavesPerSecondMultiplier = 16;

    private static WaitForSeconds saveRegionsWaitForSeconds;
    private static WaitForSeconds saveRegionsQuicklyWaitForSeconds;

    private static List<long> waitingLoadRegions = new List<long>();
    private static List<long> waitingSaveRegions = new List<long>();

    public static int RegionsSaved { get { return regionsSaved; }}

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        saveRegionsWaitForSeconds = new WaitForSeconds(1.0f / regionSavesPerSecond);
        saveRegionsQuicklyWaitForSeconds = new WaitForSeconds(1.0f / (regionSavesPerSecond * regionSavesPerSecondMultiplier));
    }

    public static void AddChunk(long coord, ref NativeArray<ushort> voxelMap) {
        long regionPos = RegionPositionHelper.ChunkPosToRegionPos(coord);

        if(!DoesRegionExist(regionPos)) {
            CreateRegionAt(regionPos);
        }

        WorldRegion region = GetRegionAtChunkCoord(coord);
        region.AddChunk(coord, ref voxelMap);
    }

    public static void SetChunk(long coord, ref NativeArray<ushort> voxelMap) {
        WorldRegion region = GetRegionAtChunkCoord(coord);
        region.SetChunk(coord, ref voxelMap);
    }

    public static NativeArray<ushort> GetChunk(long coord) {
        WorldRegion region = GetRegionAtChunkCoord(coord);
        return region.GetChunk(coord);
    }

    public static void RemoveChunk(object sender, long coord) {
        RemoveChunk(coord);
    }

    private static void RemoveChunk(long coord) {
        WorldRegion region = GetRegionAtChunkCoord(coord);
        region.RemoveChunk(coord);
    }

    public static void IncrementRegionsSaved() {
        regionsSaved++;
    }

    public static void ResetRegionsSaved() {
        regionsSaved = 0;
    }

    public static int RegionMapSize() {
        return regionMap.Count;
    }

    public static bool IsWaitingForLoadRegion(long regionPos) {
        return waitingLoadRegions.Contains(regionPos);
    }

    public static bool IsWaitingForSaveRegion(long regionPos) {
        return waitingSaveRegions.Contains(regionPos);
    }

    public static void AddWaitingLoadRegion(long regionPos) {
        waitingLoadRegions.Add(regionPos);
    }

    public static void AddWaitingSaveRegion(long regionPos) {
        waitingSaveRegions.Add(regionPos);
    }

    public static void RemoveWaitingLoadRegion(long regionPos) {
        waitingLoadRegions.Remove(regionPos);
    }

    public static void RemoveWaitingSaveRegion(long regionPos) {
        waitingSaveRegions.Remove(regionPos);
    }

    public static bool IsWaitingForAnyLoadRegion() {
        return waitingLoadRegions.Count > 0;
    }

    public static bool IsWaitingForAnySaveRegion() {
        return waitingSaveRegions.Count > 0;
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

    public static void SaveRegions(World world, bool saveQuickly) {
        StaticCoroutineAccess access = StaticCoroutineAccess.Instance;

        if(access == null) {
            Debug.Log("Failed to find StaticCoroutineAccess in the scene, can't save regions.");
            return;
        }

        access.StartCoroutine(SaveRegionsCoroutine(world, saveQuickly));
    }

    private static IEnumerator SaveRegionsCoroutine(World world, bool saveQuickly) {
        int i = regionMap.Count - 1;
        int j = 0;

        while(i >= 0) {
            i = regionMap.Count - 1 - j;
            if(i >= regionMap.Count || i < 0) break;

            var item = regionMap.ElementAt(i);

            long regionPos = item.Key;
            WorldRegion regionData = item.Value;

            yield return saveQuickly ? saveRegionsQuicklyWaitForSeconds : saveRegionsWaitForSeconds;
            if(IsWaitingForAnySaveRegion()) continue;

            string regionName = GetRegionPosName(world, regionPos);
            WorldRegionSaveLoad.SaveRegion(world, regionName, regionData, regionPos);

            j++;
        }
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

    public static void AddRegion(long regionPos, ref WorldRegion region) {
        if(DoesRegionExist(regionPos)) return;
        regionMap.Add(regionPos, region);
    }

    public static void LoadRegionToMap(World world, long regionPos) {
        WorldRegionSaveLoad.LoadRegion(GetRegionPosName(world, regionPos), regionPos);
    }

    public static bool IsRegionSaved(World world, long regionPos) {
        return WorldSaveLoad.DoesFileExist(GetRegionPosName(world, regionPos));
    }

    private static string GetRegionPosName(World world, long regionPos) {
        return WorldStorageProperties.savesFolderName + world.Name + Path.DirectorySeparatorChar + "region" + Path.DirectorySeparatorChar + regionPos.ToString() + ".dat";
    }

    public static void Clear() {
        Destroy();
        regionMap.Clear();

        Debug.Log("Cleared region data.");
    }

    private static void Destroy() {
        int disposedCount = 0;

        foreach(var regionPair in regionMap) disposedCount += regionPair.Value.Destroy();
        Debug.Log("Disposed unused region data: " + disposedCount);
    }
}
