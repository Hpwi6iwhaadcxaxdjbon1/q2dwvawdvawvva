using System;
using UnityEngine;

// Token: 0x02000655 RID: 1621
public struct TextureData
{
	// Token: 0x0400269D RID: 9885
	public int width;

	// Token: 0x0400269E RID: 9886
	public int height;

	// Token: 0x0400269F RID: 9887
	public Color32[] colors;

	// Token: 0x06002F08 RID: 12040 RVA: 0x0011C544 File Offset: 0x0011A744
	public TextureData(Texture2D tex)
	{
		if (tex != null)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.colors = tex.GetPixels32();
			return;
		}
		this.width = 0;
		this.height = 0;
		this.colors = null;
	}

	// Token: 0x06002F09 RID: 12041 RVA: 0x0011C594 File Offset: 0x0011A794
	public Color32 GetColor(int x, int y)
	{
		return this.colors[y * this.width + x];
	}

	// Token: 0x06002F0A RID: 12042 RVA: 0x0011C5AB File Offset: 0x0011A7AB
	public int GetShort(int x, int y)
	{
		return (int)BitUtility.DecodeShort(this.GetColor(x, y));
	}

	// Token: 0x06002F0B RID: 12043 RVA: 0x0011C5BA File Offset: 0x0011A7BA
	public int GetInt(int x, int y)
	{
		return BitUtility.DecodeInt(this.GetColor(x, y));
	}

	// Token: 0x06002F0C RID: 12044 RVA: 0x0011C5C9 File Offset: 0x0011A7C9
	public float GetFloat(int x, int y)
	{
		return BitUtility.DecodeFloat(this.GetColor(x, y));
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x0011C5D8 File Offset: 0x0011A7D8
	public float GetHalf(int x, int y)
	{
		return BitUtility.Short2Float(this.GetShort(x, y));
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x0011C5E7 File Offset: 0x0011A7E7
	public Vector4 GetVector(int x, int y)
	{
		return BitUtility.DecodeVector(this.GetColor(x, y));
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x0011C5F6 File Offset: 0x0011A7F6
	public Vector3 GetNormal(int x, int y)
	{
		return BitUtility.DecodeNormal(this.GetColor(x, y));
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x0011C60C File Offset: 0x0011A80C
	public Color32 GetInterpolatedColor(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Color a = this.GetColor(num3, num4);
		Color b = this.GetColor(x2, num4);
		Color a2 = this.GetColor(num3, y2);
		Color b2 = this.GetColor(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Color a3 = Color.Lerp(a, b, t);
		Color b3 = Color.Lerp(a2, b2, t);
		return Color.Lerp(a3, b3, t2);
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x0011C6E8 File Offset: 0x0011A8E8
	public int GetInterpolatedInt(float x, float y)
	{
		float f = x * (float)(this.width - 1);
		float f2 = y * (float)(this.height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(f), 1, this.width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(f2), 1, this.height - 2);
		return this.GetInt(x2, y2);
	}

	// Token: 0x06002F12 RID: 12050 RVA: 0x0011C740 File Offset: 0x0011A940
	public int GetInterpolatedShort(float x, float y)
	{
		float f = x * (float)(this.width - 1);
		float f2 = y * (float)(this.height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(f), 1, this.width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(f2), 1, this.height - 2);
		return this.GetShort(x2, y2);
	}

	// Token: 0x06002F13 RID: 12051 RVA: 0x0011C798 File Offset: 0x0011A998
	public float GetInterpolatedFloat(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		float @float = this.GetFloat(num3, num4);
		float float2 = this.GetFloat(x2, num4);
		float float3 = this.GetFloat(num3, y2);
		float float4 = this.GetFloat(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		float a = Mathf.Lerp(@float, float2, t);
		float b = Mathf.Lerp(float3, float4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06002F14 RID: 12052 RVA: 0x0011C858 File Offset: 0x0011AA58
	public float GetInterpolatedHalf(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		float half = this.GetHalf(num3, num4);
		float half2 = this.GetHalf(x2, num4);
		float half3 = this.GetHalf(num3, y2);
		float half4 = this.GetHalf(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		float a = Mathf.Lerp(half, half2, t);
		float b = Mathf.Lerp(half3, half4, t);
		return Mathf.Lerp(a, b, t2);
	}

	// Token: 0x06002F15 RID: 12053 RVA: 0x0011C918 File Offset: 0x0011AB18
	public Vector4 GetInterpolatedVector(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Vector4 vector = this.GetVector(num3, num4);
		Vector4 vector2 = this.GetVector(x2, num4);
		Vector4 vector3 = this.GetVector(num3, y2);
		Vector4 vector4 = this.GetVector(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Vector4 a = Vector4.Lerp(vector, vector2, t);
		Vector4 b = Vector4.Lerp(vector3, vector4, t);
		return Vector4.Lerp(a, b, t2);
	}

	// Token: 0x06002F16 RID: 12054 RVA: 0x0011C9D8 File Offset: 0x0011ABD8
	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		float num = x * (float)(this.width - 1);
		float num2 = y * (float)(this.height - 1);
		int num3 = Mathf.Clamp((int)num, 1, this.width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, this.height - 2);
		int x2 = Mathf.Min(num3 + 1, this.width - 2);
		int y2 = Mathf.Min(num4 + 1, this.height - 2);
		Vector3 normal = this.GetNormal(num3, num4);
		Vector3 normal2 = this.GetNormal(x2, num4);
		Vector3 normal3 = this.GetNormal(num3, y2);
		Vector3 normal4 = this.GetNormal(x2, y2);
		float t = num - (float)num3;
		float t2 = num2 - (float)num4;
		Vector3 a = Vector3.Lerp(normal, normal2, t);
		Vector3 b = Vector3.Lerp(normal3, normal4, t);
		return Vector3.Lerp(a, b, t2);
	}
}
