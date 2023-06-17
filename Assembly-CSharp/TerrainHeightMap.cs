using System;
using UnityEngine;

// Token: 0x0200069A RID: 1690
public class TerrainHeightMap : TerrainMap<short>
{
	// Token: 0x04002781 RID: 10113
	public Texture2D HeightTexture;

	// Token: 0x04002782 RID: 10114
	public Texture2D NormalTexture;

	// Token: 0x04002783 RID: 10115
	private float normY;

	// Token: 0x06003028 RID: 12328 RVA: 0x0012110C File Offset: 0x0011F30C
	public override void Setup()
	{
		this.res = this.terrain.terrainData.heightmapResolution;
		this.src = (this.dst = new short[this.res * this.res]);
		this.normY = TerrainMeta.Size.x / TerrainMeta.Size.y / (float)this.res;
		if (this.HeightTexture != null)
		{
			if (this.HeightTexture.width == this.HeightTexture.height && this.HeightTexture.width == this.res)
			{
				Color32[] pixels = this.HeightTexture.GetPixels32();
				int i = 0;
				int num = 0;
				while (i < this.res)
				{
					int j = 0;
					while (j < this.res)
					{
						Color32 c = pixels[num];
						this.dst[i * this.res + j] = BitUtility.DecodeShort(c);
						j++;
						num++;
					}
					i++;
				}
				return;
			}
			Debug.LogError("Invalid height texture: " + this.HeightTexture.name);
		}
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x00121224 File Offset: 0x0011F424
	public void ApplyToTerrain()
	{
		float[,] heights = this.terrain.terrainData.GetHeights(0, 0, this.res, this.res);
		Parallel.For(0, this.res, delegate(int z)
		{
			for (int i = 0; i < this.res; i++)
			{
				heights[z, i] = this.GetHeight01(i, z);
			}
		});
		this.terrain.terrainData.SetHeights(0, 0, heights);
		TerrainCollider component = this.terrain.GetComponent<TerrainCollider>();
		if (component)
		{
			component.enabled = false;
			component.enabled = true;
		}
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x001212B4 File Offset: 0x0011F4B4
	public void GenerateTextures(bool heightTexture = true, bool normalTexture = true)
	{
		if (heightTexture)
		{
			Color32[] heights = new Color32[this.res * this.res];
			Parallel.For(0, this.res, delegate(int z)
			{
				for (int i = 0; i < this.res; i++)
				{
					heights[z * this.res + i] = BitUtility.EncodeShort(this.src[z * this.res + i]);
				}
			});
			this.HeightTexture = new Texture2D(this.res, this.res, TextureFormat.RGBA32, true, true);
			this.HeightTexture.name = "HeightTexture";
			this.HeightTexture.wrapMode = TextureWrapMode.Clamp;
			this.HeightTexture.SetPixels32(heights);
		}
		if (normalTexture)
		{
			int normalres = (this.res - 1) / 2;
			Color32[] normals = new Color32[normalres * normalres];
			Parallel.For(0, normalres, delegate(int z)
			{
				float normZ = ((float)z + 0.5f) / (float)normalres;
				for (int i = 0; i < normalres; i++)
				{
					float normX = ((float)i + 0.5f) / (float)normalres;
					Vector3 vector = this.GetNormal(normX, normZ);
					float value = Vector3.Angle(Vector3.up, vector);
					float t = Mathf.InverseLerp(50f, 70f, value);
					vector = Vector3.Slerp(vector, Vector3.up, t);
					normals[z * normalres + i] = BitUtility.EncodeNormal(vector);
				}
			});
			this.NormalTexture = new Texture2D(normalres, normalres, TextureFormat.RGBA32, false, true);
			this.NormalTexture.name = "NormalTexture";
			this.NormalTexture.wrapMode = TextureWrapMode.Clamp;
			this.NormalTexture.SetPixels32(normals);
		}
	}

	// Token: 0x0600302B RID: 12331 RVA: 0x001213E8 File Offset: 0x0011F5E8
	public void ApplyTextures()
	{
		this.HeightTexture.Apply(true, false);
		this.NormalTexture.Apply(true, false);
		this.NormalTexture.Compress(false);
		this.HeightTexture.Apply(false, true);
		this.NormalTexture.Apply(false, true);
	}

	// Token: 0x0600302C RID: 12332 RVA: 0x00121435 File Offset: 0x0011F635
	public float GetHeight(Vector3 worldPos)
	{
		return TerrainMeta.Position.y + this.GetHeight01(worldPos) * TerrainMeta.Size.y;
	}

	// Token: 0x0600302D RID: 12333 RVA: 0x00121454 File Offset: 0x0011F654
	public float GetHeight(float normX, float normZ)
	{
		return TerrainMeta.Position.y + this.GetHeight01(normX, normZ) * TerrainMeta.Size.y;
	}

	// Token: 0x0600302E RID: 12334 RVA: 0x00121474 File Offset: 0x0011F674
	public float GetHeightFast(Vector2 uv)
	{
		int num = this.res - 1;
		float num2 = uv.x * (float)num;
		float num3 = uv.y * (float)num;
		int num4 = (int)num2;
		int num5 = (int)num3;
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		num4 = ((num4 >= 0) ? num4 : 0);
		num5 = ((num5 >= 0) ? num5 : 0);
		num4 = ((num4 <= num) ? num4 : num);
		num5 = ((num5 <= num) ? num5 : num);
		int num8 = (num2 < (float)num) ? 1 : 0;
		int num9 = (num3 < (float)num) ? this.res : 0;
		int num10 = num5 * this.res + num4;
		int num11 = num10 + num8;
		int num12 = num10 + num9;
		int num13 = num12 + num8;
		float num14 = (float)this.src[num10] * 3.051944E-05f;
		float num15 = (float)this.src[num11] * 3.051944E-05f;
		float num16 = (float)this.src[num12] * 3.051944E-05f;
		float num17 = (float)this.src[num13] * 3.051944E-05f;
		float num18 = (num15 - num14) * num6 + num14;
		float num19 = ((num17 - num16) * num6 + num16 - num18) * num7 + num18;
		return TerrainMeta.Position.y + num19 * TerrainMeta.Size.y;
	}

	// Token: 0x0600302F RID: 12335 RVA: 0x0012158D File Offset: 0x0011F78D
	public float GetHeight(int x, int z)
	{
		return TerrainMeta.Position.y + this.GetHeight01(x, z) * TerrainMeta.Size.y;
	}

	// Token: 0x06003030 RID: 12336 RVA: 0x001215B0 File Offset: 0x0011F7B0
	public float GetHeight01(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetHeight01(normX, normZ);
	}

	// Token: 0x06003031 RID: 12337 RVA: 0x001215E0 File Offset: 0x0011F7E0
	public float GetHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(x, num5);
		float height3 = this.GetHeight01(num4, z);
		float height4 = this.GetHeight01(x, z);
		float t = num2 - (float)num4;
		float t2 = num3 - (float)num5;
		float a = Mathf.Lerp(height, height2, t);
		float b = Mathf.Lerp(height3, height4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06003032 RID: 12338 RVA: 0x00121684 File Offset: 0x0011F884
	public float GetTriangulatedHeight01(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		float num6 = num2 - (float)num4;
		float num7 = num3 - (float)num5;
		float height = this.GetHeight01(num4, num5);
		float height2 = this.GetHeight01(x, z);
		if (num6 > num7)
		{
			float height3 = this.GetHeight01(x, num5);
			return height + (height3 - height) * num6 + (height2 - height3) * num7;
		}
		float height4 = this.GetHeight01(num4, z);
		return height + (height2 - height4) * num6 + (height4 - height) * num7;
	}

	// Token: 0x06003033 RID: 12339 RVA: 0x00121733 File Offset: 0x0011F933
	public float GetHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003034 RID: 12340 RVA: 0x00121733 File Offset: 0x0011F933
	private float GetSrcHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.src[z * this.res + x]);
	}

	// Token: 0x06003035 RID: 12341 RVA: 0x0012174B File Offset: 0x0011F94B
	private float GetDstHeight01(int x, int z)
	{
		return BitUtility.Short2Float((int)this.dst[z * this.res + x]);
	}

	// Token: 0x06003036 RID: 12342 RVA: 0x00121764 File Offset: 0x0011F964
	public Vector3 GetNormal(Vector3 worldPos)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetNormal(normX, normZ);
	}

