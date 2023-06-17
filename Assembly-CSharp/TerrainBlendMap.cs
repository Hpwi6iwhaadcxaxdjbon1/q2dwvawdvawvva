using System;
using UnityEngine;

// Token: 0x02000698 RID: 1688
public class TerrainBlendMap : TerrainMap<byte>
{
	// Token: 0x0400277F RID: 10111
	public Texture2D BlendTexture;

	// Token: 0x06003013 RID: 12307 RVA: 0x001209DC File Offset: 0x0011EBDC
	public override void Setup()
	{
		if (!(this.BlendTexture != null))
		{
			this.res = this.terrain.terrainData.alphamapResolution;
			this.src = (this.dst = new byte[this.res * this.res]);
			for (int i = 0; i < this.res; i++)
			{
				for (int j = 0; j < this.res; j++)
				{
					this.dst[i * this.res + j] = 0;
				}
			}
			return;
		}
		if (this.BlendTexture.width == this.BlendTexture.height)
		{
			this.res = this.BlendTexture.width;
			this.src = (this.dst = new byte[this.res * this.res]);
			Color32[] pixels = this.BlendTexture.GetPixels32();
			int k = 0;
			int num = 0;
			while (k < this.res)
			{
				int l = 0;
				while (l < this.res)
				{
					this.dst[k * this.res + l] = pixels[num].a;
					l++;
					num++;
				}
				k++;
			}
			return;
		}
		Debug.LogError("Invalid alpha texture: " + this.BlendTexture.name);
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x00120B30 File Offset: 0x0011ED30
	public void GenerateTextures()
	{
		this.BlendTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, true, true);
		this.BlendTexture.name = "BlendTexture";
		this.BlendTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.BlendTexture.SetPixels32(col);
	}

	// Token: 0x06003015 RID: 12309 RVA: 0x00120BC1 File Offset: 0x0011EDC1
	public void ApplyTextures()
	{
		this.BlendTexture.Apply(true, false);
		this.BlendTexture.Compress(false);
		this.BlendTexture.Apply(false, true);
	}

	// Token: 0x06003016 RID: 12310 RVA: 0x00120BEC File Offset: 0x0011EDEC
	public float GetAlpha(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(normX, normZ);
	}

	// Token: 0x06003017 RID: 12311 RVA: 0x00120C1C File Offset: 0x0011EE1C
	public float GetAlpha(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float a = Mathf.Lerp(this.GetAlpha(num4, num5), this.GetAlpha(x, num5), num2 - (float)num4);
		float b = Mathf.Lerp(this.GetAlpha(num4, z), this.GetAlpha(x, z), num2 - (float)num4);
		return Mathf.Lerp(a, b, num3 - (float)num5);
	}

	// Token: 0x06003018 RID: 12312 RVA: 0x0012002E File Offset: 0x0011E22E
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003019 RID: 12313 RVA: 0x00120CB0 File Offset: 0x0011EEB0
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a);
	}

	// Token: 0x0600301A RID: 12314 RVA: 0x00120CE0 File Offset: 0x0011EEE0
	public void SetAlpha(float normX, float normZ, float a)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetAlpha(x, z, a);
	}

	// Token: 0x0600301B RID: 12315 RVA: 0x0012009E File Offset: 0x0011E29E
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x0600301C RID: 12316 RVA: 0x00120D06 File Offset: 0x0011EF06
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x0600301D RID: 12317 RVA: 0x00120D20 File Offset: 0x0011EF20
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a, opacity, radius, fade);
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x00120D54 File Offset: 0x0011EF54
	public void SetAlpha(float normX, float normZ, float a, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			lerp *= opacity;
			if (lerp > 0f)
			{
				this.SetAlpha(x, z, a, lerp);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
