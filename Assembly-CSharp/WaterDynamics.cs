using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020006FD RID: 1789
[ExecuteInEditMode]
public class WaterDynamics : MonoBehaviour
{
	// Token: 0x040028F6 RID: 10486
	private const int maxRasterSize = 1024;

	// Token: 0x040028F7 RID: 10487
	private const int subStep = 256;

	// Token: 0x040028F8 RID: 10488
	private const int subShift = 8;

	// Token: 0x040028F9 RID: 10489
	private const int subMask = 255;

	// Token: 0x040028FA RID: 10490
	private const float oneOverSubStep = 0.00390625f;

	// Token: 0x040028FB RID: 10491
	private const float interp_subStep = 65536f;

	// Token: 0x040028FC RID: 10492
	private const int interp_subShift = 16;

	// Token: 0x040028FD RID: 10493
	private const int interp_subFracMask = 65535;

	// Token: 0x040028FE RID: 10494
	private WaterDynamics.ImageDesc imageDesc;

	// Token: 0x040028FF RID: 10495
	private byte[] imagePixels;

	// Token: 0x04002900 RID: 10496
	private WaterDynamics.TargetDesc targetDesc;

	// Token: 0x04002901 RID: 10497
	private byte[] targetPixels;

	// Token: 0x04002902 RID: 10498
	private byte[] targetDrawTileTable;

	// Token: 0x04002903 RID: 10499
	private SimpleList<ushort> targetDrawTileList;

	// Token: 0x04002904 RID: 10500
	public bool ShowDebug;

	// Token: 0x04002906 RID: 10502
	public bool ForceFallback;

	// Token: 0x04002907 RID: 10503
	private WaterDynamics.Target target;

	// Token: 0x04002908 RID: 10504
	private bool useNativePath;

	// Token: 0x04002909 RID: 10505
	private static HashSet<WaterInteraction> interactions = new HashSet<WaterInteraction>();

	// Token: 0x0600325B RID: 12891 RVA: 0x0013618A File Offset: 0x0013438A
	private void RasterBindImage(WaterDynamics.Image image)
	{
		this.imageDesc = image.desc;
		this.imagePixels = image.pixels;
	}

	// Token: 0x0600325C RID: 12892 RVA: 0x001361A4 File Offset: 0x001343A4
	private void RasterBindTarget(WaterDynamics.Target target)
	{
		this.targetDesc = target.Desc;
		this.targetPixels = target.Pixels;
		this.targetDrawTileTable = target.DrawTileTable;
		this.targetDrawTileList = target.DrawTileList;
	}

	// Token: 0x0600325D RID: 12893 RVA: 0x001361D8 File Offset: 0x001343D8
	private void RasterInteraction(Vector2 pos, Vector2 scale, float rotation, float disp, float dist)
	{
		Vector2 a = this.targetDesc.WorldToRaster(pos);
		float f = -rotation * 0.017453292f;
		float s = Mathf.Sin(f);
		float c = Mathf.Cos(f);
		float num = Mathf.Min((float)this.imageDesc.width * scale.x, 1024f) * 0.5f;
		float num2 = Mathf.Min((float)this.imageDesc.height * scale.y, 1024f) * 0.5f;
		Vector2 vector = a + this.Rotate2D(new Vector2(-num, -num2), s, c);
		Vector2 vector2 = a + this.Rotate2D(new Vector2(num, -num2), s, c);
		Vector2 vector3 = a + this.Rotate2D(new Vector2(num, num2), s, c);
		Vector2 vector4 = a + this.Rotate2D(new Vector2(-num, num2), s, c);
		WaterDynamics.Point2D p = new WaterDynamics.Point2D(vector.x * 256f, vector.y * 256f);
		WaterDynamics.Point2D p2 = new WaterDynamics.Point2D(vector2.x * 256f, vector2.y * 256f);
		WaterDynamics.Point2D point2D = new WaterDynamics.Point2D(vector3.x * 256f, vector3.y * 256f);
		WaterDynamics.Point2D p3 = new WaterDynamics.Point2D(vector4.x * 256f, vector4.y * 256f);
		Vector2 uv = new Vector2(-0.5f, -0.5f);
		Vector2 uv2 = new Vector2((float)this.imageDesc.width - 0.5f, -0.5f);
		Vector2 vector5 = new Vector2((float)this.imageDesc.width - 0.5f, (float)this.imageDesc.height - 0.5f);
		Vector2 uv3 = new Vector2(-0.5f, (float)this.imageDesc.height - 0.5f);
		byte disp2 = (byte)(disp * 255f);
		byte dist2 = (byte)(dist * 255f);
		this.RasterizeTriangle(p, p2, point2D, uv, uv2, vector5, disp2, dist2);
		this.RasterizeTriangle(p, point2D, p3, uv, vector5, uv3, disp2, dist2);
	}

