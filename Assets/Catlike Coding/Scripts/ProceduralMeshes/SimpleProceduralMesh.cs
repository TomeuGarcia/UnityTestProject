using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour
{

    private void OnEnable()
    {
        Mesh mesh = new Mesh
        {
            name = "Procedural Mesh"
        };


        mesh.vertices = new Vector3[] { Vector3.zero, Vector3.right, Vector3.up, new Vector3(1.0f, 1.0f) };

        mesh.normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };

        mesh.tangents = new Vector4[] { new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
                                        new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
                                        new Vector4(1.0f, 0.0f, 0.0f, -1.0f),
                                        new Vector4(1.0f, 0.0f, 0.0f, -1.0f) };

        mesh.uv = new Vector2[] { Vector2.zero, Vector2.right, Vector2.up, Vector2.one };

        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };

        GetComponent<MeshFilter>().mesh = mesh;

    }

}
