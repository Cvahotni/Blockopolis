using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System;

[Serializable]
public struct FeatureSettings
{
    public ushort id;
    public int3 size;

    public int featuresPerChunk;
    public FeaturePlaceType placeType;

    public int minY;
    public int maxY;

    public FeaturePriority priority;

    public float spawnChance;
    public BlockID overrideBlock;

    public FeatureSettings(ushort id, int3 size, int featuresPerChunk, FeaturePlaceType placeType, int minY, int maxY, float spawnChance, BlockID overrideBlock, FeaturePriority priority) {
        this.id = id;
        this.size = size;

        this.featuresPerChunk = featuresPerChunk;
        this.placeType = placeType;

        this.minY = minY;
        this.maxY = maxY;

        this.spawnChance = spawnChance;
        this.overrideBlock = overrideBlock;

        this.priority = priority;
    }
}
