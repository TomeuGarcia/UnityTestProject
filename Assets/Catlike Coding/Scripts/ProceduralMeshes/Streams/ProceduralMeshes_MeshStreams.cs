using UnityEngine;
using Unity.Mathematics;

namespace ProceduralMeshes
{
    public interface IMeshStreams
    {
        public void Setup(Mesh.MeshData meshData, Bounds bounds, int vertexCount, int indexCount);

        public void SetVertex(int index, Vertex vertex);

        public void SetTriangle(int index, int3 triangle);

    }

}