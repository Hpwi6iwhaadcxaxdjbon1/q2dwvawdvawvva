using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000696 RID: 1686
public class TerrainAlphaMap : TerrainMap<byte>
{
	// Token: 0x0400277C RID: 10108
	[FormerlySerializedAs("ColorTexture")]
	public Texture2D AlphaTexture;

	// Token: 0x06002FEE RID: 12270 RVA: 0x0011FD7C File Offset: 0x0011DF7C
	public override void Setup()
	{
		this.res = this.terrain.terrainData.alphamapResolution;
		this.src = (this.dst = new byte[this.res * this.res]);
		for (int i = 0; i < this.res; i++)
		{
			for (int j = 0; j < this.res; j++)
			{
				this.dst[i * this.res + j] = byte.MaxValue;
			}
		}
		if (this.AlphaTexture != null)
		{
			if (this.AlphaTexture.width == this.AlphaTexture.height && this.AlphaTexture.width == this.res)
			{
				Color32[] pixels = this.AlphaTexture.GetPixels32();
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
			Debug.LogError("Invalid alpha texture: " + this.AlphaTexture.name);
		}
	}

	// Token: 0x06002FEF RID: 12271 RVA: 0x0011FEB0 File Offset: 0x0011E0B0
	public void GenerateTextures()
	{
		this.AlphaTexture = new Texture2D(this.res, this.res, TextureFormat.Alpha8, false, true);
		this.AlphaTexture.name = "AlphaTexture";
		this.AlphaTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] col = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				byte b = this.src[z * this.res + i];
				col[z * this.res + i] = new Color32(b, b, b, b);
			}
		});
		this.AlphaTexture.SetPixels32(col);
	}

	// Token: 0x06002FF0 RID: 12272 RVA: 0x0011FF41 File Offset: 0x0011E141
	public void ApplyTextures()
	{
		this.AlphaTexture.Apply(true, false);
		this.AlphaTexture.Compress(false);
		this.AlphaTexture.Apply(false, true);
	}

	// Token: 0x06002FF1 RID: 12273 RVA: 0x0011FF6C File Offset: 0x0011E16C
	public float GetAlpha(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetAlpha(normX, normZ);
	}

	// Token: 0x06002FF2 RID: 12274 RVA: 0x0011FF9C File Offset: 0x0011E19C
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

	// Token: 0x06002FF3 RID: 12275 RVA: 0x0012002E File Offset: 0x0011E22E
	public float GetAlpha(int x, int z)
	{
		return BitUtility.Byte2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x00120048 File Offset: 0x0011E248
	public void SetAlpha(Vector3 worldPos, float a)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a);
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x00120078 File Offset: 0x0011E278
	public void SetAlpha(float normX, float normZ, float a)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetAlpha(x, z, a);
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x0012009E File Offset: 0x0011E29E
	public void SetAlpha(int x, int z, float a)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Byte(a);
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x001200B7 File Offset: 0x0011E2B7
	public void SetAlpha(int x, int z, float a, float opacity)
	{
		this.SetAlpha(x, z, Mathf.Lerp(this.GetAlpha(x, z), a, opacity));
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x001200D4 File Offset: 0x0011E2D4
	public void SetAlpha(Vector3 worldPos, float a, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetAlpha(normX, normZ, a, opacity, radius, fade);
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x00120108 File Offset: 0x0011E308
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
