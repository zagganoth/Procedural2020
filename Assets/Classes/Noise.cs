using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Noise
{
    int seed;
    float frequency;
    float amplitude;

    float lacunarity;
    float persistence;

    int octaves;
    public Noise(int seed,float frequency, float amplitude, float lacunarity, float persistence, int octaves)
    {
        this.seed = seed;
        this.frequency = frequency;
        this.amplitude = amplitude;
        this.lacunarity = lacunarity;
        this.persistence = persistence;
        this.octaves = octaves;
    }
    public float[] GetNoiseValues(int boundsCenterX, int boundsCenterY, int size)
    {
        float[] noiseValues = new float[size*size];
        float max = 0f;
        float min = float.MaxValue;
        int index = 0;
        for(int i = boundsCenterX-size/2; i < boundsCenterX+size/2;i++)
        {
            for(int j = boundsCenterY-size/2;j<boundsCenterY+size/2;j++)
            {
                float tempA = amplitude;
                float tempF = frequency;
                noiseValues[index] = 0f;
                for(int k = 0; k < octaves; k++)
                {
                    noiseValues[index] += Mathf.PerlinNoise(((i + seed + boundsCenterX) / (float)size * frequency), (j+seed+boundsCenterY) / (float)size * frequency) * amplitude;
                    frequency *= lacunarity;
                    amplitude *= persistence;
                }
                amplitude = tempA;
                frequency = tempF;
                if (noiseValues[index] > max) max = noiseValues[index];
                if (noiseValues[index] < min) min = noiseValues[index];
                index++;
            }
        }
        index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                noiseValues[index] = Mathf.InverseLerp(max, min, noiseValues[index]);
            }
        }


        return noiseValues;
    }

}
