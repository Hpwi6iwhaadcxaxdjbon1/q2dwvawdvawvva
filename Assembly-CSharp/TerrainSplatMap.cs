using System;
using UnityEngine;

// Token: 0x0200069E RID: 1694
public class TerrainSplatMap : TerrainMap<byte>
{
	// Token: 0x04002788 RID: 10120
	public Texture2D SplatTexture0;

	// Token: 0x04002789 RID: 10121
	public Texture2D SplatTexture1;

	// Token: 0x0400278A RID: 10122
	internal int num;

	// Token: 0x06003083 RID: 12419 RVA: 0x001231AC File Offset: 0x001213AC
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.num = this.config.Splats.Length;
		this.src = (this.dst = new byte[this.num * this.res * this.res]);
		if (this.SplatTexture0 != null)
		{
			if (this.SplatTexture0.width == this.SplatTexture0.height && this.SplatTexture0.width == this.res)
			{
				Color32[] pixels = this.SplatTexture0.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 color = pixels[num];
						if (this.num > 0)
						{
							byte[] dst = this.dst;
							int res = this.res;
							dst[(0 + i) * this.res + j] = color.r;
						}
						if (this.num > 1)
						{
							this.dst[(this.res + i) * this.res + j] = color.g;
						}
						if (this.num > 2)
						{
							this.dst[(2 * this.res + i) * this.res + j] = color.b;
						}
						if (this.num > 3)
						{
							this.dst[(3 * this.res + i) * this.res + j] = color.a;
						}
						j++;
						num++;
					}
					i++;
				}
			}
			else
			{
				Debug.LogError("Invalid splat texture: " + this.SplatTexture0.name, this.SplatTexture0);
			}
		}
		if (this.SplatTexture1 != null)
		{
			if (this.SplatTexture1.width == this.SplatTexture1.height && this.SplatTexture1.width == this.res && this.num > 5)
			{
				Color32[] pixels2 = this.SplatTexture1.GetPixels32();
				int k = 0;
				int num2 = 0;
				while (k < this.res)
				{
					int l = 0;
					while (l < this.res)
					{
						Color32 color2 = pixels2[num2];
						if (this.num > 4)
						{
							this.dst[(4 * this.res + k) * this.res + l] = color2.r;
						}
						if (this.num > 5)
						{
							this.dst[(5 * this.res + k) * this.res + l] = color2.g;
						}
						if (this.num > 6)
						{
							this.dst[(6 * this.res + k) * this.res + l] = color2.b;
						}
						if (this.num > 7)
						{
							this.dst[(7 * this.res + k) * this.res + l] = color2.a;
						}
						l++;
						num2++;
					}
					k++;
				}
				return;
			}
			Debug.LogError("Invalid splat texture: " + this.SplatTexture1.name, this.SplatTexture1);
		}
	}

	// Token: 0x06003084 RID: 12420 RVA: 0x001234DC File Offset: 0x001216DC
	public void GenerateTextures()
	{
		this.SplatTexture0 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.SplatTexture0.name = "SplatTexture0";
		this.SplatTexture0.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols2 = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b;
				if (this.num <= 0)
				{
					b = 0;
				}
				else
				{
					byte[] src = this.src;
					int res = this.res;
					b = src[(0 + z) * this.res + i];
				}
				byte r = b;
				byte g = (this.num > 1) ? this.src[(this.res + z) * this.res + i] : 0;
				byte b2 = (this.num > 2) ? this.src[(2 * this.res + z) * this.res + i] : 0;
				byte a = (this.num > 3) ? this.src[(3 * this.res + z) * this.res + i] : 0;
				cols2[z * this.res + i] = new Color32(r, g, b2, a);
			}
		});
		this.SplatTexture0.SetPixels32(cols2);
		this.SplatTexture1 = new Texture2D(this.res, this.res, TextureFormat.RGBA32, false, true);
		this.SplatTexture1.name = "SplatTexture1";
		this.SplatTexture1.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte r = (this.num > 4) ? this.src[(4 * this.res + z) * this.res + i] : 0;
				byte g = (this.num > 5) ? this.src[(5 * this.res + z) * this.res + i] : 0;
				byte b = (this.num > 6) ? this.src[(6 * this.res + z) * this.res + i] : 0;
				byte a = (this.num > 7) ? this.src[(7 * this.res + z) * this.res + i] : 0;
				cols[z * this.res + i] = new Color32(r, g, b, a);
			}
		});
		this.SplatTexture1.SetPixels32(cols);
	}

	// Token: 0x06003085 RID: 12421 RVA: 0x001235F1 File Offset: 0x001217F1
	public void ApplyTextures()
	{
		this.SplatTexture0.Apply(true, true);
		this.SplatTexture1.Apply(true, true);
	}

	// Token: 0x06003086 RID: 12422 RVA: 0x00123610 File Offset: 0x00121810
	public float GetSplatMax(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMax(normX, normZ, mask);
	}

	// Token: 0x06003087 RID: 12423 RVA: 0x00123640 File Offset: 0x00121840
	public float GetSplatMax(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetSplatMax(x, z, mask);
	}

	// Token: 0x06003088 RID: 12424 RVA: 0x00123668 File Offset: 0x00121868
	public float GetSplatMax(int x, int z, int mask = -1)
	{
		byte b = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				byte b2 = this.src[(i * this.res + z) * this.res + x];
				if (b2 >= b)
				{
					b = b2;
				}
			}
		}
		return BitUtility.Byte2Float((int)b);
	}

	// Token: 0x06003089 RID: 12425 RVA: 0x001236BC File Offset: 0x001218BC
	public int GetSplatMaxIndex(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplatMaxIndex(normX, normZ, mask);
	}

	// Token: 0x0600308A RID: 12426 RVA: 0x001236EC File Offset: 0x001218EC
	public int GetSplatMaxIndex(float normX, float normZ, int mask = -1)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		return this.GetSplatMaxIndex(x, z, mask);
	}

	// Token: 0x0600308B RID: 12427 RVA: 0x00123714 File Offset: 0x00121914
	public int GetSplatMaxIndex(int x, int z, int mask = -1)
	{
		byte b = 0;
		int result = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
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

	// Token: 0x0600308C RID: 12428 RVA: 0x00123764 File Offset: 0x00121964
	public int GetSplatMaxType(Vector3 worldPos, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(worldPos, mask));
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x00123773 File Offset: 0x00121973
	public int GetSplatMaxType(float normX, float normZ, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(normX, normZ, mask));
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x00123783 File Offset: 0x00121983
	public int GetSplatMaxType(int x, int z, int mask = -1)
	{
		return TerrainSplat.IndexToType(this.GetSplatMaxIndex(x, z, mask));
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x00123794 File Offset: 0x00121994
	public float GetSplat(Vector3 worldPos, int mask)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetSplat(normX, normZ, mask);
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x001237C4 File Offset: 0x001219C4
	public float GetSplat(float normX, float normZ, int mask)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetSplat(num4, num5, mask), this.GetSplat(x, num5, mask), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetSplat(num4, z, mask), this.GetSplat(x, z, mask), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x0012385C File Offset: 0x00121A5C
	public float GetSplat(int x, int z, int mask)
	{
		if (Mathf.IsPowerOfTwo(mask))
		{
			return BitUtility.Byte2Float((int)this.src[(TerrainSplat.TypeToIndex(mask) * this.res + z) * this.res + x]);
		}
		int num = 0;
		for (int i = 0; i < this.num; i++)
		{
			if ((TerrainSplat.IndexToType(i) & mask) != 0)
			{
				num += (int)this.src[(i * this.res + z) * this.res + x];
			}
		}
		return Mathf.Clamp01(BitUtility.Byte2Float(num));
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x001238DC File Offset: 0x00121ADC
	public void SetSplat(Vector3 worldPos, int id)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(normX, normZ, id);
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x0012390C File Offset: 0x00121B0C
	public void SetSplat(float normX, float normZ, int id)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetSplat(x, z, id);
	}

	// Token: 0x06003094 RID: 12436 RVA: 0x00123934 File Offset: 0x00121B34
	public void SetSplat(int x, int z, int id)
	{
		int num = TerrainSplat.TypeToIndex(id);
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

	// Token: 0x06003095 RID: 12437 RVA: 0x0012399C File Offset: 0x00121B9C
	public void SetSplat(Vector3 worldPos, int id, float v)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(normX, normZ, id, v);
	}

	// Token: 0x06003096 RID: 12438 RVA: 0x001239CC File Offset: 0x00121BCC
	public void SetSplat(float normX, float normZ, int id, float v)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetSplat(x, z, id, v);
	}

	// Token: 0x06003097 RID: 12439 RVA: 0x001239F4 File Offset: 0x00121BF4
	public void SetSplat(int x, int z, int id, float v)
	{
		this.SetSplat(x, z, id, this.GetSplat(x, z, id), v);
	}

	// Token: 0x06003098 RID: 12440 RVA: 0x00123A0C File Offset: 0x00121C0C
	public void SetSplatRaw(int x, int z, Vector4 v1, Vector4 v2, float opacity)
	{
		if (opacity == 0f)
		{
			return;
		}
		float num = Mathf.Clamp01(v1.x + v1.y + v1.z + v1.w + v2.x + v2.y + v2.z + v2.w);
		if (num == 0f)
		{
			return;
		}
		float num2 = 1f - opacity * num;
		if (num2 == 0f && opacity == 1f)
		{
			byte[] dst = this.dst;
			int res = this.res;
			dst[(0 + z) * this.res + x] = BitUtility.Float2Byte(v1.x);
			this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.y);
			this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.z);
			this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v1.w);
			this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.x);
			this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.y);
			this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.z);
			this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(v2.w);
			return;
		}
		byte[] dst2 = this.dst;
		int res2 = this.res;
		int num3 = (0 + z) * this.res + x;
		byte[] src = this.src;
		int res3 = this.res;
		dst2[num3] = BitUtility.Float2Byte(BitUtility.Byte2Float(src[(0 + z) * this.res + x]) * num2 + v1.x * opacity);
		this.dst[(this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(this.res + z) * this.res + x]) * num2 + v1.y * opacity);
		this.dst[(2 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(2 * this.res + z) * this.res + x]) * num2 + v1.z * opacity);
		this.dst[(3 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(3 * this.res + z) * this.res + x]) * num2 + v1.w * opacity);
		this.dst[(4 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(4 * this.res + z) * this.res + x]) * num2 + v2.x * opacity);
		this.dst[(5 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(5 * this.res + z) * this.res + x]) * num2 + v2.y * opacity);
		this.dst[(6 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(6 * this.res + z) * this.res + x]) * num2 + v2.z * opacity);
		this.dst[(7 * this.res + z) * this.res + x] = BitUtility.Float2Byte(BitUtility.Byte2Float((int)this.src[(7 * this.res + z) * this.res + x]) * num2 + v2.w * opacity);
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x00123E04 File Offset: 0x00122004
	public void SetSplat(Vector3 worldPos, int id, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetSplat(normX, normZ, id, opacity, radius, fade);
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x00123E38 File Offset: 0x00122038
	public void SetSplat(float normX, float normZ, int id, float opacity, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = BitUtility.Byte2Float((int)this.dst[(idx * this.res + z) * this.res + x]);
				float new_val = Mathf.Lerp(num, 1f, lerp * opacity);
				this.SetSplat(x, z, id, num, new_val);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x00123E8C File Offset: 0x0012208C
	public void AddSplat(Vector3 worldPos, int id, float delta, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddSplat(normX, normZ, id, delta, radius, fade);
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x00123EC0 File Offset: 0x001220C0
	public void AddSplat(float normX, float normZ, int id, float delta, float radius, float fade = 0f)
	{
		int idx = TerrainSplat.TypeToIndex(id);
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				float num = BitUtility.Byte2Float((int)this.dst[(idx * this.res + z) * this.res + x]);
				float new_val = Mathf.Clamp01(num + lerp * delta);
				this.SetSplat(x, z, id, num, new_val);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x00123F14 File Offset: 0x00122114
	private void SetSplat(int x, int z, int id, float old_val, float new_val)
	{
		int num = TerrainSplat.TypeToIndex(id);
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
