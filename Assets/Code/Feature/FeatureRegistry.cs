using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class FeatureRegistry
{
    private static NativeParallelHashMap<FeaturePlacement, ushort> featureData;
    private static NativeParallelHashMap<ushort, FeatureSettings> featureSettings;

    public static NativeParallelHashMap<FeaturePlacement, ushort> FeatureData {
        get { return featureData; }
    }

    public static NativeParallelHashMap<ushort, FeatureSettings> FeatureSettings {
        get { return featureSettings; }
    }

    private static bool disposed = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Start() {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        featureData = new NativeParallelHashMap<FeaturePlacement, ushort>(1, Allocator.Persistent);
        featureSettings = new NativeParallelHashMap<ushort, FeatureSettings>(1, Allocator.Persistent);

        WorldFeatureLoad.Load();

        watch.Stop();
        float timeTaken = watch.ElapsedTicks * 1000 / System.Diagnostics.Stopwatch.Frequency;

        Dictionary<ushort, List<WorldFeatureDataEntry>> ids = new Dictionary<ushort, List<WorldFeatureDataEntry>>();

        foreach(var pair in featureData) {
            if(!ids.ContainsKey(pair.Key.id)) {
                List<WorldFeatureDataEntry> customFeatureData = new List<WorldFeatureDataEntry>();
                customFeatureData.Add(new WorldFeatureDataEntry(new int3(pair.Key.x, pair.Key.y, pair.Key.z), new BlockID(pair.Value)));
                ids.Add(pair.Key.id, customFeatureData);
            }

            else {
                List<WorldFeatureDataEntry> customFeatureData = ids[pair.Key.id];
                customFeatureData.Add(new WorldFeatureDataEntry(new int3(pair.Key.x, pair.Key.y, pair.Key.z), new BlockID(pair.Value)));
            }
        }

        foreach(var pair in ids) {
            WorldFeatureDataEntries entries = new WorldFeatureDataEntries(pair.Key, pair.Value);
            Debug.Log("Feature Data, " + pair.Key + ": " + JsonUtility.ToJson(entries));
        }

        foreach(var pair in featureSettings) {
            Debug.Log("Feature Settings, " + pair.Key + ": " + JsonUtility.ToJson(pair.Value));
        }

        Debug.Log("Feature Registry finished: " + timeTaken + " ms");
    }

    private static void DestroyNativeArrays() {
        featureData.Dispose();
        featureSettings.Dispose();

        disposed = true;
    }

    public static void Set(FeaturePlacement placement, BlockID id) {
        ushort packedID = id.Pack();

        if(featureData.ContainsKey(placement)) {
            featureData[placement] = packedID;
            return;
        }

        featureData.Add(placement, packedID);
    }

    public static void OnDestroy() {
        if(disposed) return;
        DestroyNativeArrays();
    }
}
