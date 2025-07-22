using System.Collections.Generic;
using UnityEngine;

public class ClimateBiomeHelper
{
    private List<ClimateBiome> biomes;
    private float seaLevel, mountainLevel;

    public ClimateBiomeHelper(List<ClimateBiome> biomes, float seaLevel, float mountainLevel)
    {
        this.biomes = biomes;
        this.seaLevel = seaLevel;
        this.mountainLevel = mountainLevel;
    }

    public ClimateBiome GetBiome(float elevation, float temperature, float moisture)
    {
        if (elevation <= seaLevel || elevation >= mountainLevel)
            return null;

        foreach (var biome in biomes)
        {
            if (temperature >= biome.minTemperature && temperature <= biome.maxTemperature &&
                moisture >= biome.minMoisture && moisture <= biome.maxMoisture)
                return biome;
        }

        Debug.LogWarning($"No biome match for temp {temperature:F2}, moist {moisture:F2}, elev {elevation:F2}");
        return null;
    }

    public ClimateBiome GetBiome(float temperature, float moisture)
    {
        foreach (var biome in biomes)
        {
            if (temperature >= biome.minTemperature && temperature <= biome.maxTemperature &&
                moisture >= biome.minMoisture && moisture <= biome.maxMoisture)
            {
                return biome;
            }
        }

        Debug.LogWarning($"No biome match for temperature {temperature:F2}, moisture {moisture:F2}");
        return null;
    }
}

