using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain generator")]
public class TerrainGenerator : ScriptableObject
{
    public float BaseHeight = 8;
    public NoiseOctaveSettings[] Octaves;
    public NoiseOctaveSettings DomainWarp;
    public BlockLayer[] BlockLayers;

    [Serializable]
    public class NoiseOctaveSettings
    {
        public FastNoiseLite.NoiseType NoiseType;
        public float Frequency = 0.2f;
        public float Amplitude = 1;
    }

    [Serializable]
    public class BlockLayer
    {
        public BlockType BlockType;
        public float MinHeight;
        public float MaxHeight;
        public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1); // Кривая для сглаживания высоты
    }

    private FastNoiseLite[] octaveNoises;
    private FastNoiseLite warpNoise;

    public void Init()
    {
        octaveNoises = new FastNoiseLite[Octaves.Length];
        for (int i = 0; i < octaveNoises.Length; i++)
        {
            octaveNoises[i] = new FastNoiseLite();
            octaveNoises[i].SetNoiseType(Octaves[i].NoiseType);
            octaveNoises[i].SetFrequency(Octaves[i].Frequency);
        }

        warpNoise = new FastNoiseLite();
        warpNoise.SetNoiseType(DomainWarp.NoiseType);
        warpNoise.SetFrequency(DomainWarp.Frequency);
        warpNoise.SetDomainWarpAmp(DomainWarp.Amplitude);
    }

    public BlockType[,,] GenerateTerrain(int xOffset, int zOffset)
    {
        var result = new BlockType[ChunkRenderer.ChunkWidth, ChunkRenderer.ChunkHeight, ChunkRenderer.ChunkWidth];

        for (int x = 0; x < ChunkRenderer.ChunkWidth; x++)
        {
            for (int z = 0; z < ChunkRenderer.ChunkWidth; z++)
            {
                float worldX = x * ChunkRenderer.BlockScale + xOffset;
                float worldZ = z * ChunkRenderer.BlockScale + zOffset;

                // Применение доменной деформации
                warpNoise.DomainWarp(ref worldX, ref worldZ);

                // Генерация основной высоты
                float baseHeight = GetBaseHeight(worldX, worldZ);

                // Применение октавного шума для добавления деталей
                float detailNoise = GetDetailNoise(worldX, worldZ);

                float height = baseHeight + detailNoise;

                for (int y = 0; y < ChunkRenderer.ChunkHeight; y++)
                {
                    float currentHeight = y * ChunkRenderer.BlockScale;

                    if (currentHeight <= height)
                    {
                        BlockType blockType = BlockType.Air;

                        foreach (var layer in BlockLayers)
                        {
                            if (currentHeight >= layer.MinHeight && currentHeight <= layer.MaxHeight)
                            {
                                float t = Mathf.InverseLerp(layer.MinHeight, layer.MaxHeight, currentHeight);
                                float heightMultiplier = layer.HeightCurve.Evaluate(t);
                                blockType = layer.BlockType;
                                break;
                            }
                        }

                        result[x, y, z] = blockType;
                    }
                }
            }
        }
        return result;
    }

    private float GetBaseHeight(float x, float y)
    {
        float result = BaseHeight;

        for (int i = 0; i < Octaves.Length; i++)
        {
            float noise = octaveNoises[i].GetNoise(x, y);
            result += noise * Octaves[i].Amplitude;
        }

        return result;
    }

    private float GetDetailNoise(float x, float y)
    {
        return warpNoise.GetNoise(x, y) * 2f; // Пример значения, можно настроить под ваши нужды
    }
}
