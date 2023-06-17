using System;
using UnityEngine;

// Token: 0x02000699 RID: 1689
public class TerrainDistanceMap : TerrainMap<byte>
{
	// Token: 0x04002780 RID: 10112
	public Texture2D DistanceTexture;

	// Token: 0x06003020 RID: 12320 RVA: 0x00120D98 File Offset: 0x0011EF98
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new byte[4 * this.res * this.res]);
		if (this.DistanceTexture != null)
		{
			if (this.DistanceTexture.width == this.DistanceTexture.height && this.DistanceTexture.width == this.res)
			{
				Color32[] pixels = this.DistanceTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						this.SetDistance(j, i, BitUtility.DecodeVector2i(pixels[num]));
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid distance texture: " + this.DistanceTexture.name, this.DistanceTexture);
		}
	}

	// Token: 0x06003021 RID: 12321 RVA: 0x00120E88 File Offset: 0x0011F088
	public void GenerateTextures()
	{
		this.DistanceTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
		this.DistanceTexture.name = "DistanceTexture";
		this.DistanceTexture.wrapMode = TextureWrapMode.Clamp;
		Color32[] cols = new Color32[this.res * this.res];
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				cols[z * this.res + i] = BitUtility.EncodeVector2i(this.GetDistance(i, z));
			}
		});
		this.DistanceTexture.SetPixels32(cols);
	}

	// Token: 0x06003022 RID: 12322 RVA: 0x00120F19 File Offset: 0x0011F119
	public void ApplyTextures()
	{
		this.DistanceTexture.Apply(true, true);
	}

	// Token: 0x06003023 RID: 12323 RVA: 0x00120F28 File Offset: 0x0011F128
	public Vector2i GetDistance(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetDistance(normX, normZ);
	}

	// Token: 0x06003024 RID: 12324 RVA: 0x00120F58 File Offset: 0x0011F158
	public Vector2i GetDistance(float normX, float normZ)
	{
		int num = this.res - 1;
		int x = Mathf.Clamp(Mathf.RoundToInt(normX * (float)num), 0, num);
		int z = Mathf.Clamp(Mathf.RoundToInt(normZ * (float)num), 0, num);
		return this.GetDistance(x, z);
	}

	// Token: 0x06003025 RID: 12325 RVA: 0x00120F98 File Offset: 0x0011F198
	public Vector2i GetDistance(int x, int z)
	{
		byte[] src = this.src;
		int res = this.res;
		byte b = src[(0 + z) * this.res + x];
		byte b2 = this.src[(this.res + z) * this.res + x];
		byte b3 = this.src[(2 * this.res + z) * this.res + x];
		byte b4 = this.src[(3 * this.res + z) * this.res + x];
		if (b == 255 && b2 == 255 && b3 == 255 && b4 == 255)
		{
			return new Vector2i(256, 256);
		}
		return new Vector2i((int)(b - b2), (int)(b3 - b4));
	}

	// Token: 0x06003026 RID: 12326 RVA: 0x0012104C File Offset: 0x0011F24C
	public void SetDistance(int x, int z, Vector2i v)
	{
		byte[] dst = this.dst;
		int res = this.res;
		dst[(0 + z) * this.res + x] = (byte)Mathf.Clamp(v.x, 0, 255);
		this.dst[(this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.x, 0, 255);
		this.dst[(2 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(v.y, 0, 255);
		this.dst[(3 * this.res + z) * this.res + x] = (byte)Mathf.Clamp(-v.y, 0, 255);
	}
}