	// Token: 0x06003037 RID: 12343 RVA: 0x00121794 File Offset: 0x0011F994
	public Vector3 GetNormal(float normX, float normZ)
	{
		int num = this.res - 1;
		float num2 = normX * (float)num;
		float num3 = normZ * (float)num;
		int num4 = Mathf.Clamp((int)num2, 0, num);
		int num5 = Mathf.Clamp((int)num3, 0, num);
		int x = Mathf.Min(num4 + 1, num);
		int z = Mathf.Min(num5 + 1, num);
		Vector3 normal = this.GetNormal(num4, num5);
		Vector3 normal2 = this.GetNormal(x, num5);
		Vector3 normal3 = this.GetNormal(num4, z);
		Vector3 normal4 = this.GetNormal(x, z);
		float t = num2 - (float)num4;
		float t2 = num3 - (float)num5;
		Vector3 a = Vector3.Slerp(normal, normal2, t);
		Vector3 b = Vector3.Slerp(normal3, normal4, t);
		return Vector3.Slerp(a, b, t2).normalized;
	}

	// Token: 0x06003038 RID: 12344 RVA: 0x00121844 File Offset: 0x0011FA44
	public Vector3 GetNormal(int x, int z)
	{
		int max = this.res - 1;
		int x2 = Mathf.Clamp(x - 1, 0, max);
		int z2 = Mathf.Clamp(z - 1, 0, max);
		int x3 = Mathf.Clamp(x + 1, 0, max);
		int z3 = Mathf.Clamp(z + 1, 0, max);
		float num = (this.GetHeight01(x3, z2) - this.GetHeight01(x2, z2)) * 0.5f;
		float num2 = (this.GetHeight01(x2, z3) - this.GetHeight01(x2, z2)) * 0.5f;
		return new Vector3(-num, this.normY, -num2).normalized;
	}

