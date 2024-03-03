using UnityEngine;
using Unity.Collections;

public class Noise
{
    public static float Get2DNoise(float worldX, float worldZ, float noiseOffsetX, float noiseOffsetZ, NativeList<float> frequencies, NativeList<float> amplitudes) {
        int frequenciesCount = frequencies.Length;
        float totalNoiseValue = 0.0f;

        float terrainNoiseOffsetX = noiseOffsetX;
        float terrainNoiseOffsetZ = noiseOffsetZ;

        for(int i = 0; i < frequenciesCount; i++) {
            float currentFrequency = frequencies[i];
            float currentAmplitude = amplitudes[i];
        
            totalNoiseValue += Get2DNoiseAt(terrainNoiseOffsetX, terrainNoiseOffsetZ, worldX, worldZ, currentAmplitude, currentFrequency);
        }

        return totalNoiseValue;
    }


    public static float Get2DNoiseAt(float offsetX, float offsetZ, float x, float z, float amplitude, float frequency) {
        float xOffset = 0.01f;
        float zOffset = 0.01f;

        float inputX = (x + xOffset) * frequency;
        float inputZ = (z + zOffset) * frequency;

        return Mathf.PerlinNoise(inputX + offsetX, inputZ + offsetZ) * amplitude;
    }

    public static float Get3DNoiseAt(float offsetX, float offsetY, float offsetZ, float x, float y, float z, float amplitude, float frequency) {
        float xOffset = 0.01f;
        float yOffset = 0.01f;
        float zOffset = 0.01f;

        float inputX = (x + xOffset) * frequency;
        float inputY = (y + yOffset) * frequency;
        float inputZ = (z + zOffset) * frequency;

        return Get3DPerlin(inputX + offsetX, inputY + offsetY, inputZ + offsetZ) * amplitude;
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
