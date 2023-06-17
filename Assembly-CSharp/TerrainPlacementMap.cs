using System;
using UnityEngine;

// Token: 0x0200069D RID: 1693
public class TerrainPlacementMap : TerrainMap<bool>
{
	// Token: 0x04002787 RID: 10119
	private bool isEnabled;

	// Token: 0x06003072 RID: 12402 RVA: 0x00122EE4 File Offset: 0x001210E4
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new bool[this.res * this.res]);
		this.Enable();
	}

	// Token: 0x06003073 RID: 12403 RVA: 0x00122F2E File Offset: 0x0012112E
	public override void PostSetup()
	{
		this.res = 0;
		this.src = null;
		this.Disable();
	}

	// Token: 0x06003074 RID: 12404 RVA: 0x00122F44 File Offset: 0x00121144
	public void Enable()
	{
		this.isEnabled = true;
	}

	// Token: 0x06003075 RID: 12405 RVA: 0x00122F4D File Offset: 0x0012114D
	public void Disable()
	{
		this.isEnabled = false;
	}

	// Token: 0x06003076 RID: 12406 RVA: 0x00122F58 File Offset: 0x00121158
	public void Reset()
	{
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				this.dst[i * this.res + j] = false;
			}
		}
	}

	// Token: 0x06003077 RID: 12407 RVA: 0x00122F9C File Offset: 0x0012119C
	public bool GetBlocked(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBlocked(normX, normZ);
	}

	// Token: 0x06003078 RID: 12408 RVA: 0x00122FCC File Offset: 0x001211CC
	public bool GetBlocked(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBlocked(x, z);
	}

	// Token: 0x06003079 RID: 12409 RVA: 0x00122FF1 File Offset: 0x001211F1
	public bool GetBlocked(int x, int z)
	{
		return this.isEnabled && this.res > 0 && this.src[z * this.res + x];
	}

	// Token: 0x0600307A RID: 12410 RVA: 0x00123018 File Offset: 0x00121218
	public void SetBlocked(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBlocked(normX, normZ);
	}

	// Token: 0x0600307B RID: 12411 RVA: 0x00123048 File Offset: 0x00121248
	public void SetBlocked(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBlocked(x, z);
	}

	// Token: 0x0600307C RID: 12412 RVA: 0x0012306D File Offset: 0x0012126D
	public void SetBlocked(int x, int z)
	{
		this.dst[z * this.res + x] = true;
	}

	// Token: 0x0600307D RID: 12413 RVA: 0x00123084 File Offset: 0x00121284
	public bool GetBlocked(Vector3 worldPos, float radius)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBlocked(normX, normZ, radius);
	}

	// Token: 0x0600307E RID: 12414 RVA: 0x001230B4 File Offset: 0x001212B4
	public bool GetBlocked(float normX, float normZ, float radius)
	{
		float num = TerrainMeta.OneOverSize.x * radius;
		int num2 = base.Index(normX - num);
		int num3 = base.Index(normX + num);
		int num4 = base.Index(normZ - num);
		int num5 = base.Index(normZ + num);
		for (int i = num4; i <= num5; i++)
		{
			for (int j = num2; j <= num3; j++)
			{
				if (this.src[i * this.res + j])
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x0600307F RID: 12415 RVA: 0x0012312C File Offset: 0x0012132C
	public void SetBlocked(Vector3 worldPos, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBlocked(normX, normZ, radius, fade);
	}

	// Token: 0x06003080 RID: 12416 RVA: 0x0012315C File Offset: 0x0012135C
	public void SetBlocked(float normX, float normZ, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = true;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
