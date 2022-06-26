using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace ProceduralMeshes.Generators
{

	public struct FlatHexagonGrid : IMeshGenerator
	{

		public Bounds Bounds => new Bounds(Vector3.zero, 
										   new Vector3(0.75f + 0.25f / Resolution, 0.0f, Resolution > 1 ? 0.5f + 0.25f / Resolution : 0.5f * sqrt(3.0f)));

		public int VertexCount => 7 * Resolution * Resolution;

		public int IndexCount => 18 * Resolution * Resolution;

		public int JobLength => Resolution;

		public int Resolution { get; set; }


		public void Execute<S>(int x, S streams) where S : struct, IMeshStreams
		{
			int vi = 7 * Resolution * x;
			int ti = 6 * Resolution * x;

			float h = sqrt(3.0f) / 4.0f;

			float2 centerOffset = 0.0f;
			if (Resolution > 1)
            {
				centerOffset.x = -0.375f * (Resolution - 1);
				centerOffset.y = (((x & 1) == 1 ? 1.5f : 0.5f) - Resolution) * h;
            }


			for (int z = 0; z < Resolution; z++, vi += 7, ti += 6)
			{
				float2 center = (float2(0.75f * x, 2.0f * h * z) + centerOffset) / Resolution;
				float4 xCoordinates = center.x + float4(-0.5f, -0.25f, 0.25f, 0.5f) / Resolution;
				float2 zCoordinates = center.y + float2(h, -h) / Resolution;

				Vertex vertex = new Vertex();
				vertex.position.xz = center;
				vertex.normal.y = 1.0f;
				vertex.tangent.xw = float2(1.0f, -1.0f);
				vertex.texCoord0 = 0.5f;

				streams.SetVertex(vi, vertex);

				vertex.position.x = xCoordinates.x;
				vertex.texCoord0.x = 0.0f; 
				streams.SetVertex(vi + 1, vertex);

				vertex.position.x = xCoordinates.y;
				vertex.position.z = zCoordinates.x;
				vertex.texCoord0 = float2(0.25f, 0.5f + h);
				streams.SetVertex(vi + 2, vertex);

				vertex.position.x = xCoordinates.z;
				vertex.texCoord0.x = 0.75f;
				streams.SetVertex(vi + 3, vertex);

				vertex.position.x = xCoordinates.w;
				vertex.position.z = center.y;
				vertex.texCoord0 = float2(1f, 0.5f);
				streams.SetVertex(vi + 4, vertex);

				vertex.position.x = xCoordinates.z;
				vertex.position.z = zCoordinates.y;
				vertex.texCoord0 = float2(0.75f, 0.5f - h);
				streams.SetVertex(vi + 5, vertex);

				vertex.position.x = xCoordinates.y;
				vertex.texCoord0.x = 0.25f;
				streams.SetVertex(vi + 6, vertex);



				streams.SetTriangle(ti + 0, vi + int3(0, 1, 2));
				streams.SetTriangle(ti + 1, vi + int3(0, 2, 3));
				streams.SetTriangle(ti + 2, vi + int3(0, 3, 4));
				streams.SetTriangle(ti + 3, vi + int3(0, 4, 5));
				streams.SetTriangle(ti + 4, vi + int3(0, 5, 6));
				streams.SetTriangle(ti + 5, vi + int3(0, 6, 1));
			}
		}


	}



}