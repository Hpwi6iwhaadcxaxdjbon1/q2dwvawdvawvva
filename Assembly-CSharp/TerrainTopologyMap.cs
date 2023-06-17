using System;
using UnityEngine;

// Token: 0x0200069F RID: 1695
public class TerrainTopologyMap : TerrainMap<int>
{
	// Token: 0x0400278B RID: 10123
	public Texture2D TopologyTexture;

	// Token: 0x0600309F RID: 12447 RVA: 0x00123FBC File Offset: 0x001221BC
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new int[this.res * this.res]);
		if (this.TopologyTexture != null)
		{
			if (this.TopologyTexture.width == this.TopologyTexture.height && this.TopologyTexture.width == this.res)
			{
				Color32[] pixels = this.TopologyTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						this.dst[i * this.res + j] = BitUtility.DecodeInt(pixels[num]);
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid topology texture: " + this.TopologyTexture.name);
		}
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x001240B0 File Offset: 0x001222B0
	public void GenerateTextures()
	{
		this.TopologyTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.TopologyTexture.name = "TopologyTexture";
		this.TopologyTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				col[z * this.res + i] = BitUtility.EncodeInt(this.src[z * this.res + i]);
			}
		});
		this.TopologyTexture.SetPixels32(col);
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x00124141 File Offset: 0x00122341
	public void ApplyTextures()
	{
		this.TopologyTexture.Apply(false, true);
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x00124150 File Offset: 0x00122350
	public bool GetTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ, mask);
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x00124180 File Offset: 0x00122380
	public bool GetTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetTopology(x, z, mask);
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x001241A6 File Offset: 0x001223A6
	public bool GetTopology(int x, int z, int mask)
	{
		return (this.src[z * this.res + x] & mask) != 0;
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x001241C0 File Offset: 0x001223C0
	public int GetTopology(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ);
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x001241F0 File Offset: 0x001223F0
	public int GetTopology(float normX, float normZ)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetTopology(x, z);
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x00124218 File Offset: 0x00122418
	public int GetTopologyFast(Vector2 uv)
	{
		int num = this.res - 1;
		int num2 = (int)(uv.x * (float)this.res);
		int num3 = (int)(uv.y * (float)this.res);
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 <= num) ? num2 : num);
		num3 = ((num3 <= num) ? num3 : num);
		return this.src[num3 * this.res + num2];
	}

	// Token: 0x060030A8 RID: 12456 RVA: 0x00124283 File Offset: 0x00122483
	public int GetTopology(int x, int z)
	{
		return this.src[z * this.res + x];
	}

	// Token: 0x060030A9 RID: 12457 RVA: 0x00124298 File Offset: 0x00122498
	public void SetTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(normX, normZ, mask);
	}

	// Token: 0x060030AA RID: 12458 RVA: 0x001242C8 File Offset: 0x001224C8
	public void SetTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetTopology(x, z, mask);
	}

	// Token: 0x060030AB RID: 12459 RVA: 0x001242EE File Offset: 0x001224EE
	public void SetTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] = mask;
	}

	// Token: 0x060030AC RID: 12460 RVA: 0x00124304 File Offset: 0x00122504
	public void AddTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(normX, normZ, mask);
	}

	// Token: 0x060030AD RID: 12461 RVA: 0x00124334 File Offset: 0x00122534
	public void AddTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.AddTopology(x, z, mask);
	}

	// Token: 0x060030AE RID: 12462 RVA: 0x0012435A File Offset: 0x0012255A
	public void AddTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] |= mask;
	}

	// Token: 0x060030AF RID: 12463 RVA: 0x00124378 File Offset: 0x00122578
	public void RemoveTopology(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.RemoveTopology(normX, normZ, mask);
	}

	// Token: 0x060030B0 RID: 12464 RVA: 0x001243A8 File Offset: 0x001225A8
	public void RemoveTopology(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.RemoveTopology(x, z, mask);
	}

	// Token: 0x060030B1 RID: 12465 RVA: 0x001243CE File Offset: 0x001225CE
	public void RemoveTopology(int x, int z, int mask)
	{
		this.dst[z * this.res + x] &= ~mask;
	}

	// Token: 0x060030B2 RID: 12466 RVA: 0x001243EC File Offset: 0x001225EC
	public int GetTopology(Vector3 worldPos, float radius)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetTopology(normX, normZ, radius);
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x0012441C File Offset: 0x0012261C
	public int GetTopology(float normX, float normZ, float radius)
	{
		int num = 0;
		float num2 = TerrainMeta.OneOverSize.x * radius;
		int num3 = base.Index(normX - num2);
		int num4 = base.Index(normX + num2);
		int num5 = base.Index(normZ - num2);
		int num6 = base.Index(normZ + num2);
		for (int i = num5; i <= num6; i++)
		{
			for (int j = num3; j <= num4; j++)
			{
				num |= this.src[i * this.res + j];
			}
		}
		return num;
	}

	// Token: 0x060030B4 RID: 12468 RVA: 0x00124498 File Offset: 0x00122698
	public void SetTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetTopology(normX, normZ, mask, radius, fade);
	}

	// Token: 0x060030B5 RID: 12469 RVA: 0x001244CC File Offset: 0x001226CC
	public void SetTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] = mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x060030B6 RID: 12470 RVA: 0x00124508 File Offset: 0x00122708
	public void AddTopology(Vector3 worldPos, int mask, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddTopology(normX, normZ, mask, radius, fade);
	}

	// Token: 0x060030B7 RID: 12471 RVA: 0x0012453C File Offset: 0x0012273C
	public void AddTopology(float normX, float normZ, int mask, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if ((double)lerp > 0.5)
			{
				this.dst[z * this.res + x] |= mask;
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