	// Token: 0x06003039 RID: 12345 RVA: 0x001218D0 File Offset: 0x0011FAD0
	private Vector3 GetNormalSobel(int x, int z)
	{
		int num = this.res - 1;
		Vector3 vector = new Vector3(TerrainMeta.Size.x / (float)num, TerrainMeta.Size.y, TerrainMeta.Size.z / (float)num);
		int x2 = Mathf.Clamp(x - 1, 0, num);
		int z2 = Mathf.Clamp(z - 1, 0, num);
		int x3 = Mathf.Clamp(x + 1, 0, num);
		int z3 = Mathf.Clamp(z + 1, 0, num);
		float num2 = this.GetHeight01(x2, z2) * -1f;
		num2 += this.GetHeight01(x2, z) * -2f;
		num2 += this.GetHeight01(x2, z3) * -1f;
		num2 += this.GetHeight01(x3, z2) * 1f;
		num2 += this.GetHeight01(x3, z) * 2f;
		num2 += this.GetHeight01(x3, z3) * 1f;
		num2 *= vector.y;
		num2 /= vector.x;
		float num3 = this.GetHeight01(x2, z2) * -1f;
		num3 += this.GetHeight01(x, z2) * -2f;
		num3 += this.GetHeight01(x3, z2) * -1f;
		num3 += this.GetHeight01(x2, z3) * 1f;
		num3 += this.GetHeight01(x, z3) * 2f;
		num3 += this.GetHeight01(x3, z3) * 1f;
		num3 *= vector.y;
		num3 /= vector.z;
		Vector3 vector2 = new Vector3(-num2, 8f, -num3);
		return vector2.normalized;
	}

