using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000701 RID: 1793
[Serializable]
public class WaterRadialMesh
{
	// Token: 0x0400291B RID: 10523
	private const float AlignmentGranularity = 1f;

	// Token: 0x0400291C RID: 10524
	private const float MaxHorizontalDisplacement = 1f;

	// Token: 0x0400291D RID: 10525
	private Mesh[] meshes;

	// Token: 0x0400291E RID: 10526
	private bool initialized;

	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x060032A5 RID: 12965 RVA: 0x001385DA File Offset: 0x001367DA
	public Mesh[] Meshes
	{
		get
		{
			return this.meshes;
		}
	}

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x060032A6 RID: 12966 RVA: 0x001385E2 File Offset: 0x001367E2
	public bool IsInitialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x060032A7 RID: 12967 RVA: 0x001385EA File Offset: 0x001367EA
	public void Initialize(int vertexCount)
	{
		this.meshes = this.GenerateMeshes(vertexCount, false);
		this.initialized = true;
	}

	// Token: 0x060032A8 RID: 12968 RVA: 0x00138604 File Offset: 0x00136804
	public void Destroy()
	{
		if (this.initialized)
		{
			Mesh[] array = this.meshes;
			for (int i = 0; i < array.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(array[i]);
			}
			this.meshes = null;
			this.initialized = false;
		}
	}

	// Token: 0x060032A9 RID: 12969 RVA: 0x00138644 File Offset: 0x00136844
	private Mesh CreateMesh(string name, Vector3[] vertices, int[] indices)
	{
		Mesh mesh = new Mesh();
		mesh.hideFlags = HideFlags.DontSave;
		mesh.name = name;
		mesh.vertices = vertices;
		mesh.SetIndices(indices, MeshTopology.Quads, 0);
		mesh.RecalculateBounds();
		mesh.UploadMeshData(true);
		return mesh;
	}

	// Token: 0x060032AA RID: 12970 RVA: 0x00138678 File Offset: 0x00136878
	private Mesh[] GenerateMeshes(int vertexCount, bool volume = false)
	{
		int num = Mathf.RoundToInt((float)Mathf.RoundToInt(Mathf.Sqrt((float)vertexCount)) * 0.4f);
		int num2 = Mathf.RoundToInt((float)vertexCount / (float)num);
		int num3 = volume ? (num2 / 2) : num2;
		List<Mesh> list = new List<Mesh>();
		List<Vector3> list2 = new List<Vector3>();
		List<int> list3 = new List<int>();
		Vector2[] array = new Vector2[num];
		int num4 = 0;
		int num5 = 0;
		for (int i = 0; i < num; i++)
		{
			float f = ((float)i / (float)(num - 1) * 2f - 1f) * 3.1415927f * 0.25f;
			array[i] = new Vector2(Mathf.Sin(f), Mathf.Cos(f)).normalized;
		}
		for (int j = 0; j < num3; j++)
		{
			float num6 = (float)j / (float)(num2 - 1);
			num6 = 1f - Mathf.Cos(num6 * 3.1415927f * 0.5f);
			for (int k = 0; k < num; k++)
			{
				Vector2 vector = array[k] * num6;
				if (j < num3 - 2 || !volume)
				{
					list2.Add(new Vector3(vector.x, 0f, vector.y));
				}
				else if (j == num3 - 2)
				{
					list2.Add(new Vector3(vector.x * 10f, -0.9f, vector.y) * 0.5f);
				}
				else
				{
					list2.Add(new Vector3(vector.x * 10f, -0.9f, vector.y * -10f) * 0.5f);
				}
				if (k != 0 && j != 0 && num4 > num)
				{
					list3.Add(num4);
					list3.Add(num4 - num);
					list3.Add(num4 - num - 1);
					list3.Add(num4 - 1);
				}
				num4++;
				if (num4 >= 65000)
				{
					list.Add(this.CreateMesh(string.Concat(new object[]
					{
						"WaterMesh_",
						num,
						"x",
						num2,
						"_",
						num5
					}), list2.ToArray(), list3.ToArray()));
					k--;
					j--;
					num6 = 1f - Mathf.Cos((float)j / (float)(num2 - 1) * 3.1415927f * 0.5f);
					num4 = 0;
					list2.Clear();
					list3.Clear();
					num5++;
				}
			}
		}
		if (num4 != 0)
		{
			list.Add(this.CreateMesh(string.Concat(new object[]
			{
				"WaterMesh_",
				num,
				"x",
				num2,
				"_",
				num5
			}), list2.ToArray(), list3.ToArray()));
		}
		return list.ToArray();
	}

	// Token: 0x060032AB RID: 12971 RVA: 0x00138974 File Offset: 0x00136B74
	private Vector3 RaycastPlane(Camera camera, float planeHeight, Vector3 pos)
	{
		Ray ray = camera.ViewportPointToRay(pos);
		if (camera.transform.position.y > planeHeight)
		{
			if (ray.direction.y > -0.01f)
			{
				ray.direction = new Vector3(ray.direction.x, -ray.direction.y - 0.02f, ray.direction.z);
			}
		}
		else if (ray.direction.y < 0.01f)
		{
			ray.direction = new Vector3(ray.direction.x, -ray.direction.y + 0.02f, ray.direction.z);
		}
		float d = -(ray.origin.y - planeHeight) / ray.direction.y;
		return Quaternion.AngleAxis(-camera.transform.eulerAngles.y, Vector3.up) * (ray.direction * d);
	}

	// Token: 0x060032AC RID: 12972 RVA: 0x00138A80 File Offset: 0x00136C80
	public Matrix4x4 ComputeLocalToWorldMatrix(Camera camera, float oceanWaterLevel)
	{
		if (camera == null)
		{
			return Matrix4x4.identity;
		}
		Vector3 vector = camera.worldToCameraMatrix.MultiplyVector(Vector3.up);
		Vector3 vector2 = camera.worldToCameraMatrix.MultiplyVector(Vector3.Cross(camera.transform.forward, Vector3.up));
		vector = new Vector3(vector.x, vector.y, 0f).normalized * 0.5f + new Vector3(0.5f, 0f, 0.5f);
		vector2 = new Vector3(vector2.x, vector2.y, 0f).normalized * 0.5f;
		Vector3 vector3 = this.RaycastPlane(camera, oceanWaterLevel, vector - vector2);
		Vector3 vector4 = this.RaycastPlane(camera, oceanWaterLevel, vector + vector2);
		float num = Mathf.Min(camera.farClipPlane, 5000f);
		Vector3 vector5 = camera.transform.position;
		Vector3 vector6 = default(Vector3);
		vector6.x = num * Mathf.Tan(camera.fieldOfView * 0.5f * 0.017453292f) * camera.aspect + 2f;
		vector6.y = num;
		vector6.z = num;
		float num2 = Mathf.Abs(vector4.x - vector3.x);
		float num3 = Mathf.Min(vector3.z, vector4.z) - (num2 + 2f) * vector6.z / vector6.x;
		num3 = Mathf.Min(num3, -15f);
		Vector3 forward = camera.transform.forward;
		forward.y = 0f;
		forward.Normalize();
		vector6.z -= num3;
		vector5 = new Vector3(vector5.x, oceanWaterLevel, vector5.z) + forward * num3;
		Quaternion q = Quaternion.AngleAxis(Mathf.Atan2(forward.x, forward.z) * 57.29578f, Vector3.up);
		return Matrix4x4.TRS(vector5, q, vector6);
	}
}
