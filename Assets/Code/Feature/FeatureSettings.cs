using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FeatureSettings
{
    public int sizeX;
    public int sizeY;
    public int sizeZ;

    public FeatureSettings(int sizeX, int sizeY, int sizeZ) {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
        this.sizeZ = sizeZ;
    }
}
