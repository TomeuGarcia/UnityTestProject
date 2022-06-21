using UnityEngine;

public class FractalCPU : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;


    private void Start()
    {
        name = "Fractal " + depth;
        if (depth <= 1) return;

        FractalCPU childA = CreateFractal(Vector3.up, Quaternion.identity);
        FractalCPU childB = CreateFractal(Vector3.right, Quaternion.Euler(0.0f, 0.0f, -90.0f));
        FractalCPU childC = CreateFractal(Vector3.left, Quaternion.Euler(0.0f, 0.0f, 90.0f));
        FractalCPU childD = CreateFractal(Vector3.forward, Quaternion.Euler(90.0f, 0.0f, 0.0f));
        FractalCPU childE = CreateFractal(Vector3.back, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        childA.transform.SetParent(transform, false);
        childB.transform.SetParent(transform, false);
        childC.transform.SetParent(transform, false);
        childD.transform.SetParent(transform, false);
        childE.transform.SetParent(transform, false);
    }

    private void Update()
    {
        transform.Rotate(0.0f, 22.5f * Time.deltaTime, 0.0f);
    }



    FractalCPU CreateFractal(Vector3 direction, Quaternion rotation)
    {
        FractalCPU child = Instantiate(this);
        child.transform.localPosition = 0.75f * direction;
        child.transform.localRotation = rotation;
        child.transform.localScale = 0.5f * Vector3.one;
        child.depth = depth - 1;

        return child;
    }

}