	// Token: 0x0600303A RID: 12346 RVA: 0x00121A7C File Offset: 0x0011FC7C
	public float GetSlope(Vector3 worldPos)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(worldPos));
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x00121A8F File Offset: 0x0011FC8F
	public float GetSlope(float normX, float normZ)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(normX, normZ));
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x00121AA3 File Offset: 0x0011FCA3
	public float GetSlope(int x, int z)
	{
		return Vector3.Angle(Vector3.up, this.GetNormal(x, z));
	}

	// Token: 0x0600303D RID: 12349 RVA: 0x00121AB7 File Offset: 0x0011FCB7
	public float GetSlope01(Vector3 worldPos)
	{
		return this.GetSlope(worldPos) * 0.011111111f;
	}

	// Token: 0x0600303E RID: 12350 RVA: 0x00121AC6 File Offset: 0x0011FCC6
	public float GetSlope01(float normX, float normZ)
	{
		return this.GetSlope(normX, normZ) * 0.011111111f;
	}

	// Token: 0x0600303F RID: 12351 RVA: 0x00121AD6 File Offset: 0x0011FCD6
	public float GetSlope01(int x, int z)
	{
		return this.GetSlope(x, z) * 0.011111111f;
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x00121AE8 File Offset: 0x0011FCE8
	public void SetHeight(Vector3 worldPos, float height)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height);
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x00121B18 File Offset: 0x0011FD18
	public void SetHeight(float normX, float normZ, float height)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x00121B3E File Offset: 0x0011FD3E
	public void SetHeight(int x, int z, float height)
	{
		this.dst[z * this.res + x] = BitUtility.Float2Short(height);
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x00121B58 File Offset: 0x0011FD58
	public void SetHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.SetHeight(normX, normZ, height, opacity);
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x00121B88 File Offset: 0x0011FD88
	public void SetHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.SetHeight(x, z, height, opacity);
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x00121BB0 File Offset: 0x0011FDB0
	public void SetHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity);
		this.SetHeight(x, z, height2);
	}

	// Token: 0x06003046 RID: 12358 RVA: 0x00121BD8 File Offset: 0x0011FDD8
	public void AddHeight(Vector3 worldPos, float delta)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(normX, normZ, delta);
	}

	// Token: 0x06003047 RID: 12359 RVA: 0x00121C08 File Offset: 0x0011FE08
	public void AddHeight(float normX, float normZ, float delta)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.AddHeight(x, z, delta);
	}

	// Token: 0x06003048 RID: 12360 RVA: 0x00121C30 File Offset: 0x0011FE30
	public void AddHeight(int x, int z, float delta)
	{
		float height = Mathf.Clamp01(this.GetDstHeight01(x, z) + delta);
		this.SetHeight(x, z, height);
	}

	// Token: 0x06003049 RID: 12361 RVA: 0x00121C58 File Offset: 0x0011FE58
	public void LowerHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.LowerHeight(normX, normZ, height, opacity);
	}

	// Token: 0x0600304A RID: 12362 RVA: 0x00121C88 File Offset: 0x0011FE88
	public void LowerHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.LowerHeight(x, z, height, opacity);
	}

	// Token: 0x0600304B RID: 12363 RVA: 0x00121CB0 File Offset: 0x0011FEB0
	public void LowerHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Min(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, height2);
	}

	// Token: 0x0600304C RID: 12364 RVA: 0x00121CE4 File Offset: 0x0011FEE4
	public void RaiseHeight(Vector3 worldPos, float height, float opacity)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.RaiseHeight(normX, normZ, height, opacity);
	}

	// Token: 0x0600304D RID: 12365 RVA: 0x00121D14 File Offset: 0x0011FF14
	public void RaiseHeight(float normX, float normZ, float height, float opacity)
	{
		int x = base.Index(normX);
		int z = base.Index(normZ);
		this.RaiseHeight(x, z, height, opacity);
	}

	// Token: 0x0600304E RID: 12366 RVA: 0x00121D3C File Offset: 0x0011FF3C
	public void RaiseHeight(int x, int z, float height, float opacity)
	{
		float height2 = Mathf.Max(this.GetDstHeight01(x, z), Mathf.SmoothStep(this.GetSrcHeight01(x, z), height, opacity));
		this.SetHeight(x, z, height2);
	}

	// Token: 0x0600304F RID: 12367 RVA: 0x00121D70 File Offset: 0x0011FF70
	public void SetHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.SetHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x00121DB0 File Offset: 0x0011FFB0
	public void SetHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.SetHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x00121DF4 File Offset: 0x0011FFF4
	public void LowerHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.LowerHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06003052 RID: 12370 RVA: 0x00121E34 File Offset: 0x00120034
	public void LowerHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.LowerHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06003053 RID: 12371 RVA: 0x00121E78 File Offset: 0x00120078
	public void RaiseHeight(Vector3 worldPos, float opacity, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		float height = TerrainMeta.NormalizeY(worldPos.y);
		this.RaiseHeight(normX, normZ, height, opacity, radius, fade);
	}

	// Token: 0x06003054 RID: 12372 RVA: 0x00121EB8 File Offset: 0x001200B8
	public void RaiseHeight(float normX, float normZ, float height, float opacity, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.RaiseHeight(x, z, height, lerp * opacity);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x00121EFC File Offset: 0x001200FC
	public void AddHeight(Vector3 worldPos, float delta, float radius, float fade = 0f)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		this.AddHeight(normX, normZ, delta, radius, fade);
	}

	// Token: 0x06003056 RID: 12374 RVA: 0x00121F30 File Offset: 0x00120130
	public void AddHeight(float normX, float normZ, float delta, float radius, float fade = 0f)
	{
		Action<int, int, float> action = delegate(int x, int z, float lerp)
		{
			if (lerp > 0f)
			{
				this.AddHeight(x, z, lerp * delta);
			}
		};
		base.ApplyFilter(normX, normZ, radius, fade, action);
	}
}
