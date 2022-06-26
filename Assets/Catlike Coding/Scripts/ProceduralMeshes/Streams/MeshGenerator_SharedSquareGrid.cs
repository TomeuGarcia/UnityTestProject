using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{

	public struct SharedSquareGrid : IMeshGenerator
	{

		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f, 0f, 1f));

		public int VertexCount => (Resolution + 1) * (Resolution + 1);

		public int IndexCount => 6 * Resolution * Resolution;

		public int JobLength => Resolution + 1;

		public int Resolution { get; set; }


		public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
		{
			int vi = (Resolution + 1) * z;
			int ti = 2 * Resolution * (z - 1);

			Vertex vertex = new Vertex();
			vertex.normal.y = 1.0f;
			vertex.tangent.xw = float2(1.0f, -1.0f);

			vertex.position.x = -0.5f;
			vertex.position.z = ((float)z / Resolution) - 0.5f;
			vertex.texCoord0.y = (float)z / Resolution;
			streams.SetVertex(vi, vertex);

			vi += 1;
			for (int x = 1; x <= Resolution; ++x, ++vi, ti += 2)
            {
				vertex.position.x += 1.0f / Resolution;
				vertex.texCoord0.x += 1.0f / Resolution;
				streams.SetVertex(vi, vertex);

				if (z > 0)
                {
					streams.SetTriangle(ti, vi + int3(-Resolution - 2, -1, - Resolution - 1));
					streams.SetTriangle(ti + 1, vi + int3(-Resolution - 1, -1, 0));
                }
            }


		}


	}



}