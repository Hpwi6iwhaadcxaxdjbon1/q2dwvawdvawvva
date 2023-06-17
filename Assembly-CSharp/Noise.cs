using System;

// Token: 0x0200064F RID: 1615
public static class Noise
{
	// Token: 0x0400265D RID: 9821
	public const float MIN = -1000000f;

	// Token: 0x0400265E RID: 9822
	public const float MAX = 1000000f;

	// Token: 0x06002EC3 RID: 11971 RVA: 0x00119982 File Offset: 0x00117B82
	public static float Simplex1D(float x)
	{
		return NativeNoise.Simplex1D(x);
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x0011998A File Offset: 0x00117B8A
	public static float Simplex2D(float x, float y)
	{
		return NativeNoise.Simplex2D(x, y);
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x00119993 File Offset: 0x00117B93
	public static float Turbulence(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Turbulence(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x001199A4 File Offset: 0x00117BA4
	public static float Billow(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Billow(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x001199B5 File Offset: 0x00117BB5
	public static float Ridge(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Ridge(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x001199C6 File Offset: 0x00117BC6
	public static float Sharp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.Sharp(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x001199D7 File Offset: 0x00117BD7
	public static float TurbulenceIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.TurbulenceIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x001199E8 File Offset: 0x00117BE8
	public static float BillowIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.BillowIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x001199F9 File Offset: 0x00117BF9
	public static float RidgeIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.RidgeIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x00119A0A File Offset: 0x00117C0A
	public static float SharpIQ(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f)
	{
		return NativeNoise.SharpIQ(x, y, octaves, frequency, amplitude, lacunarity, gain);
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x00119A1B File Offset: 0x00117C1B
	public static float TurbulenceWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.TurbulenceWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x00119A2E File Offset: 0x00117C2E
	public static float BillowWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.BillowWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x00119A41 File Offset: 0x00117C41
	public static float RidgeWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.RidgeWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x00119A54 File Offset: 0x00117C54
	public static float SharpWarp(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 0.25f)
	{
		return NativeNoise.SharpWarp(x, y, octaves, frequency, amplitude, lacunarity, gain, warp);
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x00119A68 File Offset: 0x00117C68
	public static float Jordan(float x, float y, int octaves = 1, float frequency = 1f, float amplitude = 1f, float lacunarity = 2f, float gain = 0.5f, float warp = 1f, float damp = 1f, float damp_scale = 1f)
	{
		return NativeNoise.Jordan(x, y, octaves, frequency, amplitude, lacunarity, gain, warp, damp, damp_scale);
	}
}
