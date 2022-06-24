using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

public abstract class Visualization : MonoBehaviour
{
    static int positionsId = Shader.PropertyToID("_Positions");
    static int normalsId = Shader.PropertyToID("_Normals");
    static int configId = Shader.PropertyToID("_Config");

    [SerializeField] Mesh instanceMesh;
    [SerializeField] Material material;
    [SerializeField] Shape shape;
    [SerializeField, Range(0.1f, 10.0f)] float instanceScale = 2.0f;
    [SerializeField, Range(1, 512)] int resolution = 16;
    [SerializeField, Range(-0.5f, 0.5f)] float displacement = 0.1f;



    NativeArray<float3x4> positions;
    NativeArray<float3x4> normals;
    ComputeBuffer positionsBuffer;
    ComputeBuffer normalsBuffer;
    MaterialPropertyBlock propertyBlock;

    bool isDirty = false;

    Bounds bounds;


    public enum Shape { Plane, UVSphere, OctahedronSphere, Torus }

    static Shapes.ScheduleDelegate[] shapeJobs = {
        Shapes.Job<Shapes.Plane>.ScheduleParallel,
        Shapes.Job<Shapes.UVSphere>.ScheduleParallel,
        Shapes.Job<Shapes.OctahedronSphere>.ScheduleParallel,
        Shapes.Job<Shapes.Torus>.ScheduleParallel
    };



    private void OnEnable()
    {
        isDirty = true;

        int length = resolution * resolution;
        length = length / 4 + (length & 1);

        positions = new NativeArray<float3x4>(length, Allocator.Persistent);
        normals = new NativeArray<float3x4>(length, Allocator.Persistent);

        positionsBuffer = new ComputeBuffer(length * 4, 3 * sizeof(uint));
        normalsBuffer = new ComputeBuffer(length * 4, 3 * sizeof(uint));

        propertyBlock ??= new MaterialPropertyBlock();
        EnableVisualization(length, propertyBlock);
        propertyBlock.SetBuffer(positionsId, positionsBuffer);
        propertyBlock.SetBuffer(normalsId, normalsBuffer);
        propertyBlock.SetVector(configId, new Vector4(resolution, instanceScale / resolution, displacement));
    }

    private void OnDisable()
    {
        positions.Dispose();
        normals.Dispose();

        positionsBuffer.Release();
        positionsBuffer = null;
        normalsBuffer.Release();
        normalsBuffer = null;

        DisableVisualization();
    }

    private void OnValidate()
    {
        if (positionsBuffer != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }



    private void Update()
    {
        if (isDirty || transform.hasChanged)
        {
            isDirty = false;
            transform.hasChanged = false;

            UpdateVisualization(positions, 
                                resolution, 
                                shapeJobs[(int)shape](positions, normals, resolution, transform.localToWorldMatrix, default)
                                );

            positionsBuffer.SetData(positions.Reinterpret<float3>(3 * 4 * 4));
            normalsBuffer.SetData(normals.Reinterpret<float3>(3 * 4 * 4));

            bounds = new Bounds(transform.position, float3(2.0f * cmax(abs(transform.lossyScale)) + displacement));
        }


        Graphics.DrawMeshInstancedProcedural(
            instanceMesh, 
            0, 
            material, 
            bounds, 
            resolution * resolution, 
            propertyBlock
        );

    }


    protected abstract void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock);

    protected abstract void DisableVisualization();

    protected abstract void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle);


}
