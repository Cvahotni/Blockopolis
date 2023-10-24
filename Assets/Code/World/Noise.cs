using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float Get2DNoiseAt(float offsetX, float offsetZ, float x, float z, float amplitude, float frequency) {
        float xOffset = 0.01f;
        float zOffset = 0.01f;

        float inputX = (x + xOffset) * frequency;
        float inputZ = (z + zOffset) * frequency;

        return (Mathf.PerlinNoise(inputX + offsetX, inputZ + offsetZ)) * amplitude;
    }

    public static float Get3DNoiseAt(float offsetX, float offsetY, float offsetZ, float x, float y, float z, float amplitude, float frequency) {
        float xOffset = 0.01f;
        float yOffset = 0.01f;
        float zOffset = 0.01f;

        float inputX = (x + xOffset) * frequency;
        float inputY = (y + yOffset) * frequency;
        float inputZ = (z + zOffset) * frequency;

        return (Get3DPerlin(inputX + offsetX, inputY + offsetY, inputZ + offsetZ)) * amplitude;
    }

    private static float Get3DPerlin(float x, float y, float z) {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);

        return (xy + xz + yz + yx + zx + zy) / 6;
    }
}
