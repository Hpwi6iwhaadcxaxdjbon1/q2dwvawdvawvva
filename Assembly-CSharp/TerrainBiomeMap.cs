using System;
using UnityEngine;

// Token: 0x02000697 RID: 1687
public class TerrainBiomeMap : TerrainMap<byte>
{
	// Token: 0x0400277D RID: 10109
	public Texture2D BiomeTexture;

	// Token: 0x0400277E RID: 10110
	internal int num;

	// Token: 0x06002FFB RID: 12283 RVA: 0x00120154 File Offset: 0x0011E354
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = 4;
		this.src = (this.dst = new byte[this.num * this.res * this.res]);
		if (this.BiomeTexture != null)
		{
			if (this.BiomeTexture.width == this.BiomeTexture.height && this.BiomeTexture.width == this.res)
			{
				Color32[] pixels = this.BiomeTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						byte[] dst = this.dst;
						int res = this.res;
						dst[(0 + i) * this.res + j] = color.r;
						this.dst[(this.res + i) * this.res + j] = color.g;
						this.dst[(2 * this.res + i) * this.res + j] = color.b;
						this.dst[(3 * this.res + i) * this.res + j] = color.a;
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid biome texture: " + this.BiomeTexture.name);
		}
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x001202D8 File Offset: 0x0011E4D8
	public void GenerateTextures()
	{
		this.BiomeTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.BiomeTexture.name = "BiomeTexture";
		this.BiomeTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte[] src = this.src;
				int res = this.res;
				byte r = src[(0 + z) * this.res + i];
				byte g = this.src[(this.res + z) * this.res + i];
				byte b = this.src[(2 * this.res + z) * this.res + i];
				byte a = this.src[(3 * this.res + z) * this.res + i];
				col[z * this.res + i] = new Color32(r, g, b, a);
			}
		});
		this.BiomeTexture.SetPixels32(col);
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x00120369 File Offset: 0x0011E569
	public void ApplyTextures()
	{
		this.BiomeTexture.Apply(true, false);
		this.BiomeTexture.Compress(false);
		this.BiomeTexture.Apply(false, true);
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x00120394 File Offset: 0x0011E594
	public float GetBiomeMax(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMax(normX, normZ, mask);
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x001203C4 File Offset: 0x0011E5C4
	public float GetBiomeMax(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiomeMax(x, z, mask);
	}

	// Token: 0x06003000 RID: 12288 RVA: 0x001203EC File Offset: 0x0011E5EC
	public float GetBiomeMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return (float)b;
	}

	// Token: 0x06003001 RID: 12289 RVA: 0x0012043C File Offset: 0x0011E63C
	public int GetBiomeMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiomeMaxIndex(normX, normZ, mask);
	}

	// Token: 0x06003002 RID: 12290 RVA: 0x0012046C File Offset: 0x0011E66C
	public int GetBiomeMaxIndex(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiomeMaxIndex(x, z, mask);
	}

	// Token: 0x06003003 RID: 12291 RVA: 0x00120494 File Offset: 0x0011E694
	public int GetBiomeMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int result = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
					result = i;
				}
			}
		}
		return result;
	}

	// Token: 0x06003004 RID: 12292 RVA: 0x001204E4 File Offset: 0x0011E6E4
	public int GetBiomeMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(worldPos, mask));
	}

	// Token: 0x06003005 RID: 12293 RVA: 0x001204F3 File Offset: 0x0011E6F3
	public int GetBiomeMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(normX, normZ, mask));
	}

	// Token: 0x06003006 RID: 12294 RVA: 0x00120503 File Offset: 0x0011E703
	public int GetBiomeMaxType(int x, int z, int mask = -1)
	{
		return TerrainBiome.IndexToType(this.GetBiomeMaxIndex(x, z, mask));
	}

	// Token: 0x06003007 RID: 12295 RVA: 0x00120514 File Offset: 0x0011E714
	public float GetBiome(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetBiome(normX, normZ, mask);
	}

	// Token: 0x06003008 RID: 12296 RVA: 0x00120544 File Offset: 0x0011E744
	public float GetBiome(float normX, float normZ, int mask)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetBiome(x, z, mask);
	}

	// Token: 0x06003009 RID: 12297 RVA: 0x0012056C File Offset: 0x0011E76C
	public float GetBiome(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainBiome.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainBiome.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x0600300A RID: 12298 RVA: 0x001205EC File Offset: 0x0011E7EC
	public void SetBiome(Vector3 worldPos, int id)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(normX, normZ, id);
	}

	// Token: 0x0600300B RID: 12299 RVA: 0x0012061C File Offset: 0x0011E81C
	public void SetBiome(float normX, float normZ, int id)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBiome(x, z, id);
	}

	// Token: 0x0600300C RID: 12300 RVA: 0x00120644 File Offset: 0x0011E844
	public void SetBiome(int x, int z, int id)
	{
		int num = TerrainBiome.TypeToIndex(id);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = byte.MaxValue;
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = 0;
			}
		}
	}

	// Token: 0x0600300D RID: 12301 RVA: 0x001206AC File Offset: 0x0011E8AC
	public void SetBiome(Vector3 worldPos, int id, float v)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetBiome(normX, normZ, id, v);
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x001206DC File Offset: 0x0011E8DC
	public void SetBiome(float normX, float normZ, int id, float v)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetBiome(x, z, id, v);
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x00120704 File Offset: 0x0011E904
	public void SetBiome(int x, int z, int id, float v)
	{
		this.SetBiome(x, z, id, this.GetBiome(x, z, id), v);
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x0012071C File Offset: 0x0011E91C
	public void SetBiomeRaw(int x, int z, Vector4 v, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v.x + v.y + v.z + v.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v.w * opacity);
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x00120934 File Offset: 0x0011EB34
	private void SetBiome(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainBiome.TypeToIndex(id);
		if (old_val >= 1f)
		{
			return;
		}
		float num2 = (1f - new_val) / (1f - old_val);
		for (int i = 0; i < this.num; i++)
		{
			if (i == num)
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(new_val);
			}
			else
			{
				this.dst[(i * this.res + z) * this.res + x] = BitUtility.Float2Byte(num2 * BitUtility.Byte2Float((int)this.dst[(i * this.res + z) * this.res + x]));
			}
		}
	}
}
