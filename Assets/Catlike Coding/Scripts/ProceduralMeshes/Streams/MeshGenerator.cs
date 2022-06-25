using UnityEngine;

namespace ProceduralMeshes
{
    public interface IMeshGenerator
    {
        int VertexCount { get; }
        int IndexCount { get; }
        int JobLength { get; }
        int Resolution { get; set; }

        Bounds Bounds { get; }

        void Execute<S>(int i, S Streams) where S : struct, IMeshStreams;
    }

}