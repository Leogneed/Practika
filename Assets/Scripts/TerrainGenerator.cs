using System;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour 
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType noiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    private FastNoiseLite[] octaveNoises;

    public void Awake()
    {
        octaveNoises = new FastNoiseLite[Octaves.Length];
        for (int i = 0; i < octaveNoises.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(Octaves[i].noiseType);
            octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }
    }

    public BlockType[,,] GenerateTerrain(int xOffset, int zOffset)
    {
        var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++) 
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++) 
            {
                float height = GetHeight(x * ChunkRenderer.BlockScale + xOffset, z * ChunkRenderer.BlockScale + zOffset);

                for (int y = 0; y < height / ChunkRenderer.BlockScale; y++)
                {
                    result[x, y, z] = BlockType.Dirt;
                }
            }
        }
        return result;
    }

    private float GetHeight(float x, float y)
    {
        float result = BaseHeight;

        for (int i = 0; i < Octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude / 2;
        }

        return result;
    }
}
