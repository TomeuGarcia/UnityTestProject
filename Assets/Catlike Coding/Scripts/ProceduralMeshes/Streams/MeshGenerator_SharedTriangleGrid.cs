using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{

	public struct SharedTriangleGrid : IMeshGenerator
	{

		public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f + 0.5f / Resolution, 0f, sqrt(3.0f) / 2.0f));

		public int VertexCount => (Resolution + 1) * (Resolution + 1);

		public int IndexCount => 6 * Resolution * Resolution;

		public int JobLength => Resolution + 1;

		public int Resolution { get; set; }

		public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
		{
			int vi = (Resolution + 1) * z, ti = 2 * Resolution * (z - 1);

			float xOffset = -0.25f;
			float uOffset = 0.0f;

			int iA = -Resolution - 2;
			int iB = -Resolution - 1;
			int iC = -1;
			int iD = 0;

			int3 tA = int3(iA, iC, iD);
			int3 tB = int3(iA, iD, iB);

			if ((z & 1) == 1)
			{
				xOffset = 0.25f;
				uOffset = 0.5f / (Resolution + 0.5f);
				tA = int3(iA, iC, iB);
				tB = int3(iB, iC, iD);
			}

			xOffset = xOffset / Resolution - 0.5f;

			Vertex vertex = new Vertex();
			vertex.normal.y = 1f;
			vertex.tangent.xw = float2(1f, -1f);

			vertex.position.x = xOffset;
			vertex.position.z = ((float)z / Resolution - 0.5f) * sqrt(3.0f) / 2.0f;
			vertex.texCoord0.x = uOffset;
			vertex.texCoord0.y = vertex.position.z / (1.0f + (0.5f / Resolution)) + 0.5f;
			streams.SetVertex(vi, vertex);
			vi += 1;

			for (int x = 1; x <= Resolution; x++, vi++, ti += 2)
			{
				vertex.position.x += 1.0f / Resolution;
				vertex.texCoord0.x += 1.0f / (Resolution + 0.5f);
				streams.SetVertex(vi, vertex);

				if (z > 0)
				{
					streams.SetTriangle(ti + 0, vi + tA);
					streams.SetTriangle(ti + 1, vi + tB);
				}
			}

		}


	}




}