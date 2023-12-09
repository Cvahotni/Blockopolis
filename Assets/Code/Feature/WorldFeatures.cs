using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

[RequireComponent(typeof(EndlessTerrain))]
public class WorldFeatures : MonoBehaviour
{
    public static WorldFeatures Instance { get; private set; }
    private NativeList<FeaturePlacement> featurePlacements = new NativeList<FeaturePlacement>();

    private EndlessTerrain endlessTerrain;
    
    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        endlessTerrain = EndlessTerrain.Instance;
        featurePlacements = new NativeList<FeaturePlacement>(Allocator.Persistent);

        Vector2 noiseOffset = endlessTerrain.NoiseOffset;

        //This is temporary
        for(int i = 0; i < 5000; i++) {
            int randomX = Random.Range(-512, 512);
            int randomZ = Random.Range(-512, 512);

            int y = (int) (Noise.Get2DNoise(
                WorldUtil.GetRealWorldX(randomX), WorldUtil.GetRealWorldZ(randomZ), 
                noiseOffset.x, noiseOffset.y, 
                endlessTerrain.NativeFrequencies, endlessTerrain.NativeAmplitudes)) - 15;

            if(WorldUtil.IsBelowSeaLevel(y)) continue;
            Add(new FeaturePlacement(randomX, y, randomZ, 0));
        }
    }

    public void Add(FeaturePlacement placement) {
        featurePlacements.Add(placement);
    }

    public void Remove(FeaturePlacement placement) {
        //featurePlacements.Remove(placement);
    }

    public NativeList<FeaturePlacement> GetPlacements() {
        return featurePlacements;
    }

    private void OnDestroy() {
        featurePlacements.Dispose();
    }
}
