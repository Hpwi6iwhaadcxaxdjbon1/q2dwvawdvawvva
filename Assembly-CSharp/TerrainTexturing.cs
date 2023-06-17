using System;
using Rust;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x020006A8 RID: 1704
[ExecuteInEditMode]
public class TerrainTexturing : TerrainExtension
{
	// Token: 0x040027C8 RID: 10184
	private const int ShoreVectorDownscale = 3;

	// Token: 0x040027C9 RID: 10185
	private const int ShoreVectorBlurPasses = 0;

	// Token: 0x040027CA RID: 10186
	private float terrainSize;

	// Token: 0x040027CB RID: 10187
	private int shoreMapSize;

	// Token: 0x040027CC RID: 10188
	private float[] shoreDistances;

	// Token: 0x040027CD RID: 10189
	private Vector3[] shoreVectors;

	// Token: 0x040027CE RID: 10190
	public bool debugFoliageDisplacement;

	// Token: 0x040027CF RID: 10191
	private bool initialized;

	// Token: 0x040027D0 RID: 10192
	private static TerrainTexturing instance;

	// Token: 0x06003130 RID: 12592 RVA: 0x000063A5 File Offset: 0x000045A5
	private void InitializeBasePyramid()
	{
	}

	// Token: 0x06003131 RID: 12593 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ReleaseBasePyramid()
	{
	}

	// Token: 0x06003132 RID: 12594 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateBasePyramid()
	{
	}

	// Token: 0x06003133 RID: 12595 RVA: 0x000063A5 File Offset: 0x000045A5
	private void InitializeCoarseHeightSlope()
	{
	}

	// Token: 0x06003134 RID: 12596 RVA: 0x000063A5 File Offset: 0x000045A5
	private void ReleaseCoarseHeightSlope()
	{
	}

	// Token: 0x06003135 RID: 12597 RVA: 0x000063A5 File Offset: 0x000045A5
	private void UpdateCoarseHeightSlope()
	{
	}

	// Token: 0x17000405 RID: 1029
	// (get) Token: 0x06003136 RID: 12598 RVA: 0x0012652B File Offset: 0x0012472B
	public int ShoreMapSize
	{
		get
		{
			return this.shoreMapSize;
		}
	}

	// Token: 0x17000406 RID: 1030
	// (get) Token: 0x06003137 RID: 12599 RVA: 0x00126533 File Offset: 0x00124733
	public Vector3[] ShoreMap
	{
		get
		{
			return this.shoreVectors;
		}
	}

	// Token: 0x06003138 RID: 12600 RVA: 0x0012653C File Offset: 0x0012473C
	private void InitializeShoreVector()
	{
		int num = Mathf.ClosestPowerOfTwo(this.terrain.terrainData.heightmapResolution) >> 3;
		int num2 = num * num;
		this.terrainSize = Mathf.Max(this.terrain.terrainData.size.x, this.terrain.terrainData.size.z);
		this.shoreMapSize = num;
		this.shoreDistances = new float[num * num];
		this.shoreVectors = new Vector3[num * num];
		for (int i = 0; i < num2; i++)
		{
			this.shoreDistances[i] = 10000f;
			this.shoreVectors[i] = Vector3.one;
		}
	}

	// Token: 0x06003139 RID: 12601 RVA: 0x001265E8 File Offset: 0x001247E8
	private void GenerateShoreVector()
	{
		using (TimeWarning.New("GenerateShoreVector", 500))
		{
			this.GenerateShoreVector(out this.shoreDistances, out this.shoreVectors);
		}
	}

	// Token: 0x0600313A RID: 12602 RVA: 0x00126634 File Offset: 0x00124834
	private void ReleaseShoreVector()
	{
		this.shoreDistances = null;
		this.shoreVectors = null;
	}