	// Token: 0x0600325E RID: 12894 RVA: 0x001363EB File Offset: 0x001345EB
	private float Frac(float x)
	{
		return x - (float)((int)x);
	}

	// Token: 0x0600325F RID: 12895 RVA: 0x001363F4 File Offset: 0x001345F4
	private Vector2 Rotate2D(Vector2 v, float s, float c)
	{
		Vector2 result;
		result.x = v.x * c - v.y * s;
		result.y = v.y * c + v.x * s;
		return result;
	}

	// Token: 0x06003260 RID: 12896 RVA: 0x00136432 File Offset: 0x00134632
	private int Min3(int a, int b, int c)
	{
		return Mathf.Min(a, Mathf.Min(b, c));
	}

	// Token: 0x06003261 RID: 12897 RVA: 0x00136441 File Offset: 0x00134641
	private int Max3(int a, int b, int c)
	{
		return Mathf.Max(a, Mathf.Max(b, c));
	}

	// Token: 0x06003262 RID: 12898 RVA: 0x00136450 File Offset: 0x00134650
	private int EdgeFunction(WaterDynamics.Point2D a, WaterDynamics.Point2D b, WaterDynamics.Point2D c)
	{
		return (int)(((long)(b.x - a.x) * (long)(c.y - a.y) >> 8) - ((long)(b.y - a.y) * (long)(c.x - a.x) >> 8));
	}

	// Token: 0x06003263 RID: 12899 RVA: 0x0013649D File Offset: 0x0013469D
	private bool IsTopLeft(WaterDynamics.Point2D a, WaterDynamics.Point2D b)
	{
		return (a.y == b.y && a.x < b.x) || a.y > b.y;
	}

