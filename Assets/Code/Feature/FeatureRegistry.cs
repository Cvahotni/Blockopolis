using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

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

        featureData = new NativeParallelHashMap<FeaturePlacement, ushort>(1, Allocator.Persistent);
        featureSettings = new NativeParallelHashMap<ushort, FeatureSettings>(1, Allocator.Persistent);

        AddTree();

        watch.Stop();
        float timeTaken = watch.ElapsedTicks * 1000000 / System.Diagnostics.Stopwatch.Frequency;

        Debug.Log("Feature Registry finished: " + timeTaken + " μs");
    }

    private static void DestroyNativeArrays() {
        featureData.Dispose();
        featureSettings.Dispose();

        disposed = true;
    }

    private static void AddTree() {
        featureSettings.Add(0, new FeatureSettings(5, 7, 5, 4, FeaturePlaceType.Surface, 66));

        for(int i = -2; i <= 2; i++) {
            for(int j = 3; j < 5; j++) {
                for(int k = -2; k <= 2; k++) {
                    Set(new FeaturePlacement(i, j, k, 0), new BlockID(8, 0));
                }
            }
        }

        for(int i = -1; i <= 1; i++) {
            for(int j = 5; j <= 7; j++) {
                for(int k = -1; k <= 1; k++) {
                    Set(new FeaturePlacement(i, j, k, 0), new BlockID(8, 0));
                }
            }
        }

        Set(new FeaturePlacement(-1, 6, -1, 0), new BlockID(0));
        Set(new FeaturePlacement(-1, 6, 1, 0), new BlockID(0));
        Set(new FeaturePlacement(1, 6, -1, 0), new BlockID(0));
        Set(new FeaturePlacement(1, 6, 1, 0), new BlockID(0));

        for(int i = 0; i < 5; i++) Set(new FeaturePlacement(0, i, 0, 0), new BlockID(7, 0));
    }

    private static void Set(FeaturePlacement placement, BlockID id) {
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
