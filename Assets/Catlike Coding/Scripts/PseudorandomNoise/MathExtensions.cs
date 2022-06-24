
using Unity.Mathematics;

using static Unity.Mathematics.math;


public static class MathExtensions
{
    public static float4x3 TransformVectors(float3x4 trs, float4x3 p, float w = 1.0f) => float4x3(
        trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x * w,
        trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y * w,
        trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z * w
    );

    public static float3x4 Get3x4(float4x4 m) => float3x4(m.c0.xyz, m.c1.xyz, m.c2.xyz, m.c3.xyz);

    public static float4 C2continuous01Smoothstep(float4 t)
    {
        return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
    }

}
