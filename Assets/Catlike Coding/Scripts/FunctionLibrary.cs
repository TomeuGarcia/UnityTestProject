using UnityEngine;

//using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate Vector3 Function(float u, float v, float t);

    //public enum FunctionName
    //{
    //    Wave, MultiWave, Ripple,
    //    Sphere, CollapsingSphere, VerticalBandsSphere, HorizontalBandsSphere, TwistingBandsSphere,
    //    Torus, TwistingTorus
    //};

    //private static Function[] functions = { Wave, MultiWave, Ripple,
    //                                        Sphere, CollapsingSphere, VerticalBandsSphere, HorizontalBandsSphere, TwistingBandsSphere,
    //                                        Torus, TwistingTorus };



    public enum FunctionName { Wave, MultiWave, Ripple, TwistingBandsSphere, TwistingTorus };

    private static Function[] functions = { Wave, MultiWave, Ripple, TwistingBandsSphere, TwistingTorus };

    public static int FunctionCount => functions.Length;

    public static Function GetFunction(FunctionName functionName) => functions[(int)functionName];

    public static FunctionName GetNextFunctionName(FunctionName name) => (int)name < functions.Length - 1 ? name + 1 : (FunctionName)0;


    public static FunctionName GetRandomFunctionName()
    {
        return (FunctionName)Random.Range(0, functions.Length);
    }

    public static FunctionName GetRandomFunctionNameOtherThan(FunctionName functionName)
    {
        FunctionName choice = (FunctionName)Random.Range(1, functions.Length);
        return choice == functionName ? 0 : choice;
    }


    public static Vector3 Wave(float u, float v, float t) 
    {
        Vector3 p;
        p.x = u;
        p.y = Mathf.Sin(Mathf.PI * (u + v + t));
        p.z = v;

        return p;
    }

    public static Vector3 MultiWave(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;

        p.y = Mathf.Sin(Mathf.PI * (u + 0.5f * t));
        p.y += Mathf.Sin(2.0f * Mathf.PI * (v + t)) * (1.0f / 2.0f);
        p.y += Mathf.Sin(Mathf.PI * (u + v + 0.25f * t));
        p.y *= 1.0f / 2.5f;

        p.z = v;

        return p;
    }

    public static Vector3 Ripple(float u, float v, float t)
    {
        Vector3 p;
        p.x = u;

        float d = Mathf.Sqrt(u * u + v * v);
        p.y = Mathf.Sin(Mathf.PI * (4.0f * d - t));
        p.y /= (1.0f + 10.0f * d);

        p.z = v;

        return p;
    }

    public static Vector3 Sphere(float u, float v, float t)
    {
        float r = Mathf.Cos(0.5f * Mathf.PI * v);

        Vector3 p;
        p.x = r * Mathf.Sin(Mathf.PI * u);
        p.y = Mathf.Sin(Mathf.PI * 0.5f * v);
        p.z = r * Mathf.Cos(Mathf.PI * u);

        return p;
    }

    public static Vector3 CollapsingSphere(float u, float v, float t)
    {
        float r = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * t);
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);

        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);

        return p;
    }

    public static Vector3 VerticalBandsSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Mathf.Sin(8.0f * Mathf.PI * u);
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);

        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);

        return p;
    }

    public static Vector3 HorizontalBandsSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Mathf.Sin(8.0f * Mathf.PI * v);
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);

        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);

        return p;
    }

    public static Vector3 TwistingBandsSphere(float u, float v, float t)
    {
        float r = 0.9f + 0.1f * Mathf.Sin(Mathf.PI * (6.0f * u + 4.0f * v + t));
        float s = r * Mathf.Cos(0.5f * Mathf.PI * v);

        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r * Mathf.Sin(Mathf.PI * 0.5f * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);

        return p;
    }

    public static Vector3 Torus(float u, float v, float t)
    {
        float r1 = 0.75f;
        float r2 = 0.25f;
        
        float s = r1 + r2 * Mathf.Cos(Mathf.PI * v);

        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r2 * Mathf.Sin(Mathf.PI * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);

        return p;
    }

    public static Vector3 TwistingTorus(float u, float v, float t)
    {
        float r1 = 0.7f + 0.1f * Mathf.Sin(Mathf.PI * (6.0f * u + 0.5f * t));
        float r2 = 0.15f + 0.05f * Mathf.Sin(Mathf.PI * (8.0f * u + 4.0f * v + 2.0f * t));

        float s = r1 + r2 * Mathf.Cos(Mathf.PI * v);

        Vector3 p;
        p.x = s * Mathf.Sin(Mathf.PI * u);
        p.y = r2 * Mathf.Sin(Mathf.PI * v);
        p.z = s * Mathf.Cos(Mathf.PI * u);

        return p;
    }



    public static Vector3 Morph( float u, float v, float t, Function from, Function to, float progress )
    {
        return Vector3.LerpUnclamped(from(u, v, t), to(u, v, t), Mathf.SmoothStep(0f, 1f, progress));
    }

}
