using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRegionPool : MonoBehaviour
{
    public static WorldRegionPool Instance { get; private set; }

    private List<WorldRegion> regions = new List<WorldRegion>();
    [SerializeField] private int poolSize = 1024;

    private void Awake() {
        if(Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Start() {
        for(int i = 0; i < poolSize; i++) {
            regions.Add(new WorldRegion(false));
        }
    }

    private void OnDestroy() {
        foreach(WorldRegion region in regions) {
            region.Destroy();
        }
    }
}
