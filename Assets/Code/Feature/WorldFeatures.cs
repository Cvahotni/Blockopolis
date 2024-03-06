using UnityEngine;
using Unity.Collections;

[RequireComponent(typeof(EndlessTerrain))]
public class WorldFeatures : MonoBehaviour
{
    public static WorldFeatures Instance { get; private set; }
    private NativeList<FeaturePlacement> featurePlacements = new NativeList<FeaturePlacement>();
    
    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        featurePlacements = new NativeList<FeaturePlacement>(Allocator.Persistent);
    }

    public void Add(FeaturePlacement placement) {
        featurePlacements.Add(placement);
    }

    public void Clear() {
        featurePlacements.Clear();
    }

    public NativeList<FeaturePlacement> GetPlacements() {
        return featurePlacements;
    }

    private void OnDestroy() {
        featurePlacements.Dispose();
    }
}