	// Token: 0x06003264 RID: 12900 RVA: 0x001364CC File Offset: 0x001346CC
	private void RasterizeTriangle(WaterDynamics.Point2D p0, WaterDynamics.Point2D p1, WaterDynamics.Point2D p2, Vector2 uv0, Vector2 uv1, Vector2 uv2, byte disp, byte dist)
	{
		int width = this.imageDesc.width;
		int widthShift = this.imageDesc.widthShift;
		int maxWidth = this.imageDesc.maxWidth;
		int maxHeight = this.imageDesc.maxHeight;
		int size = this.targetDesc.size;
		int tileCount = this.targetDesc.tileCount;
		int num = Mathf.Max(this.Min3(p0.x, p1.x, p2.x), 0);
		int num2 = Mathf.Max(this.Min3(p0.y, p1.y, p2.y), 0);
		int num3 = Mathf.Min(this.Max3(p0.x, p1.x, p2.x), this.targetDesc.maxSizeSubStep);
		int num4 = Mathf.Min(this.Max3(p0.y, p1.y, p2.y), this.targetDesc.maxSizeSubStep);
		int num5 = Mathf.Max(num >> 8 >> this.targetDesc.tileSizeShift, 0);
		int num6 = Mathf.Min(num3 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
		int num7 = Mathf.Max(num2 >> 8 >> this.targetDesc.tileSizeShift, 0);
		int num8 = Mathf.Min(num4 >> 8 >> this.targetDesc.tileSizeShift, this.targetDesc.tileMaxCount);
		for (int i = num7; i <= num8; i++)
		{
			int num9 = i * tileCount;
			for (int j = num5; j <= num6; j++)
			{
				int num10 = num9 + j;
				if (this.targetDrawTileTable[num10] == 0)
				{
					this.targetDrawTileTable[num10] = 1;
					this.targetDrawTileList.Add((ushort)num10);
				}
			}
		}
		num = (num + 255 & -256);
		num2 = (num2 + 255 & -256);
		int num11 = this.IsTopLeft(p1, p2) ? 0 : -1;
		int num12 = this.IsTopLeft(p2, p0) ? 0 : -1;
		int num13 = this.IsTopLeft(p0, p1) ? 0 : -1;
		WaterDynamics.Point2D c = new WaterDynamics.Point2D(num, num2);
		int num14 = this.EdgeFunction(p1, p2, c) + num11;
		int num15 = this.EdgeFunction(p2, p0, c) + num12;
		int num16 = this.EdgeFunction(p0, p1, c) + num13;
		int num17 = p1.y - p2.y;
		int num18 = p2.y - p0.y;
		int num19 = p0.y - p1.y;
		int num20 = p2.x - p1.x;
		int num21 = p0.x - p2.x;
		int num22 = p1.x - p0.x;
		float num23 = 16777216f / (float)this.EdgeFunction(p0, p1, p2);
		float num24 = uv0.x * 65536f;
		float num25 = uv0.y * 65536f;
		float num26 = (uv1.x - uv0.x) * num23;
		float num27 = (uv1.y - uv0.y) * num23;
		float num28 = (uv2.x - uv0.x) * num23;
		float num29 = (uv2.y - uv0.y) * num23;
		int num30 = (int)((float)num18 * 0.00390625f * num26 + (float)num19 * 0.00390625f * num28);
		int num31 = (int)((float)num18 * 0.00390625f * num27 + (float)num19 * 0.00390625f * num29);
		for (int k = num2; k <= num4; k += 256)
		{
			int num32 = num14;
			int num33 = num15;
			int num34 = num16;
			int num35 = (int)(num24 + num26 * 0.00390625f * (float)num33 + num28 * 0.00390625f * (float)num34);
			int num36 = (int)(num25 + num27 * 0.00390625f * (float)num33 + num29 * 0.00390625f * (float)num34);
			for (int l = num; l <= num3; l += 256)
			{
				if ((num32 | num33 | num34) >= 0)
				{
					int num37 = (num35 > 0) ? num35 : 0;
					object obj = (num36 > 0) ? num36 : 0;
					int num38 = num37 >> 16;
					object obj2 = obj;
					int num39 = obj2 >> 16;
					byte b = (byte)((num37 & 65535) >> 8);
					byte b2 = (obj2 & 65535) >> 8;
					num38 = ((num38 > 0) ? num38 : 0);
					num39 = ((num39 > 0) ? num39 : 0);
					num38 = ((num38 < maxWidth) ? num38 : maxWidth);
					num39 = ((num39 < maxHeight) ? num39 : maxHeight);
					int num40 = (num38 < maxWidth) ? 1 : 0;
					int num41 = (num39 < maxHeight) ? width : 0;
					int num42 = (num39 << widthShift) + num38;
					int num43 = num42 + num40;
					int num44 = num42 + num41;
					int num45 = num44 + num40;
					byte b3 = this.imagePixels[num42];
					byte b4 = this.imagePixels[num43];
					byte b5 = this.imagePixels[num44];
					byte b6 = this.imagePixels[num45];
					int num46 = (int)b3 + (b * (b4 - b3) >> 8);
					int num47 = (int)b5 + (b * (b6 - b5) >> 8);
					int num48 = num46 + ((int)b2 * (num47 - num46) >> 8);
					num48 = num48 * (int)disp >> 8;
					int num49 = (k >> 8) * size + (l >> 8);
					num48 = (int)this.targetPixels[num49] + num48;
					num48 = ((num48 < 255) ? num48 : 255);
					this.targetPixels[num49] = (byte)num48;
				}
				num32 += num17;
				num33 += num18;
				num34 += num19;
				num35 += num30;
				num36 += num31;
			}
			num14 += num20;
			num15 += num21;
			num16 += num22;
		}
	}

	// Token: 0x06003265 RID: 12901
	[DllImport("RustNative", EntryPoint = "Water_RasterClearTile")]
	private static extern void RasterClearTile_Native(ref byte pixels, int offset, int stride, int width, int height);

	// Token: 0x06003266 RID: 12902
	[DllImport("RustNative", EntryPoint = "Water_RasterBindImage")]
	private static extern void RasterBindImage_Native(ref WaterDynamics.ImageDesc desc, ref byte pixels);

	// Token: 0x06003267 RID: 12903
	[DllImport("RustNative", EntryPoint = "Water_RasterBindTarget")]
	private static extern void RasterBindTarget_Native(ref WaterDynamics.TargetDesc desc, ref byte pixels, ref byte drawTileTable, ref ushort drawTileList, ref int drawTileCount);

	// Token: 0x06003268 RID: 12904
	[DllImport("RustNative", EntryPoint = "Water_RasterInteraction")]
	private static extern void RasterInteraction_Native(Vector2 pos, Vector2 scale, float rotation, float disp, float dist);

	// Token: 0x06003269 RID: 12905 RVA: 0x00136A2A File Offset: 0x00134C2A
	public static void SafeDestroy<T>(ref T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
			obj = default(T);
		}
	}

