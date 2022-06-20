using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;
    static readonly int positionsId = Shader.PropertyToID("_Positions");
    static readonly int resolutionId = Shader.PropertyToID("_Resolution");
    static readonly int stepId = Shader.PropertyToID("_Step");
    static readonly int timeId = Shader.PropertyToID("_Time");

    [SerializeField] Material material;
    [SerializeField] Mesh mesh;

    [SerializeField, Range(10, 200)] int resolution = 100;
    [SerializeField] FunctionLibrary.FunctionName function = FunctionLibrary.FunctionName.Wave;

    float duration = 0.0f;

    public enum TransitionMode { Cycle, Random }
    [SerializeField] TransitionMode transitionMode;
    [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;

    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;


    ComputeBuffer positionsBuffer;

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * sizeof(float));
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }


    private void Update()
    {
        duration += Time.deltaTime;

        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }


        UpdateFunctionOnGPU();
    }



    void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }


    void UpdateFunctionOnGPU()
    {
        float step = 2.0f / resolution;

        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);

        computeShader.SetBuffer(0, positionsId, positionsBuffer);

        int groups = Mathf.CeilToInt(resolution / 8.0f);
        computeShader.Dispatch(0, groups, groups, 1);


        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);

        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (2.0f + 2.0f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
    }

}