	// Token: 0x0600313B RID: 12603 RVA: 0x00126644 File Offset: 0x00124844
	private void GenerateShoreVector(out float[] distances, out Vector3[] vectors)
	{
		float num = this.terrainSize / (float)this.shoreMapSize;
		Vector3 position = this.terrain.GetPosition();
		int num2 = LayerMask.NameToLayer("Terrain");
		NativeArray<RaycastHit> results = new NativeArray<RaycastHit>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(this.shoreMapSize * this.shoreMapSize, Allocator.TempJob, NativeArrayOptions.ClearMemory);
		for (int i = 0; i < this.shoreMapSize; i++)
		{
			for (int j = 0; j < this.shoreMapSize; j++)
			{
				float x = ((float)j + 0.5f) * num;
				float z = ((float)i + 0.5f) * num;
				Vector3 from = new Vector3(position.x, 0f, position.z) + new Vector3(x, 1000f, z);
				Vector3 down = Vector3.down;
				commands[i * this.shoreMapSize + j] = new RaycastCommand(from, down, float.MaxValue, -5, 1);
			}
		}
		RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle)).Complete();
		byte[] array = new byte[this.shoreMapSize * this.shoreMapSize];
		distances = new float[this.shoreMapSize * this.shoreMapSize];
		vectors = new Vector3[this.shoreMapSize * this.shoreMapSize];
		int k = 0;
		int num3 = 0;
		while (k < this.shoreMapSize)
		{
			int l = 0;
			while (l < this.shoreMapSize)
			{
				bool flag = results[k * this.shoreMapSize + l].collider.gameObject.layer == num2;
				array[num3] = (flag ? byte.MaxValue : 0);
				distances[num3] = (float)(flag ? 256 : 0);
				l++;
				num3++;
			}
			k++;
		}
		byte b = 127;
		DistanceField.Generate(this.shoreMapSize, b, array, ref distances);
		DistanceField.ApplyGaussianBlur(this.shoreMapSize, distances, 0);
		DistanceField.GenerateVectors(this.shoreMapSize, distances, ref vectors);
		results.Dispose();
		commands.Dispose();
	}

	// Token: 0x0600313C RID: 12604 RVA: 0x0012685C File Offset: 0x00124A5C
	public float GetCoarseDistanceToShore(Vector3 pos)
	{
		Vector2 uv;
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseDistanceToShore(uv);
	}

	// Token: 0x0600313D RID: 12605 RVA: 0x001268B8 File Offset: 0x00124AB8
	public float GetCoarseDistanceToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = (num3 < (float)num2) ? 1 : 0;
		int num10 = (num4 < (float)num2) ? num : 0;
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		float num15 = this.shoreDistances[num11];
		float num16 = this.shoreDistances[num12];
		float num17 = this.shoreDistances[num13];
		float num18 = this.shoreDistances[num14];
		float num19 = (num16 - num15) * num7 + num15;
		return ((num18 - num17) * num7 + num17 - num19) * num8 + num19;
	}

	// Token: 0x0600313E RID: 12606 RVA: 0x0012699C File Offset: 0x00124B9C
	public Vector3 GetCoarseVectorToShore(Vector3 pos)
	{
		Vector2 uv;
		uv.x = (pos.x - TerrainMeta.Position.x) * TerrainMeta.OneOverSize.x;
		uv.y = (pos.z - TerrainMeta.Position.z) * TerrainMeta.OneOverSize.z;
		return this.GetCoarseVectorToShore(uv);
	}

	// Token: 0x0600313F RID: 12607 RVA: 0x001269F8 File Offset: 0x00124BF8
	public Vector3 GetCoarseVectorToShore(Vector2 uv)
	{
		int num = this.shoreMapSize;
		int num2 = num - 1;
		float num3 = uv.x * (float)num2;
		float num4 = uv.y * (float)num2;
		int num5 = (int)num3;
		int num6 = (int)num4;
		float num7 = num3 - (float)num5;
		float num8 = num4 - (float)num6;
		num5 = ((num5 >= 0) ? num5 : 0);
		num6 = ((num6 >= 0) ? num6 : 0);
		num5 = ((num5 <= num2) ? num5 : num2);
		num6 = ((num6 <= num2) ? num6 : num2);
		int num9 = (num3 < (float)num2) ? 1 : 0;
		int num10 = (num4 < (float)num2) ? num : 0;
		int num11 = num6 * num + num5;
		int num12 = num11 + num9;
		int num13 = num11 + num10;
		int num14 = num13 + num9;
		Vector3 vector = this.shoreVectors[num11];
		Vector3 vector2 = this.shoreVectors[num12];
		Vector3 vector3 = this.shoreVectors[num13];
		Vector3 vector4 = this.shoreVectors[num14];
		Vector3 vector5;
		vector5.x = (vector2.x - vector.x) * num7 + vector.x;
		vector5.y = (vector2.y - vector.y) * num7 + vector.y;
		vector5.z = (vector2.z - vector.z) * num7 + vector.z;
		Vector3 vector6;
		vector6.x = (vector4.x - vector3.x) * num7 + vector3.x;
		vector6.y = (vector4.y - vector3.y) * num7 + vector3.y;
		vector6.z = (vector4.z - vector3.z) * num7 + vector3.z;
		float x = (vector6.x - vector5.x) * num8 + vector5.x;
		float y = (vector6.y - vector5.y) * num8 + vector5.y;
		float z = (vector6.z - vector5.z) * num8 + vector5.z;
		return new Vector3(x, y, z);
	}

	// Token: 0x17000407 RID: 1031
	// (get) Token: 0x06003140 RID: 12608 RVA: 0x00126BF0 File Offset: 0x00124DF0
	public static TerrainTexturing Instance
	{
		get
		{
			return TerrainTexturing.instance;
		}
	}

	// Token: 0x06003141 RID: 12609 RVA: 0x00126BF7 File Offset: 0x00124DF7
	private void CheckInstance()
	{
		TerrainTexturing.instance = ((TerrainTexturing.instance != null) ? TerrainTexturing.instance : this);
	}

	// Token: 0x06003142 RID: 12610 RVA: 0x00126C13 File Offset: 0x00124E13
	private void Awake()
	{
		this.CheckInstance();
	}

	// Token: 0x06003143 RID: 12611 RVA: 0x00126C1B File Offset: 0x00124E1B
	public override void Setup()
	{
		this.InitializeShoreVector();
	}

	// Token: 0x06003144 RID: 12612 RVA: 0x00126C24 File Offset: 0x00124E24
	public override void PostSetup()
	{
		TerrainMeta component = base.GetComponent<TerrainMeta>();
		if (component == null || component.config == null)
		{
			Debug.LogError("[TerrainTexturing] Missing TerrainMeta or TerrainConfig not assigned.");
			return;
		}
		this.Shutdown();
		this.InitializeCoarseHeightSlope();
		this.GenerateShoreVector();
		this.initialized = true;
	}

	// Token: 0x06003145 RID: 12613 RVA: 0x00126C73 File Offset: 0x00124E73
	private void Shutdown()
	{
		this.ReleaseBasePyramid();
		this.ReleaseCoarseHeightSlope();
		this.ReleaseShoreVector();
		this.initialized = false;
	}

	// Token: 0x06003146 RID: 12614 RVA: 0x00126C13 File Offset: 0x00124E13
	private void OnEnable()
	{
		this.CheckInstance();
	}

	// Token: 0x06003147 RID: 12615 RVA: 0x00126C8E File Offset: 0x00124E8E
	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		this.Shutdown();
	}

	// Token: 0x06003148 RID: 12616 RVA: 0x00126C9E File Offset: 0x00124E9E
	private void Update()
	{
		if (!this.initialized)
		{
			return;
		}
		this.UpdateBasePyramid();
		this.UpdateCoarseHeightSlope();
	}
}
