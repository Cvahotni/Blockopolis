using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

public class WorldFeatureLoad
{
    public static void Load() {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

        LoadSettingsFromDisk();
        LoadDataFromDisk();

        watch.Stop();
        float timeTaken = watch.ElapsedTicks * 1000 / System.Diagnostics.Stopwatch.Frequency;

        Debug.Log("World Feature Load finished: " + timeTaken + " ms");
    }

    private static void LoadSettingsFromDisk() {
        UnityEngine.Object[] objects = Resources.LoadAll("Feature Settings");
    
        for(ushort i = 0; i < objects.Length; i++) {
            UnityEngine.Object currentObject = objects[i];

            TextAsset textData = (TextAsset) currentObject;
            string text = textData.text;

            FeatureSettings settings = JsonUtility.FromJson<FeatureSettings>(text);
            FeatureRegistry.FeatureSettings.Add(settings.id, settings);
        }
    }

    private static void LoadDataFromDisk() {
        UnityEngine.Object[] objects = Resources.LoadAll("Feature Data");
    
        for(ushort i = 0; i < objects.Length; i++) {
            UnityEngine.Object currentObject = objects[i];

            TextAsset textData = (TextAsset) currentObject;
            string text = textData.text;

            WorldFeatureDataEntries entries = JsonUtility.FromJson<WorldFeatureDataEntries>(text);
            Dictionary<FeaturePlacement, ushort> dictionary = entries.ToDictionary();

            foreach(var pair in dictionary) FeatureRegistry.Set(pair.Key, pair.Value);
        }
    }
}
