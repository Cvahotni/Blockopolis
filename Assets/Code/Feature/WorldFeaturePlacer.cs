using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[DefaultExecutionOrder(-800)]
[RequireComponent(typeof(WorldFeatures))]
[RequireComponent(typeof(EndlessTerrain))]
public class WorldFeaturePlacer : MonoBehaviour
{
    public static WorldFeaturePlacer Instance { get; private set; }
    private List<long> addedFeatureChunks = new List<long>();

    private WorldEventSystem worldEventSystem;
    private WorldFeatures worldFeatures;
    private EndlessTerrain endlessTerrain;

    private WaitForSeconds shortWait;
    private bool buildFeaturesQuickly = true;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        shortWait = new WaitForSeconds(1.0f / GameSettings.FeaturesPerSecond);

        worldEventSystem = WorldEventSystem.Instance;
        worldFeatures = WorldFeatures.Instance;
        endlessTerrain = EndlessTerrain.Instance;
    }

    public void PlaceFeatures(object sender, int3 data) {
        StartCoroutine(PlaceFeatures(data.x, data.y, data.z));
    }

    public void RemoveFeatures(object sender, int3 data) {
        for(int i = addedFeatureChunks.Count - 1; i >= 0; i--) {
            long coord = addedFeatureChunks[i];

            if(endlessTerrain.IsFeatureChunkOutOfRange(coord)) {
                addedFeatureChunks.RemoveAt(i);
            }
        }

        worldFeatures.Clear();
    }

    private IEnumerator PlaceFeatures(int originX, int originZ, int addedViewDistance) {
        for(int x = -addedViewDistance + originX; x < addedViewDistance + originX; x++) {
            for(int z = -addedViewDistance + originZ; z < addedViewDistance + originZ; z++) {
                long coord = ChunkPositionHelper.GetChunkPos(x, z);
                if(addedFeatureChunks.Contains(coord)) continue;

                if(!buildFeaturesQuickly) yield return shortWait;
                if(endlessTerrain.IsFeatureChunkOutOfRange(coord)) continue;
                if(WorldStorage.DoesChunkExist(coord)) continue;

                foreach(var pair in FeatureRegistry.FeatureSettings) {
                    ushort id = pair.Key;
                    PlaceFeaturesOfType(x, z, id);
                }

                addedFeatureChunks.Add(coord);
            }
        }

        worldEventSystem.InvokeFinishedBuildingFeatures();
    }

    private void PlaceFeaturesOfType(int x, int z, ushort type) {
        FeatureSettings settings = FeatureRegistry.FeatureSettings[type];
        UnityEngine.Random.InitState(WorldHandler.CurrentWorld.Seed + ((x + 800) * z) + (type * 404));

        for(int i = 0; i < settings.featuresPerChunk; i++) {
            int wx = x * VoxelProperties.chunkWidth;
            int wz = z * VoxelProperties.chunkWidth;

            int randomX = UnityEngine.Random.Range(wx, wx + VoxelProperties.chunkWidth);
            int randomZ = UnityEngine.Random.Range(wz, wz + VoxelProperties.chunkWidth);

            int y = 0;

            switch(settings.placeType) {
                case FeaturePlaceType.Surface:
                case FeaturePlaceType.SurfaceHeight: { 
                    y = GetSurfaceFeatureY(randomX, randomZ);
                    break;
                }

                case FeaturePlaceType.Underground: { 
                    y = UnityEngine.Random.Range(settings.minY, settings.maxY);
                    break;
                }

                default: {
                    y = VoxelProperties.chunkHeight;
                    break;
                }
            }

            if(y < settings.minY || y > settings.maxY) continue;
            if(UnityEngine.Random.Range(0.0f, 100.0f) > settings.spawnChance) continue;

            worldFeatures.Add(new FeaturePlacement(randomX, y, randomZ, type));
        }
    }

    private int GetSurfaceFeatureY(int randomX, int randomZ) {
        return (int) Noise.Get2DNoise(
                WorldUtil.GetRealWorldX(randomX), WorldUtil.GetRealWorldZ(randomZ), 
                endlessTerrain.NoiseOffset.x, endlessTerrain.NoiseOffset.y, 
                endlessTerrain.NativeFrequencies, endlessTerrain.NativeAmplitudes) - 15;
    }

    public void EnableBuildFeaturesQuickly() {
        buildFeaturesQuickly = true;
    }

    public void DisableBuildFeaturesQuickly(object sender, EventArgs e) {
        buildFeaturesQuickly = false;
    }

    public void UpdateBuildFeaturesQuickly(object sender, bool value) {
        buildFeaturesQuickly = !value;
    }
}
