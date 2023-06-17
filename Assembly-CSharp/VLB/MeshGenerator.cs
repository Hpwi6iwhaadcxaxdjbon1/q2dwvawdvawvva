using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009B6 RID: 2486
	public static class MeshGenerator
	{
		// Token: 0x040035E5 RID: 13797
		private const float kMinTruncatedRadius = 0.001f;

		// Token: 0x170004B5 RID: 1205
		// (get) Token: 0x06003B3F RID: 15167 RVA: 0x0015F2C0 File Offset: 0x0015D4C0
		private static bool duplicateBackFaces
		{
			get
			{
				return Config.Instance.forceSinglePass;
			}
		}

		// Token: 0x06003B40 RID: 15168 RVA: 0x0015F2CC File Offset: 0x0015D4CC
		public static Mesh GenerateConeZ_RadiusAndAngle(float lengthZ, float radiusStart, float coneAngle, int numSides, int numSegments, bool cap)
		{
			Debug.Assert(lengthZ > 0f);
			Debug.Assert(coneAngle > 0f && coneAngle < 180f);
			float radiusEnd = lengthZ * Mathf.Tan(coneAngle * 0.017453292f * 0.5f);
			return MeshGenerator.GenerateConeZ_Radius(lengthZ, radiusStart, radiusEnd, numSides, numSegments, cap);
		}

		// Token: 0x06003B41 RID: 15169 RVA: 0x0015F320 File Offset: 0x0015D520
		public static Mesh GenerateConeZ_Angle(float lengthZ, float coneAngle, int numSides, int numSegments, bool cap)
		{
			return MeshGenerator.GenerateConeZ_RadiusAndAngle(lengthZ, 0f, coneAngle, numSides, numSegments, cap);
		}

		// Token: 0x06003B42 RID: 15170 RVA: 0x0015F334 File Offset: 0x0015D534
		public static Mesh GenerateConeZ_Radius(float lengthZ, float radiusStart, float radiusEnd, int numSides, int numSegments, bool cap)
		{
			Debug.Assert(lengthZ > 0f);
			Debug.Assert(radiusStart >= 0f);
			Debug.Assert(numSides >= 3);
			Debug.Assert(numSegments >= 0);
			Mesh mesh = new Mesh();
			bool flag = cap && radiusStart > 0f;
			radiusStart = Mathf.Max(radiusStart, 0.001f);
			int num = numSides * (numSegments + 2);
			int num2 = num;
			if (flag)
			{
				num2 += numSides + 1;
			}
			Vector3[] array = new Vector3[num2];
			for (int i = 0; i < numSides; i++)
			{
				float f = 6.2831855f * (float)i / (float)numSides;
				float num3 = Mathf.Cos(f);
				float num4 = Mathf.Sin(f);
				for (int j = 0; j < numSegments + 2; j++)
				{
					float num5 = (float)j / (float)(numSegments + 1);
					Debug.Assert(num5 >= 0f && num5 <= 1f);
					float num6 = Mathf.Lerp(radiusStart, radiusEnd, num5);
					array[i + j * numSides] = new Vector3(num6 * num3, num6 * num4, num5 * lengthZ);
				}
			}
			if (flag)
			{
				int num7 = num;
				array[num7] = Vector3.zero;
				num7++;
				for (int k = 0; k < numSides; k++)
				{
					float f2 = 6.2831855f * (float)k / (float)numSides;
					float num8 = Mathf.Cos(f2);
					float num9 = Mathf.Sin(f2);
					array[num7] = new Vector3(radiusStart * num8, radiusStart * num9, 0f);
					num7++;
				}
				Debug.Assert(num7 == array.Length);
			}
			if (!MeshGenerator.duplicateBackFaces)
			{
				mesh.vertices = array;
			}
			else
			{
				Vector3[] array2 = new Vector3[array.Length * 2];
				array.CopyTo(array2, 0);
				array.CopyTo(array2, array.Length);
				mesh.vertices = array2;
			}
			Vector2[] array3 = new Vector2[num2];
			int num10 = 0;
			for (int l = 0; l < num; l++)
			{
				array3[num10++] = Vector2.zero;
			}
			if (flag)
			{
				for (int m = 0; m < numSides + 1; m++)
				{
					array3[num10++] = new Vector2(1f, 0f);
				}
			}
			Debug.Assert(num10 == array3.Length);
			if (!MeshGenerator.duplicateBackFaces)
			{
				mesh.uv = array3;
			}
			else
			{
				Vector2[] array4 = new Vector2[array3.Length * 2];
				array3.CopyTo(array4, 0);
				array3.CopyTo(array4, array3.Length);
				for (int n = 0; n < array3.Length; n++)
				{
					Vector2 vector = array4[n + array3.Length];
					array4[n + array3.Length] = new Vector2(vector.x, 1f);
				}
				mesh.uv = array4;
			}
			int num11 = numSides * 2 * Mathf.Max(numSegments + 1, 1) * 3;
			if (flag)
			{
				num11 += numSides * 3;
			}
			int[] array5 = new int[num11];
			int num12 = 0;
			for (int num13 = 0; num13 < numSides; num13++)
			{
				int num14 = num13 + 1;
				if (num14 == numSides)
				{
					num14 = 0;
				}
				for (int num15 = 0; num15 < numSegments + 1; num15++)
				{
					int num16 = num15 * numSides;
					array5[num12++] = num16 + num13;
					array5[num12++] = num16 + num14;
					array5[num12++] = num16 + num13 + numSides;
					array5[num12++] = num16 + num14 + numSides;
					array5[num12++] = num16 + num13 + numSides;
					array5[num12++] = num16 + num14;
				}
			}
			if (flag)
			{
				for (int num17 = 0; num17 < numSides - 1; num17++)
				{
					array5[num12++] = num;
					array5[num12++] = num + num17 + 2;
					array5[num12++] = num + num17 + 1;
				}
				array5[num12++] = num;
				array5[num12++] = num + 1;
				array5[num12++] = num + numSides;
			}
			Debug.Assert(num12 == array5.Length);
			if (!MeshGenerator.duplicateBackFaces)
			{
				mesh.triangles = array5;
			}
			else
			{
				int[] array6 = new int[array5.Length * 2];
				array5.CopyTo(array6, 0);
				for (int num18 = 0; num18 < array5.Length; num18 += 3)
				{
					array6[array5.Length + num18] = array5[num18] + num2;
					array6[array5.Length + num18 + 1] = array5[num18 + 2] + num2;
					array6[array5.Length + num18 + 2] = array5[num18 + 1] + num2;
				}
				mesh.triangles = array6;
			}
			Bounds bounds = new Bounds(new Vector3(0f, 0f, lengthZ * 0.5f), new Vector3(Mathf.Max(radiusStart, radiusEnd) * 2f, Mathf.Max(radiusStart, radiusEnd) * 2f, lengthZ));
			mesh.bounds = bounds;
			Debug.Assert(mesh.vertexCount == MeshGenerator.GetVertexCount(numSides, numSegments, flag));
			Debug.Assert(mesh.triangles.Length == MeshGenerator.GetIndicesCount(numSides, numSegments, flag));
			return mesh;
		}

		// Token: 0x06003B43 RID: 15171 RVA: 0x0015F814 File Offset: 0x0015DA14
		public static int GetVertexCount(int numSides, int numSegments, bool geomCap)
		{
			Debug.Assert(numSides >= 2);
			Debug.Assert(numSegments >= 0);
			int num = numSides * (numSegments + 2);
			if (geomCap)
			{
				num += numSides + 1;
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				num *= 2;
			}
			return num;
		}

		// Token: 0x06003B44 RID: 15172 RVA: 0x0015F854 File Offset: 0x0015DA54
		public static int GetIndicesCount(int numSides, int numSegments, bool geomCap)
		{
			Debug.Assert(numSides >= 2);
			Debug.Assert(numSegments >= 0);
			int num = numSides * (numSegments + 1) * 2 * 3;
			if (geomCap)
			{
				num += numSides * 3;
			}
			if (MeshGenerator.duplicateBackFaces)
			{
				num *= 2;
			}
			return num;
		}

		// Token: 0x06003B45 RID: 15173 RVA: 0x0015F898 File Offset: 0x0015DA98
		public static int GetSharedMeshVertexCount()
		{
			return MeshGenerator.GetVertexCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
		}

		// Token: 0x06003B46 RID: 15174 RVA: 0x0015F8B4 File Offset: 0x0015DAB4
		public static int GetSharedMeshIndicesCount()
		{
			return MeshGenerator.GetIndicesCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, true);
		}
	}
}
