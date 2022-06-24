using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;


using static Noise;


public class NoiseVisualization : Visualization
{
    static int noiseId = Shader.PropertyToID("_Noise");


    [SerializeField] Settings noiseSettings = Settings.Default;
    [SerializeField] SpaceTRS domain = new SpaceTRS { scale = 8.0f };
    [SerializeField, Range(1, 3)] int dimensions = 1;
    [SerializeField] bool tiling = false;


    public enum NoiseType { Perlin, PerlinTurbulence, Value, ValueTurbulence, 
                            VoronoiWorleyF1, VoronoiWorleyF2, VoronoiWorleyF2MinusF1,
                            VoronoiChebyshevF1, VoronoiChebyshevF2, VoronoiChebyshevF2MinusF1,
                            VoronoiEucledianF1, VoronoiEucledianF2, VoronoiEucledianF2MinusF1,
                            VoronoiManhattanF1, VoronoiManhattanF2, VoronoiManhattanF2MinusF1
    };
    [SerializeField] NoiseType noiseType;

    NativeArray<float4> noise;
    ComputeBuffer noiseBuffer;

    static ScheduleDelegate[,] noiseJobs =
    {
        {
            Job<Lattice1D<LatticeNormal, Perlin>>.ScheduleParallel,
            Job<Lattice1D<LatticeTiling, Perlin>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Perlin>>.ScheduleParallel,
            Job<Lattice2D<LatticeTiling, Perlin>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Perlin>>.ScheduleParallel,
            Job<Lattice3D<LatticeTiling, Perlin>>.ScheduleParallel
        },
        {
            Job<Lattice1D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice1D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice2D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Turbulence<Perlin>>>.ScheduleParallel,
            Job<Lattice3D<LatticeTiling, Turbulence<Perlin>>>.ScheduleParallel
        },
        {
            Job<Lattice1D<LatticeNormal, Value>>.ScheduleParallel,
            Job<Lattice1D<LatticeTiling, Value>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Value>>.ScheduleParallel,
            Job<Lattice2D<LatticeTiling, Value>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Value>>.ScheduleParallel,
            Job<Lattice3D<LatticeTiling, Value>>.ScheduleParallel
        },
        {
            Job<Lattice1D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice1D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice2D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice2D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice3D<LatticeNormal, Turbulence<Value>>>.ScheduleParallel,
            Job<Lattice3D<LatticeTiling, Turbulence<Value>>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Worley, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Worley, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Worley, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Worley, F1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Worley, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Worley, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Worley, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Worley, F2>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Chebyshev, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Chebyshev, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Chebyshev, F1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Chebyshev, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Chebyshev, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Chebyshev, F2>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Chebyshev, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Chebyshev, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Chebyshev, F2MinusF1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Eucledian, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Eucledian, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Eucledian, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Eucledian, F1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Eucledian, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Eucledian, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Eucledian, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Eucledian, F2>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Eucledian, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Eucledian, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Eucledian, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Eucledian, F2MinusF1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Manhattan, F1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Manhattan, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Manhattan, F1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Manhattan, F1>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Manhattan, F2>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Manhattan, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Manhattan, F2>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Manhattan, F2>>.ScheduleParallel
        },
        {
            Job<Voronoi1D<LatticeNormal, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi1D<LatticeTiling, Worley, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeNormal, Manhattan, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi2D<LatticeTiling, Manhattan, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeNormal, Manhattan, F2MinusF1>>.ScheduleParallel,
            Job<Voronoi3D<LatticeTiling, Manhattan, F2MinusF1>>.ScheduleParallel
        }
    };



    protected override void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock)
    {
        noise = new NativeArray<float4>(dataLength, Allocator.Persistent);

        noiseBuffer = new ComputeBuffer(dataLength * 4, sizeof(uint));

        propertyBlock.SetBuffer(noiseId, noiseBuffer);
    }

    protected override void DisableVisualization()
    {
        noise.Dispose();

        noiseBuffer.Release();
        noiseBuffer = null;
    }




    protected override void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle)
    {
        noiseJobs[(int)noiseType, 2 * dimensions - (tiling ? 1 : 2)](positions, noise, noiseSettings, domain, resolution, handle).Complete();
        noiseBuffer.SetData(noise.Reinterpret<float>(4 * 4));
    }



}
