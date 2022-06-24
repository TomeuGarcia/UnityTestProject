using System;
using Unity.Burst;
using UnityEngine;

using static Unity.Mathematics.math;

public static partial class Noise
{
    [Serializable]
    public struct Settings
    {
        public int seed;

        [Min(1)] public int frequency;
        [Range(1, 6)] public int octaves;
        [Range(2, 4)] public int locunarity;
        [Range(0.0f, 1.0f)] public float persistance;

        public static Settings Default => new Settings {
            frequency = 4,
            octaves = 1,
            locunarity = 2,
            persistance = 0.5f
        };
    }


}
