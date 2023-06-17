using System;
using UnityEngine;

// Token: 0x0200092E RID: 2350
public class sRGB
{
	// Token: 0x0400333C RID: 13116
	public static byte[] to_linear = new byte[256];

	// Token: 0x0400333D RID: 13117
	public static byte[] to_srgb = new byte[256];

	// Token: 0x06003878 RID: 14456 RVA: 0x001510B0 File Offset: 0x0014F2B0
	static sRGB()
	{
		sRGB.to_linear = new byte[256];
		sRGB.to_srgb = new byte[256];
		for (int i = 0; i < 256; i++)
		{
			sRGB.to_linear[i] = (byte)(sRGB.srgb_to_linear((float)i * 0.003921569f) * 255f + 0.5f);
		}
		for (int j = 0; j < 256; j++)
		{
			sRGB.to_srgb[j] = (byte)(sRGB.linear_to_srgb((float)j * 0.003921569f) * 255f + 0.5f);
		}
	}

	// Token: 0x06003879 RID: 14457 RVA: 0x0015115C File Offset: 0x0014F35C
	public static float linear_to_srgb(float linear)
	{
		if (float.IsNaN(linear))
		{
			return 0f;
		}
		if (linear > 1f)
		{
			return 1f;
		}
		if (linear < 0f)
		{
			return 0f;
		}
		if (linear < 0.0031308f)
		{
			return 12.92f * linear;
		}
		return 1.055f * Mathf.Pow(linear, 0.41666f) - 0.055f;
	}

	// Token: 0x0600387A RID: 14458 RVA: 0x001511BA File Offset: 0x0014F3BA
	public static float srgb_to_linear(float srgb)
	{
		if (srgb <= 0.04045f)
		{
			return srgb / 12.92f;
		}
		return Mathf.Pow((srgb + 0.055f) / 1.055f, 2.4f);
	}
}