	// Token: 0x0600326A RID: 12906 RVA: 0x00136A58 File Offset: 0x00134C58
	public static T SafeDestroy<T>(T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		return default(T);
	}

	// Token: 0x0600326B RID: 12907 RVA: 0x00136A87 File Offset: 0x00134C87
	public static void SafeRelease<T>(ref T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}

	// Token: 0x0600326C RID: 12908 RVA: 0x00136AB0 File Offset: 0x00134CB0
	public static T SafeRelease<T>(T obj) where T : class, IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
		}
		return default(T);
	}

	// Token: 0x17000414 RID: 1044
	// (get) Token: 0x0600326E RID: 12910 RVA: 0x00136AE2 File Offset: 0x00134CE2
	// (set) Token: 0x0600326D RID: 12909 RVA: 0x00136AD9 File Offset: 0x00134CD9
	public bool IsInitialized { get; private set; }

	// Token: 0x0600326F RID: 12911 RVA: 0x00136AEA File Offset: 0x00134CEA
	public static void RegisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Add(interaction);
	}

	// Token: 0x06003270 RID: 12912 RVA: 0x00136AF8 File Offset: 0x00134CF8
	public static void UnregisterInteraction(WaterInteraction interaction)
	{
		WaterDynamics.interactions.Remove(interaction);
	}

	// Token: 0x06003271 RID: 12913 RVA: 0x00136B08 File Offset: 0x00134D08
	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			WaterDynamics.ImageDesc imageDesc = default(WaterDynamics.ImageDesc);
			byte[] array = new byte[1];
			WaterDynamics.RasterBindImage_Native(ref imageDesc, ref array[0]);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[WaterDynamics] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	// Token: 0x06003272 RID: 12914 RVA: 0x00136B58 File Offset: 0x00134D58
	public void Initialize(Vector3 areaPosition, Vector3 areaSize)
	{
		this.target = new WaterDynamics.Target(this, areaPosition, areaSize);
		this.useNativePath = this.SupportsNativePath();
		this.IsInitialized = true;
	}

	// Token: 0x06003273 RID: 12915 RVA: 0x00136B7B File Offset: 0x00134D7B
	public bool TryInitialize()
	{
		if (!this.IsInitialized && TerrainMeta.Data != null)
		{
			this.Initialize(TerrainMeta.Position, TerrainMeta.Data.size);
			return true;
		}
		return false;
	}

	// Token: 0x06003274 RID: 12916 RVA: 0x00136BAA File Offset: 0x00134DAA
	public void Shutdown()
	{
		if (this.target != null)
		{
			this.target.Destroy();
			this.target = null;
		}
		this.IsInitialized = false;
	}

	// Token: 0x06003275 RID: 12917 RVA: 0x00136BCD File Offset: 0x00134DCD
	public void OnEnable()
	{
		this.TryInitialize();
	}

	// Token: 0x06003276 RID: 12918 RVA: 0x00136BD6 File Offset: 0x00134DD6
	public void OnDisable()
	{
		this.Shutdown();
	}

	// Token: 0x06003277 RID: 12919 RVA: 0x00136BDE File Offset: 0x00134DDE
	public void Update()
	{
		if (!(WaterSystem.Instance == null))
		{
			if (this.IsInitialized)
			{
				return;
			}
			this.TryInitialize();
		}
	}

	// Token: 0x06003278 RID: 12920 RVA: 0x00136C00 File Offset: 0x00134E00
	private void ProcessInteractions()
	{
		foreach (WaterInteraction waterInteraction in WaterDynamics.interactions)
		{
			if (!(waterInteraction == null))
			{
				waterInteraction.UpdateTransform();
			}
		}
	}

	// Token: 0x06003279 RID: 12921 RVA: 0x00029CA8 File Offset: 0x00027EA8
	public float SampleHeight(Vector3 pos)
	{
		return 0f;
	}

	// Token: 0x02000E2C RID: 3628
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct ImageDesc
	{
		// Token: 0x04004A62 RID: 19042
		public int width;

		// Token: 0x04004A63 RID: 19043
		public int height;

		// Token: 0x04004A64 RID: 19044
		public int maxWidth;

		// Token: 0x04004A65 RID: 19045
		public int maxHeight;

		// Token: 0x04004A66 RID: 19046
		public int widthShift;

		// Token: 0x0600521E RID: 21022 RVA: 0x001AF2F4 File Offset: 0x001AD4F4
		public ImageDesc(Texture2D tex)
		{
			this.width = tex.width;
			this.height = tex.height;
			this.maxWidth = tex.width - 1;
			this.maxHeight = tex.height - 1;
			this.widthShift = (int)Mathf.Log((float)tex.width, 2f);
		}

		// Token: 0x0600521F RID: 21023 RVA: 0x001AF34D File Offset: 0x001AD54D
		public void Clear()
		{
			this.width = 0;
			this.height = 0;
			this.maxWidth = 0;
			this.maxHeight = 0;
			this.widthShift = 0;
		}
	}

	// Token: 0x02000E2D RID: 3629
	public class Image
	{
		// Token: 0x04004A67 RID: 19047
		public WaterDynamics.ImageDesc desc;

		// Token: 0x04004A69 RID: 19049
		public byte[] pixels;

		// Token: 0x170006E0 RID: 1760
		// (get) Token: 0x06005220 RID: 21024 RVA: 0x001AF372 File Offset: 0x001AD572
		// (set) Token: 0x06005221 RID: 21025 RVA: 0x001AF37A File Offset: 0x001AD57A
		public Texture2D texture { get; private set; }

		// Token: 0x06005222 RID: 21026 RVA: 0x001AF383 File Offset: 0x001AD583
		public Image(Texture2D tex)
		{
			this.desc = new WaterDynamics.ImageDesc(tex);
			this.texture = tex;
			this.pixels = this.GetDisplacementPixelsFromTexture(tex);
		}

		// Token: 0x06005223 RID: 21027 RVA: 0x001AF3AB File Offset: 0x001AD5AB
		public void Destroy()
		{
			this.desc.Clear();
			this.texture = null;
			this.pixels = null;
		}

		// Token: 0x06005224 RID: 21028 RVA: 0x001AF3C8 File Offset: 0x001AD5C8
		private byte[] GetDisplacementPixelsFromTexture(Texture2D tex)
		{
			Color32[] pixels = tex.GetPixels32();
			byte[] array = new byte[pixels.Length];
			for (int i = 0; i < pixels.Length; i++)
			{
				array[i] = pixels[i].b;
			}
			return array;
		}
	}

	// Token: 0x02000E2E RID: 3630
	private struct Point2D
	{
		// Token: 0x04004A6A RID: 19050
		public int x;

		// Token: 0x04004A6B RID: 19051
		public int y;

		// Token: 0x06005225 RID: 21029 RVA: 0x001AF403 File Offset: 0x001AD603
		public Point2D(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		// Token: 0x06005226 RID: 21030 RVA: 0x001AF413 File Offset: 0x001AD613
		public Point2D(float x, float y)
		{
			this.x = (int)x;
			this.y = (int)y;
		}
	}

	// Token: 0x02000E2F RID: 3631
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TargetDesc
	{
		// Token: 0x04004A6C RID: 19052
		public int size;

		// Token: 0x04004A6D RID: 19053
		public int maxSize;

		// Token: 0x04004A6E RID: 19054
		public int maxSizeSubStep;

		// Token: 0x04004A6F RID: 19055
		public Vector2 areaOffset;

		// Token: 0x04004A70 RID: 19056
		public Vector2 areaToMapUV;

		// Token: 0x04004A71 RID: 19057
		public Vector2 areaToMapXY;

		// Token: 0x04004A72 RID: 19058
		public int tileSize;

		// Token: 0x04004A73 RID: 19059
		public int tileSizeShift;

		// Token: 0x04004A74 RID: 19060
		public int tileCount;

		// Token: 0x04004A75 RID: 19061
		public int tileMaxCount;

		// Token: 0x06005227 RID: 21031 RVA: 0x001AF428 File Offset: 0x001AD628
		public TargetDesc(Vector3 areaPosition, Vector3 areaSize)
		{
			this.size = 512;
			this.maxSize = this.size - 1;
			this.maxSizeSubStep = this.maxSize * 256;
			this.areaOffset = new Vector2(areaPosition.x, areaPosition.z);
			this.areaToMapUV = new Vector2(1f / areaSize.x, 1f / areaSize.z);
			this.areaToMapXY = this.areaToMapUV * (float)this.size;
			this.tileSize = Mathf.NextPowerOfTwo(Mathf.Max(this.size, 4096)) / 256;
			this.tileSizeShift = (int)Mathf.Log((float)this.tileSize, 2f);
			this.tileCount = Mathf.CeilToInt((float)this.size / (float)this.tileSize);
			this.tileMaxCount = this.tileCount - 1;
		}

		// Token: 0x06005228 RID: 21032 RVA: 0x001AF514 File Offset: 0x001AD714
		public void Clear()
		{
			this.areaOffset = Vector2.zero;
			this.areaToMapUV = Vector2.zero;
			this.areaToMapXY = Vector2.zero;
			this.size = 0;
			this.maxSize = 0;
			this.maxSizeSubStep = 0;
			this.tileSize = 0;
			this.tileSizeShift = 0;
			this.tileCount = 0;
			this.tileMaxCount = 0;
		}

		// Token: 0x06005229 RID: 21033 RVA: 0x001AF574 File Offset: 0x001AD774
		public ushort TileOffsetToXYOffset(ushort tileOffset, out int x, out int y, out int offset)
		{
			int num = (int)tileOffset % this.tileCount;
			int num2 = (int)tileOffset / this.tileCount;
			x = num * this.tileSize;
			y = num2 * this.tileSize;
			offset = y * this.size + x;
			return tileOffset;
		}

		// Token: 0x0600522A RID: 21034 RVA: 0x001AF5B7 File Offset: 0x001AD7B7
		public ushort TileOffsetToTileXYIndex(ushort tileOffset, out int tileX, out int tileY, out ushort tileIndex)
		{
			tileX = (int)tileOffset % this.tileCount;
			tileY = (int)tileOffset / this.tileCount;
			tileIndex = (ushort)(tileY * this.tileCount + tileX);
			return tileOffset;
		}

		// Token: 0x0600522B RID: 21035 RVA: 0x001AF5E0 File Offset: 0x001AD7E0
		public Vector2 WorldToRaster(Vector2 pos)
		{
			Vector2 result;
			result.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			result.y = (pos.y - this.areaOffset.y) * this.areaToMapXY.y;
			return result;
		}

		// Token: 0x0600522C RID: 21036 RVA: 0x001AF638 File Offset: 0x001AD838
		public Vector3 WorldToRaster(Vector3 pos)
		{
			Vector2 v;
			v.x = (pos.x - this.areaOffset.x) * this.areaToMapXY.x;
			v.y = (pos.z - this.areaOffset.y) * this.areaToMapXY.y;
			return v;
		}
	}

	// Token: 0x02000E30 RID: 3632
	public class Target
	{
		// Token: 0x04004A76 RID: 19062
		public WaterDynamics owner;

		// Token: 0x04004A77 RID: 19063
		public WaterDynamics.TargetDesc desc;

		// Token: 0x04004A78 RID: 19064
		private byte[] pixels;

		// Token: 0x04004A79 RID: 19065
		private byte[] clearTileTable;

		// Token: 0x04004A7A RID: 19066
		private SimpleList<ushort> clearTileList;

		// Token: 0x04004A7B RID: 19067
		private byte[] drawTileTable;

		// Token: 0x04004A7C RID: 19068
		private SimpleList<ushort> drawTileList;

		// Token: 0x04004A7D RID: 19069
		private const int MaxInteractionOffset = 100;

		// Token: 0x04004A7E RID: 19070
		private Vector3 prevCameraWorldPos;

		// Token: 0x04004A7F RID: 19071
		private Vector2i interactionOffset;

		// Token: 0x170006E1 RID: 1761
		// (get) Token: 0x0600522D RID: 21037 RVA: 0x001AF695 File Offset: 0x001AD895
		public WaterDynamics.TargetDesc Desc
		{
			get
			{
				return this.desc;
			}
		}

		// Token: 0x170006E2 RID: 1762
		// (get) Token: 0x0600522E RID: 21038 RVA: 0x001AF69D File Offset: 0x001AD89D
		public byte[] Pixels
		{
			get
			{
				return this.pixels;
			}
		}

		// Token: 0x170006E3 RID: 1763
		// (get) Token: 0x0600522F RID: 21039 RVA: 0x001AF6A5 File Offset: 0x001AD8A5
		public byte[] DrawTileTable
		{
			get
			{
				return this.drawTileTable;
			}
		}

		// Token: 0x170006E4 RID: 1764
		// (get) Token: 0x06005230 RID: 21040 RVA: 0x001AF6AD File Offset: 0x001AD8AD
		public SimpleList<ushort> DrawTileList
		{
			get
			{
				return this.drawTileList;
			}
		}

		// Token: 0x06005231 RID: 21041 RVA: 0x001AF6B5 File Offset: 0x001AD8B5
		public Target(WaterDynamics owner, Vector3 areaPosition, Vector3 areaSize)
		{
			this.owner = owner;
			this.desc = new WaterDynamics.TargetDesc(areaPosition, areaSize);
		}

		// Token: 0x06005232 RID: 21042 RVA: 0x001AF6D1 File Offset: 0x001AD8D1
		public void Destroy()
		{
			this.desc.Clear();
		}

		// Token: 0x06005233 RID: 21043 RVA: 0x001AF6DE File Offset: 0x001AD8DE
		private Texture2D CreateDynamicTexture(int size)
		{
			return new Texture2D(size, size, TextureFormat.ARGB32, false, true)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
		}

		// Token: 0x06005234 RID: 21044 RVA: 0x001AF6F8 File Offset: 0x001AD8F8
		private RenderTexture CreateRenderTexture(int size)
		{
			RenderTextureFormat format = SystemInfoEx.SupportsRenderTextureFormat(RenderTextureFormat.RHalf) ? RenderTextureFormat.RHalf : RenderTextureFormat.RFloat;
			RenderTexture renderTexture = new RenderTexture(size, size, 0, format, RenderTextureReadWrite.Linear);
			renderTexture.filterMode = FilterMode.Bilinear;
			renderTexture.wrapMode = TextureWrapMode.Clamp;
			renderTexture.Create();
			return renderTexture;
		}

		// Token: 0x06005235 RID: 21045 RVA: 0x001AF734 File Offset: 0x001AD934
		public void ClearTiles()
		{
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				int num;
				int num2;
				int num3;
				this.desc.TileOffsetToXYOffset(this.clearTileList[i], out num, out num2, out num3);
				int num4 = Mathf.Min(num + this.desc.tileSize, this.desc.size) - num;
				int num5 = Mathf.Min(num2 + this.desc.tileSize, this.desc.size) - num2;
				if (this.owner.useNativePath)
				{
					WaterDynamics.RasterClearTile_Native(ref this.pixels[0], num3, this.desc.size, num4, num5);
				}
				else
				{
					for (int j = 0; j < num5; j++)
					{
						Array.Clear(this.pixels, num3, num4);
						num3 += this.desc.size;
					}
				}
			}
		}

		// Token: 0x06005236 RID: 21046 RVA: 0x001AF81C File Offset: 0x001ADA1C
		public void ProcessTiles()
		{
			for (int i = 0; i < this.clearTileList.Count; i++)
			{
				int num2;
				int num3;
				ushort num4;
				ushort num = this.desc.TileOffsetToTileXYIndex(this.clearTileList[i], out num2, out num3, out num4);
				this.clearTileTable[(int)num] = 0;
				this.clearTileList[i] = ushort.MaxValue;
			}
			this.clearTileList.Clear();
			for (int j = 0; j < this.drawTileList.Count; j++)
			{
				int num2;
				int num3;
				ushort num4;
				ushort num5 = this.desc.TileOffsetToTileXYIndex(this.drawTileList[j], out num2, out num3, out num4);
				if (this.clearTileTable[(int)num4] == 0)
				{
					this.clearTileTable[(int)num4] = 1;
					this.clearTileList.Add(num4);
				}
				this.drawTileTable[(int)num5] = 0;
				this.drawTileList[j] = ushort.MaxValue;
			}
			this.drawTileList.Clear();
		}

		// Token: 0x06005237 RID: 21047 RVA: 0x000063A5 File Offset: 0x000045A5
		public void UpdateTiles()
		{
		}

		// Token: 0x06005238 RID: 21048 RVA: 0x000063A5 File Offset: 0x000045A5
		public void Prepare()
		{
		}

		// Token: 0x06005239 RID: 21049 RVA: 0x000063A5 File Offset: 0x000045A5
		public void Update()
		{
		}

		// Token: 0x0600523A RID: 21050 RVA: 0x000063A5 File Offset: 0x000045A5
		public void UpdateGlobalShaderProperties()
		{
		}
	}
}
