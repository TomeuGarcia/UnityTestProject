
using Unity.Mathematics;

using static Unity.Mathematics.math;


public static partial class Noise
{
    public interface IGradient
    {
        public float4 Evaluate(SmallXXHash4 hash, float4 x);
        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y);
        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y, float4 z);

        public float4 EvaluateAfterInterpolation(float4 value);
    }

    public struct Value : IGradient
    {
        public float4 Evaluate(SmallXXHash4 hash, float4 x) => hash.Floats01A * 2.0f - 1.0f;
        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y) => hash.Floats01A * 2.0f - 1.0f;
        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y, float4 z) => hash.Floats01A * 2.0f - 1.0f;

        public float4 EvaluateAfterInterpolation(float4 value) => value;
    }

    public struct Perlin : IGradient
    {
        public float4 Evaluate(SmallXXHash4 hash, float4 x) => 2.0f * (1.0f + hash.Floats01A) * select(-x, x, ((uint4)hash & (1 << 8)) == 0);
        
        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y)
        {
            float4 gx = hash.Floats01A * 2.0f - 1.0f;
            float4 gy = 0.5f - abs(gx);
            gx -= floor(gx + 0.5f);

            return (gx * x + gy * y) * (2.0f / 0.53528f);
        }

        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y, float4 z)
        {
            float4 gx = hash.Floats01A * 2.0f - 1.0f; 
            float4 gy = hash.Floats01D * 2.0f - 1.0f;
            float4 gz = 1.0f - abs(gx) - abs(gy);

            float4 offset = max(-gz, 0.0f);

            gx += select(-offset, offset, gx < 0.0f);
            gy += select(-offset, offset, gy < 0.0f);

            return (gx * x + gy * y + gz * z) * (1.0f / 0.56290f);
        }

        public float4 EvaluateAfterInterpolation(float4 value) => value;
    }


    public struct Turbulence<G> : IGradient where G : struct, IGradient
    {
        public float4 Evaluate(SmallXXHash4 hash, float4 x) => default(G).Evaluate(hash, x);

        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y) => default(G).Evaluate(hash, x, y);

        public float4 Evaluate(SmallXXHash4 hash, float4 x, float4 y, float4 z) => default(G).Evaluate(hash, x, y, z);

        public float4 EvaluateAfterInterpolation(float4 value) => abs(default(G).EvaluateAfterInterpolation(value));
    }


}
