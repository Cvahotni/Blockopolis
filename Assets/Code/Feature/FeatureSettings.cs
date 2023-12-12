using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FeatureSettings
{
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public int featuresPerChunk;
    public FeaturePlaceType placeType;

    public int minY;

    public FeatureSettings(int sizeX, int sizeY, int sizeZ, int featuresPerChunk, FeaturePlaceType placeType, int minY) {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeZ = sizeZ;

        this.featuresPerChunk = featuresPerChunk;
        this.placeType = placeType;

        this.minY = minY;
    }
}
