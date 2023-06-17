using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000654 RID: 1620
public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	// Token: 0x04002693 RID: 9875
	private List<int> indices;

	// Token: 0x04002694 RID: 9876
	private List<Vector3> vertices;

	// Token: 0x04002695 RID: 9877
	private List<Vector3> normals;

	// Token: 0x04002696 RID: 9878
	private List<int> triangles;

	// Token: 0x04002697 RID: 9879
	private Vector3 pivot;

	// Token: 0x04002698 RID: 9880
	private int width;

	// Token: 0x04002699 RID: 9881
	private int height;

	// Token: 0x0400269A RID: 9882
	private bool normal;

	// Token: 0x0400269B RID: 9883
	private bool alpha;

	// Token: 0x0400269C RID: 9884
	private Action worker;

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x06002EFF RID: 12031 RVA: 0x0011C0D8 File Offset: 0x0011A2D8
	public override bool keepWaiting
	{
		get
		{
			return this.worker != null;
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06002F00 RID: 12032 RVA: 0x0011C0E3 File Offset: 0x0011A2E3
	public bool isDone
	{
		get
		{
			return this.worker == null;
		}
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x0011C0F0 File Offset: 0x0011A2F0
	public NavMeshBuildSource CreateNavMeshBuildSource()
	{
		return new NavMeshBuildSource
		{
			transform = Matrix4x4.TRS(this.pivot, Quaternion.identity, Vector3.one),
			shape = NavMeshBuildSourceShape.Mesh,
			sourceObject = this.mesh
		};
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x0011C138 File Offset: 0x0011A338
	public NavMeshBuildSource CreateNavMeshBuildSource(int area)
	{
		NavMeshBuildSource result = this.CreateNavMeshBuildSource();
		result.area = area;
		return result;
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06002F03 RID: 12035 RVA: 0x0011C158 File Offset: 0x0011A358
	public Mesh mesh
	{
		get
		{
			Mesh mesh = new Mesh();
			if (this.vertices != null)
			{
				mesh.SetVertices(this.vertices);
				Pool.FreeList<Vector3>(ref this.vertices);
			}
			if (this.normals != null)
			{
				mesh.SetNormals(this.normals);
				Pool.FreeList<Vector3>(ref this.normals);
			}
			if (this.triangles != null)
			{
				mesh.SetTriangles(this.triangles, 0);
				Pool.FreeList<int>(ref this.triangles);
			}
			if (this.indices != null)
			{
				Pool.FreeList<int>(ref this.indices);
			}
			return mesh;
		}
	}

	// Token: 0x06002F04 RID: 12036 RVA: 0x0011C1E0 File Offset: 0x0011A3E0
	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
	{
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		this.indices = Pool.GetList<int>();
		this.vertices = Pool.GetList<Vector3>();
		this.normals = (normal ? Pool.GetList<Vector3>() : null);
		this.triangles = Pool.GetList<int>();
		this.Invoke();
	}

	// Token: 0x06002F05 RID: 12037 RVA: 0x0011C254 File Offset: 0x0011A454
	private void DoWork()
	{
		Vector3 vector = new Vector3((float)(this.width / 2), 0f, (float)(this.height / 2));
		Vector3 b = new Vector3(this.pivot.x - vector.x, 0f, this.pivot.z - vector.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int num = 0;
		for (int i = 0; i <= this.height; i++)
		{
			int j = 0;
			while (j <= this.width)
			{
				Vector3 worldPos = new Vector3((float)j, 0f, (float)i) + b;
				Vector3 item = new Vector3((float)j, 0f, (float)i) - vector;
				float num2 = heightMap.GetHeight(worldPos);
				if (num2 < -1f)
				{
					this.indices.Add(-1);
				}
				else if (this.alpha && alphaMap.GetAlpha(worldPos) < 0.1f)
				{
					this.indices.Add(-1);
				}
				else
				{
					if (this.normal)
					{
						Vector3 item2 = heightMap.GetNormal(worldPos);
						this.normals.Add(item2);
					}
					worldPos.y = (item.y = num2 - this.pivot.y);
					this.indices.Add(this.vertices.Count);
					this.vertices.Add(item);
				}
				j++;
				num++;
			}
		}
		int num3 = 0;
		int k = 0;
		while (k < this.height)
		{
			int l = 0;
			while (l < this.width)
			{
				int num4 = this.indices[num3];
				int num5 = this.indices[num3 + this.width + 1];
				int num6 = this.indices[num3 + 1];
				int num7 = this.indices[num3 + 1];
				int num8 = this.indices[num3 + this.width + 1];
				int num9 = this.indices[num3 + this.width + 2];
				if (num4 != -1 && num5 != -1 && num6 != -1)
				{
					this.triangles.Add(num4);
					this.triangles.Add(num5);
					this.triangles.Add(num6);
				}
				if (num7 != -1 && num8 != -1 && num9 != -1)
				{
					this.triangles.Add(num7);
					this.triangles.Add(num8);
					this.triangles.Add(num9);
				}
				l++;
				num3++;
			}
			k++;
			num3++;
		}
	}

	// Token: 0x06002F06 RID: 12038 RVA: 0x0011C500 File Offset: 0x0011A700
	private void Invoke()
	{
		this.worker = new Action(this.DoWork);
		this.worker.BeginInvoke(new AsyncCallback(this.Callback), null);
	}

	// Token: 0x06002F07 RID: 12039 RVA: 0x0011C52D File Offset: 0x0011A72D
	private void Callback(IAsyncResult result)
	{
		this.worker.EndInvoke(result);
		this.worker = null;
	}
}
