using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct NoiseProfile
{
    public float amplitude;
    public float frequency;
    public float persistance;
    public int octaves;

    public NoiseProfile(float amplitude, float frequency, float persistance, int octaves) {
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.persistance = persistance;
        this.octaves = octaves;
    }
}
