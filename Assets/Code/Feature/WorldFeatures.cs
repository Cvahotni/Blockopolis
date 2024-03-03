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

        featurePlacements = new NativeList<FeaturePlacement>(Allocator.Persistent);
    }

    private void Start() {
        endlessTerrain = EndlessTerrain.Instance;
    }

    public void Add(FeaturePlacement placement) {
        featurePlacements.Add(placement);
    }

    public void Clear() {
        for(int i = featurePlacements.Length - 1; i >= 0; i--) {
            FeaturePlacement placement = featurePlacements[i];

            int x = placement.x >> VoxelProperties.chunkBitShift;
            int z = placement.z >> VoxelProperties.chunkBitShift;

            long pos = ChunkPositionHelper.GetChunkPos(x, z);

            if(endlessTerrain.IsFeatureChunkOutOfRange(pos)) {
                featurePlacements.RemoveAtSwapBack(i);
            }
        }
    }

    public NativeList<FeaturePlacement> GetPlacements() {
        return featurePlacements;
    }

    private void OnDestroy() {
        featurePlacements.Dispose();
    }
}
