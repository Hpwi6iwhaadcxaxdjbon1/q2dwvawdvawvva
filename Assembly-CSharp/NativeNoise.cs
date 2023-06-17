using System;
using System.Runtime.InteropServices;
using System.Security;

// Token: 0x0200064E RID: 1614
[SuppressUnmanagedCodeSecurity]
public static class NativeNoise
{
	// Token: 0x06002EB2 RID: 11954
	[DllImport("RustNative", EntryPoint = "snoise1_32")]
	public static extern float Simplex1D(float x);

	// Token: 0x06002EB3 RID: 11955
	[DllImport("RustNative", EntryPoint = "sdnoise1_32")]
	public static extern float Simplex1D(float x, out float dx);

	// Token: 0x06002EB4 RID: 11956
	[DllImport("RustNative", EntryPoint = "snoise2_32")]
	public static extern float Simplex2D(float x, float y);

	// Token: 0x06002EB5 RID: 11957
	[DllImport("RustNative", EntryPoint = "sdnoise2_32")]
	public static extern float Simplex2D(float x, float y, out float dx, out float dy);

	// Token: 0x06002EB6 RID: 11958
	[DllImport("RustNative", EntryPoint = "turbulence_32")]
	public static extern float Turbulence(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EB7 RID: 11959
	[DllImport("RustNative", EntryPoint = "billow_32")]
	public static extern float Billow(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EB8 RID: 11960
	[DllImport("RustNative", EntryPoint = "ridge_32")]
	public static extern float Ridge(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EB9 RID: 11961
	[DllImport("RustNative", EntryPoint = "sharp_32")]
	public static extern float Sharp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EBA RID: 11962
	[DllImport("RustNative", EntryPoint = "turbulence_iq_32")]
	public static extern float TurbulenceIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EBB RID: 11963
	[DllImport("RustNative", EntryPoint = "billow_iq_32")]
	public static extern float BillowIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EBC RID: 11964
	[DllImport("RustNative", EntryPoint = "ridge_iq_32")]
	public static extern float RidgeIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EBD RID: 11965
	[DllImport("RustNative", EntryPoint = "sharp_iq_32")]
	public static extern float SharpIQ(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain);

	// Token: 0x06002EBE RID: 11966
	[DllImport("RustNative", EntryPoint = "turbulence_warp_32")]
	public static extern float TurbulenceWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002EBF RID: 11967
	[DllImport("RustNative", EntryPoint = "billow_warp_32")]
	public static extern float BillowWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002EC0 RID: 11968
	[DllImport("RustNative", EntryPoint = "ridge_warp_32")]
	public static extern float RidgeWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002EC1 RID: 11969
	[DllImport("RustNative", EntryPoint = "sharp_warp_32")]
	public static extern float SharpWarp(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp);

	// Token: 0x06002EC2 RID: 11970
	[DllImport("RustNative", EntryPoint = "jordan_32")]
	public static extern float Jordan(float x, float y, int octaves, float frequency, float amplitude, float lacunarity, float gain, float warp, float damp, float damp_scale);
}
