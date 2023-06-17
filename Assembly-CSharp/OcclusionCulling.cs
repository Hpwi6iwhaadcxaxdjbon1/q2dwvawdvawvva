using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using RustNative;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200099F RID: 2463
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Camera))]
public class OcclusionCulling : MonoBehaviour
{
	// Token: 0x040034EC RID: 13548
	public OcclusionCulling.DebugSettings debugSettings = new OcclusionCulling.DebugSettings();

	// Token: 0x040034ED RID: 13549
	private Material debugMipMat;

	// Token: 0x040034EE RID: 13550
	private const float debugDrawDuration = 0.0334f;

	// Token: 0x040034EF RID: 13551
	private Material downscaleMat;

	// Token: 0x040034F0 RID: 13552
	private Material blitCopyMat;

	// Token: 0x040034F1 RID: 13553
	private int hiZLevelCount;

	// Token: 0x040034F2 RID: 13554
	private int hiZWidth;

	// Token: 0x040034F3 RID: 13555
	private int hiZHeight;

	// Token: 0x040034F4 RID: 13556
	private RenderTexture depthTexture;

	// Token: 0x040034F5 RID: 13557
	private RenderTexture hiZTexture;

	// Token: 0x040034F6 RID: 13558
	private RenderTexture[] hiZLevels;

	// Token: 0x040034F7 RID: 13559
	private const int GridCellsPerAxis = 2097152;

	// Token: 0x040034F8 RID: 13560
	private const int GridHalfCellsPerAxis = 1048576;

	// Token: 0x040034F9 RID: 13561
	private const int GridMinHalfCellsPerAxis = -1048575;

	// Token: 0x040034FA RID: 13562
	private const int GridMaxHalfCellsPerAxis = 1048575;

	// Token: 0x040034FB RID: 13563
	private const float GridCellSize = 100f;

	// Token: 0x040034FC RID: 13564
	private const float GridHalfCellSize = 50f;

	// Token: 0x040034FD RID: 13565
	private const float GridRcpCellSize = 0.01f;

	// Token: 0x040034FE RID: 13566
	private const int GridPoolCapacity = 16384;

	// Token: 0x040034FF RID: 13567
	private const int GridPoolGranularity = 4096;

	// Token: 0x04003500 RID: 13568
	private static OcclusionCulling.HashedPool<OcclusionCulling.Cell> grid = new OcclusionCulling.HashedPool<OcclusionCulling.Cell>(16384, 4096);

	// Token: 0x04003501 RID: 13569
	private static Queue<OcclusionCulling.Cell> gridChanged = new Queue<OcclusionCulling.Cell>();

	// Token: 0x04003502 RID: 13570
	public ComputeShader computeShader;

	// Token: 0x04003503 RID: 13571
	public bool usePixelShaderFallback = true;

	// Token: 0x04003504 RID: 13572
	public bool useAsyncReadAPI;

	// Token: 0x04003505 RID: 13573
	private Camera camera;

	// Token: 0x04003506 RID: 13574
	private const int ComputeThreadsPerGroup = 64;

	// Token: 0x04003507 RID: 13575
	private const int InputBufferStride = 16;

	// Token: 0x04003508 RID: 13576
	private const int ResultBufferStride = 4;

	// Token: 0x04003509 RID: 13577
	private const int OccludeeMaxSlotsPerPool = 1048576;

	// Token: 0x0400350A RID: 13578
	private const int OccludeePoolGranularity = 2048;

	// Token: 0x0400350B RID: 13579
	private const int StateBufferGranularity = 2048;

	// Token: 0x0400350C RID: 13580
	private const int GridBufferGranularity = 256;

	// Token: 0x0400350D RID: 13581
	private static Queue<OccludeeState> statePool = new Queue<OccludeeState>();

	// Token: 0x0400350E RID: 13582
	private static OcclusionCulling.SimpleList<OccludeeState> staticOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x0400350F RID: 13583
	private static OcclusionCulling.SimpleList<OccludeeState.State> staticStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x04003510 RID: 13584
	private static OcclusionCulling.SimpleList<int> staticVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x04003511 RID: 13585
	private static OcclusionCulling.SimpleList<OccludeeState> dynamicOccludees = new OcclusionCulling.SimpleList<OccludeeState>(2048);

	// Token: 0x04003512 RID: 13586
	private static OcclusionCulling.SimpleList<OccludeeState.State> dynamicStates = new OcclusionCulling.SimpleList<OccludeeState.State>(2048);

	// Token: 0x04003513 RID: 13587
	private static OcclusionCulling.SimpleList<int> dynamicVisibilityChanged = new OcclusionCulling.SimpleList<int>(1024);

	// Token: 0x04003514 RID: 13588
	private static List<int> staticChanged = new List<int>(256);

	// Token: 0x04003515 RID: 13589
	private static Queue<int> staticRecycled = new Queue<int>();

	// Token: 0x04003516 RID: 13590
	private static List<int> dynamicChanged = new List<int>(1024);

	// Token: 0x04003517 RID: 13591
	private static Queue<int> dynamicRecycled = new Queue<int>();

	// Token: 0x04003518 RID: 13592
	private static OcclusionCulling.BufferSet staticSet = new OcclusionCulling.BufferSet();

	// Token: 0x04003519 RID: 13593
	private static OcclusionCulling.BufferSet dynamicSet = new OcclusionCulling.BufferSet();

	// Token: 0x0400351A RID: 13594
	private static OcclusionCulling.BufferSet gridSet = new OcclusionCulling.BufferSet();

	// Token: 0x0400351B RID: 13595
	private Vector4[] frustumPlanes = new Vector4[6];

	// Token: 0x0400351C RID: 13596
	private string[] frustumPropNames = new string[6];

	// Token: 0x0400351D RID: 13597
	private float[] matrixToFloatTemp = new float[16];

	// Token: 0x0400351E RID: 13598
	private Material fallbackMat;

	// Token: 0x0400351F RID: 13599
	private Material depthCopyMat;

	// Token: 0x04003520 RID: 13600
	private Matrix4x4 viewMatrix;

	// Token: 0x04003521 RID: 13601
	private Matrix4x4 projMatrix;

	// Token: 0x04003522 RID: 13602
	private Matrix4x4 viewProjMatrix;

	// Token: 0x04003523 RID: 13603
	private Matrix4x4 prevViewProjMatrix;

	// Token: 0x04003524 RID: 13604
	private Matrix4x4 invViewProjMatrix;

	// Token: 0x04003525 RID: 13605
	private bool useNativePath = true;

	// Token: 0x04003526 RID: 13606
	private static OcclusionCulling instance;

	// Token: 0x04003527 RID: 13607
	private static GraphicsDeviceType[] supportedDeviceTypes = new GraphicsDeviceType[]
	{
		GraphicsDeviceType.Direct3D11
	};

	// Token: 0x04003528 RID: 13608
	private static bool _enabled = false;

	// Token: 0x04003529 RID: 13609
	private static bool _safeMode = false;

	// Token: 0x0400352A RID: 13610
	private static OcclusionCulling.DebugFilter _debugShow = OcclusionCulling.DebugFilter.Off;

	// Token: 0x06003A88 RID: 14984 RVA: 0x0015A01E File Offset: 0x0015821E
	public static bool DebugFilterIsDynamic(int filter)
	{
		return filter == 1 || filter == 4;
	}

	// Token: 0x06003A89 RID: 14985 RVA: 0x0015A02A File Offset: 0x0015822A
	public static bool DebugFilterIsStatic(int filter)
	{
		return filter == 2 || filter == 4;
	}

	// Token: 0x06003A8A RID: 14986 RVA: 0x0015A036 File Offset: 0x00158236
	public static bool DebugFilterIsGrid(int filter)
	{
		return filter == 3 || filter == 4;
	}

	// Token: 0x06003A8B RID: 14987 RVA: 0x0015A042 File Offset: 0x00158242
	private void DebugInitialize()
	{
		this.debugMipMat = new Material(Shader.Find("Hidden/OcclusionCulling/DebugMip"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
	}

	// Token: 0x06003A8C RID: 14988 RVA: 0x0015A061 File Offset: 0x00158261
	private void DebugShutdown()
	{
		if (this.debugMipMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.debugMipMat);
			this.debugMipMat = null;
		}
	}

	// Token: 0x06003A8D RID: 14989 RVA: 0x0015A083 File Offset: 0x00158283
	private void DebugUpdate()
	{
		if (this.HiZReady)
		{
			this.debugSettings.showMainLod = Mathf.Clamp(this.debugSettings.showMainLod, 0, this.hiZLevels.Length - 1);
		}
	}

	// Token: 0x06003A8E RID: 14990 RVA: 0x000063A5 File Offset: 0x000045A5
	private void DebugDraw()
	{
	}

	// Token: 0x06003A8F RID: 14991 RVA: 0x0015A0B4 File Offset: 0x001582B4
	public static void NormalizePlane(ref Vector4 plane)
	{
		float num = Mathf.Sqrt(plane.x * plane.x + plane.y * plane.y + plane.z * plane.z);
		plane.x /= num;
		plane.y /= num;
		plane.z /= num;
		plane.w /= num;
	}

	// Token: 0x06003A90 RID: 14992 RVA: 0x0015A11C File Offset: 0x0015831C
	public static void ExtractFrustum(Matrix4x4 viewProjMatrix, ref Vector4[] planes)
	{
		planes[0].x = viewProjMatrix.m30 + viewProjMatrix.m00;
		planes[0].y = viewProjMatrix.m31 + viewProjMatrix.m01;
		planes[0].z = viewProjMatrix.m32 + viewProjMatrix.m02;
		planes[0].w = viewProjMatrix.m33 + viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[0]);
		planes[1].x = viewProjMatrix.m30 - viewProjMatrix.m00;
		planes[1].y = viewProjMatrix.m31 - viewProjMatrix.m01;
		planes[1].z = viewProjMatrix.m32 - viewProjMatrix.m02;
		planes[1].w = viewProjMatrix.m33 - viewProjMatrix.m03;
		OcclusionCulling.NormalizePlane(ref planes[1]);
		planes[2].x = viewProjMatrix.m30 - viewProjMatrix.m10;
		planes[2].y = viewProjMatrix.m31 - viewProjMatrix.m11;
		planes[2].z = viewProjMatrix.m32 - viewProjMatrix.m12;
		planes[2].w = viewProjMatrix.m33 - viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[2]);
		planes[3].x = viewProjMatrix.m30 + viewProjMatrix.m10;
		planes[3].y = viewProjMatrix.m31 + viewProjMatrix.m11;
		planes[3].z = viewProjMatrix.m32 + viewProjMatrix.m12;
		planes[3].w = viewProjMatrix.m33 + viewProjMatrix.m13;
		OcclusionCulling.NormalizePlane(ref planes[3]);
		planes[4].x = viewProjMatrix.m20;
		planes[4].y = viewProjMatrix.m21;
		planes[4].z = viewProjMatrix.m22;
		planes[4].w = viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[4]);
		planes[5].x = viewProjMatrix.m30 - viewProjMatrix.m20;
		planes[5].y = viewProjMatrix.m31 - viewProjMatrix.m21;
		planes[5].z = viewProjMatrix.m32 - viewProjMatrix.m22;
		planes[5].w = viewProjMatrix.m33 - viewProjMatrix.m23;
		OcclusionCulling.NormalizePlane(ref planes[5]);
	}

	// Token: 0x170004A2 RID: 1186
	// (get) Token: 0x06003A91 RID: 14993 RVA: 0x0015A3CB File Offset: 0x001585CB
	public bool HiZReady
	{
		get
		{
			return this.hiZTexture != null && this.hiZWidth > 0 && this.hiZHeight > 0;
		}
	}

	// Token: 0x06003A92 RID: 14994 RVA: 0x0015A3F0 File Offset: 0x001585F0
	public void CheckResizeHiZMap()
	{
		int pixelWidth = this.camera.pixelWidth;
		int pixelHeight = this.camera.pixelHeight;
		if (pixelWidth > 0 && pixelHeight > 0)
		{
			int num = pixelWidth / 4;
			int num2 = pixelHeight / 4;
			if (this.hiZLevels == null || this.hiZWidth != num || this.hiZHeight != num2)
			{
				this.InitializeHiZMap(num, num2);
				this.hiZWidth = num;
				this.hiZHeight = num2;
				if (this.debugSettings.log)
				{
					Debug.Log(string.Concat(new object[]
					{
						"[OcclusionCulling] Resized HiZ Map to ",
						this.hiZWidth,
						" x ",
						this.hiZHeight
					}));
				}
			}
		}
	}

	// Token: 0x06003A93 RID: 14995 RVA: 0x0015A4A4 File Offset: 0x001586A4
	private void InitializeHiZMap()
	{
		Shader shader = Shader.Find("Hidden/OcclusionCulling/DepthDownscale");
		Shader shader2 = Shader.Find("Hidden/OcclusionCulling/BlitCopy");
		this.downscaleMat = new Material(shader)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.blitCopyMat = new Material(shader2)
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.CheckResizeHiZMap();
	}

	// Token: 0x06003A94 RID: 14996 RVA: 0x0015A4F8 File Offset: 0x001586F8
	private void FinalizeHiZMap()
	{
		this.DestroyHiZMap();
		if (this.downscaleMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.downscaleMat);
			this.downscaleMat = null;
		}
		if (this.blitCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.blitCopyMat);
			this.blitCopyMat = null;
		}
	}

	// Token: 0x06003A95 RID: 14997 RVA: 0x0015A54C File Offset: 0x0015874C
	private void InitializeHiZMap(int width, int height)
	{
		this.DestroyHiZMap();
		width = Mathf.Clamp(width, 1, 65536);
		height = Mathf.Clamp(height, 1, 65536);
		int num = Mathf.Min(width, height);
		this.hiZLevelCount = (int)(Mathf.Log((float)num, 2f) + 1f);
		this.hiZLevels = new RenderTexture[this.hiZLevelCount];
		this.depthTexture = this.CreateDepthTexture("DepthTex", width, height, false);
		this.hiZTexture = this.CreateDepthTexture("HiZMapTex", width, height, true);
		for (int i = 0; i < this.hiZLevelCount; i++)
		{
			this.hiZLevels[i] = this.CreateDepthTextureMip("HiZMap" + i, width, height, i);
		}
	}

	// Token: 0x06003A96 RID: 14998 RVA: 0x0015A608 File Offset: 0x00158808
	private void DestroyHiZMap()
	{
		if (this.depthTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.depthTexture);
			this.depthTexture = null;
		}
		if (this.hiZTexture != null)
		{
			RenderTexture.active = null;
			UnityEngine.Object.DestroyImmediate(this.hiZTexture);
			this.hiZTexture = null;
		}
		if (this.hiZLevels != null)
		{
			for (int i = 0; i < this.hiZLevels.Length; i++)
			{
				UnityEngine.Object.DestroyImmediate(this.hiZLevels[i]);
			}
			this.hiZLevels = null;
		}
	}

	// Token: 0x06003A97 RID: 14999 RVA: 0x0015A690 File Offset: 0x00158890
	private RenderTexture CreateDepthTexture(string name, int width, int height, bool mips = false)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = mips;
		renderTexture.autoGenerateMips = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06003A98 RID: 15000 RVA: 0x0015A6C8 File Offset: 0x001588C8
	private RenderTexture CreateDepthTextureMip(string name, int width, int height, int mip)
	{
		int width2 = width >> mip;
		int height2 = height >> mip;
		RenderTexture renderTexture = new RenderTexture(width2, height2, 0, RenderTextureFormat.RFloat, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.useMipMap = false;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.filterMode = FilterMode.Point;
		renderTexture.Create();
		return renderTexture;
	}

	// Token: 0x06003A99 RID: 15001 RVA: 0x0015A711 File Offset: 0x00158911
	public void GrabDepthTexture()
	{
		if (this.depthTexture != null)
		{
			UnityEngine.Graphics.Blit(null, this.depthTexture, this.depthCopyMat, 0);
		}
	}

	// Token: 0x06003A9A RID: 15002 RVA: 0x0015A734 File Offset: 0x00158934
	public void GenerateHiZMipChain()
	{
		if (this.HiZReady)
		{
			bool flag = true;
			this.depthCopyMat.SetMatrix("_CameraReprojection", this.prevViewProjMatrix * this.invViewProjMatrix);
			this.depthCopyMat.SetFloat("_FrustumNoDataDepth", flag ? 1f : 0f);
			UnityEngine.Graphics.Blit(this.depthTexture, this.hiZLevels[0], this.depthCopyMat, 1);
			for (int i = 1; i < this.hiZLevels.Length; i++)
			{
				RenderTexture renderTexture = this.hiZLevels[i - 1];
				RenderTexture dest = this.hiZLevels[i];
				int pass = ((renderTexture.width & 1) == 0 && (renderTexture.height & 1) == 0) ? 0 : 1;
				this.downscaleMat.SetTexture("_MainTex", renderTexture);
				UnityEngine.Graphics.Blit(renderTexture, dest, this.downscaleMat, pass);
			}
			for (int j = 0; j < this.hiZLevels.Length; j++)
			{
				UnityEngine.Graphics.SetRenderTarget(this.hiZTexture, j);
				UnityEngine.Graphics.Blit(this.hiZLevels[j], this.blitCopyMat);
			}
		}
	}

	// Token: 0x06003A9B RID: 15003 RVA: 0x0015A844 File Offset: 0x00158A44
	private void DebugDrawGizmos()
	{
		Camera component = base.GetComponent<Camera>();
		Gizmos.color = new Color(0.75f, 0.75f, 0f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		Gizmos.DrawFrustum(Vector3.zero, component.fieldOfView, component.farClipPlane, component.nearClipPlane, component.aspect);
		Gizmos.color = Color.red;
		Gizmos.matrix = Matrix4x4.identity;
		Matrix4x4 worldToCameraMatrix = component.worldToCameraMatrix;
		Matrix4x4 matrix4x = GL.GetGPUProjectionMatrix(component.projectionMatrix, false) * worldToCameraMatrix;
		Vector4[] array = new Vector4[6];
		OcclusionCulling.ExtractFrustum(matrix4x, ref array);
		for (int i = 0; i < array.Length; i++)
		{
			Vector3 a = new Vector3(array[i].x, array[i].y, array[i].z);
			float w = array[i].w;
			Vector3 vector = -a * w;
			Gizmos.DrawLine(vector, vector * 2f);
		}
	}

	// Token: 0x06003A9C RID: 15004 RVA: 0x0015A95C File Offset: 0x00158B5C
	private static int floor(float x)
	{
		int num = (int)x;
		if (x >= (float)num)
		{
			return num;
		}
		return num - 1;
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x0015A978 File Offset: 0x00158B78
	public static OcclusionCulling.Cell RegisterToGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		int num4 = Mathf.Clamp(num, -1048575, 1048575);
		int num5 = Mathf.Clamp(num2, -1048575, 1048575);
		int num6 = Mathf.Clamp(num3, -1048575, 1048575);
		ulong num7 = (ulong)((long)((num4 >= 0) ? num4 : (num4 + 1048575)));
		ulong num8 = (ulong)((long)((num5 >= 0) ? num5 : (num5 + 1048575)));
		ulong num9 = (ulong)((long)((num6 >= 0) ? num6 : (num6 + 1048575)));
		ulong key = num7 << 42 | num8 << 21 | num9;
		OcclusionCulling.Cell cell;
		bool flag = OcclusionCulling.grid.TryGetValue(key, out cell);
		if (!flag)
		{
			Vector3 center = default(Vector3);
			center.x = (float)num * 100f + 50f;
			center.y = (float)num2 * 100f + 50f;
			center.z = (float)num3 * 100f + 50f;
			Vector3 size = new Vector3(100f, 100f, 100f);
			cell = OcclusionCulling.grid.Add(key, 16).Initialize(num, num2, num3, new Bounds(center, size));
		}
		OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
		if (!flag || !smartList.Contains(occludee))
		{
			occludee.cell = cell;
			smartList.Add(occludee, 16);
			OcclusionCulling.gridChanged.Enqueue(cell);
		}
		return cell;
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x0015AB58 File Offset: 0x00158D58
	public static void UpdateInGrid(OccludeeState occludee)
	{
		int num = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.x * 0.01f);
		int num2 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.y * 0.01f);
		int num3 = OcclusionCulling.floor(occludee.states.array[occludee.slot].sphereBounds.z * 0.01f);
		if (num != occludee.cell.x || num2 != occludee.cell.y || num3 != occludee.cell.z)
		{
			OcclusionCulling.UnregisterFromGrid(occludee);
			OcclusionCulling.RegisterToGrid(occludee);
		}
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x0015AC20 File Offset: 0x00158E20
	public static void UnregisterFromGrid(OccludeeState occludee)
	{
		OcclusionCulling.Cell cell = occludee.cell;
		OcclusionCulling.SmartList smartList = occludee.isStatic ? cell.staticBucket : cell.dynamicBucket;
		OcclusionCulling.gridChanged.Enqueue(cell);
		smartList.Remove(occludee);
		if (cell.staticBucket.Count == 0 && cell.dynamicBucket.Count == 0)
		{
			OcclusionCulling.grid.Remove(cell);
			cell.Reset();
		}
		occludee.cell = null;
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x0015AC90 File Offset: 0x00158E90
	public void UpdateGridBuffers()
	{
		if (OcclusionCulling.gridSet.CheckResize(OcclusionCulling.grid.Size, 256))
		{
			if (this.debugSettings.log)
			{
				Debug.Log("[OcclusionCulling] Resized grid to " + OcclusionCulling.grid.Size);
			}
			for (int i = 0; i < OcclusionCulling.grid.Size; i++)
			{
				if (OcclusionCulling.grid[i] != null)
				{
					OcclusionCulling.gridChanged.Enqueue(OcclusionCulling.grid[i]);
				}
			}
		}
		bool flag = OcclusionCulling.gridChanged.Count > 0;
		while (OcclusionCulling.gridChanged.Count > 0)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.gridChanged.Dequeue();
			OcclusionCulling.gridSet.inputData[cell.hashedPoolIndex] = cell.sphereBounds;
		}
		if (flag)
		{
			OcclusionCulling.gridSet.UploadData();
		}
	}

	// Token: 0x170004A3 RID: 1187
	// (get) Token: 0x06003AA1 RID: 15009 RVA: 0x0015AD6F File Offset: 0x00158F6F
	public static OcclusionCulling Instance
	{
		get
		{
			return OcclusionCulling.instance;
		}
	}

	// Token: 0x170004A4 RID: 1188
	// (get) Token: 0x06003AA2 RID: 15010 RVA: 0x0015AD76 File Offset: 0x00158F76
	public static bool Supported
	{
		get
		{
			return OcclusionCulling.supportedDeviceTypes.Contains(SystemInfo.graphicsDeviceType);
		}
	}

	// Token: 0x170004A5 RID: 1189
	// (get) Token: 0x06003AA3 RID: 15011 RVA: 0x0015AD87 File Offset: 0x00158F87
	// (set) Token: 0x06003AA4 RID: 15012 RVA: 0x0015AD8E File Offset: 0x00158F8E
	public static bool Enabled
	{
		get
		{
			return OcclusionCulling._enabled;
		}
		set
		{
			OcclusionCulling._enabled = value;
			if (OcclusionCulling.instance != null)
			{
				OcclusionCulling.instance.enabled = value;
			}
		}
	}

	// Token: 0x170004A6 RID: 1190
	// (get) Token: 0x06003AA5 RID: 15013 RVA: 0x0015ADAE File Offset: 0x00158FAE
	// (set) Token: 0x06003AA6 RID: 15014 RVA: 0x0015ADB5 File Offset: 0x00158FB5
	public static bool SafeMode
	{
		get
		{
			return OcclusionCulling._safeMode;
		}
		set
		{
			OcclusionCulling._safeMode = value;
		}
	}

	// Token: 0x170004A7 RID: 1191
	// (get) Token: 0x06003AA7 RID: 15015 RVA: 0x0015ADBD File Offset: 0x00158FBD
	// (set) Token: 0x06003AA8 RID: 15016 RVA: 0x0015ADC4 File Offset: 0x00158FC4
	public static OcclusionCulling.DebugFilter DebugShow
	{
		get
		{
			return OcclusionCulling._debugShow;
		}
		set
		{
			OcclusionCulling._debugShow = value;
		}
	}

	// Token: 0x06003AA9 RID: 15017 RVA: 0x0015ADCC File Offset: 0x00158FCC
	private static void GrowStatePool()
	{
		for (int i = 0; i < 2048; i++)
		{
			OcclusionCulling.statePool.Enqueue(new OccludeeState());
		}
	}

	// Token: 0x06003AAA RID: 15018 RVA: 0x0015ADF8 File Offset: 0x00158FF8
	private static OccludeeState Allocate()
	{
		if (OcclusionCulling.statePool.Count == 0)
		{
			OcclusionCulling.GrowStatePool();
		}
		return OcclusionCulling.statePool.Dequeue();
	}

	// Token: 0x06003AAB RID: 15019 RVA: 0x0015AE15 File Offset: 0x00159015
	private static void Release(OccludeeState state)
	{
		OcclusionCulling.statePool.Enqueue(state);
	}

	// Token: 0x06003AAC RID: 15020 RVA: 0x0015AE24 File Offset: 0x00159024
	private void Awake()
	{
		OcclusionCulling.instance = this;
		this.camera = base.GetComponent<Camera>();
		for (int i = 0; i < 6; i++)
		{
			this.frustumPropNames[i] = "_FrustumPlane" + i;
		}
	}

	// Token: 0x06003AAD RID: 15021 RVA: 0x0015AE68 File Offset: 0x00159068
	private void OnEnable()
	{
		if (!OcclusionCulling.Enabled)
		{
			OcclusionCulling.Enabled = false;
			return;
		}
		if (!OcclusionCulling.Supported)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to graphics device type " + SystemInfo.graphicsDeviceType + " not supported.");
			OcclusionCulling.Enabled = false;
			return;
		}
		this.usePixelShaderFallback = (this.usePixelShaderFallback || !SystemInfo.supportsComputeShaders || this.computeShader == null || !this.computeShader.HasKernel("compute_cull"));
		this.useNativePath = (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11 && this.SupportsNativePath());
		this.useAsyncReadAPI = (!this.useNativePath && SystemInfo.supportsAsyncGPUReadback);
		if (!this.useNativePath && !this.useAsyncReadAPI)
		{
			Debug.LogWarning("[OcclusionCulling] Disabled due to unsupported Async GPU Reads on device " + SystemInfo.graphicsDeviceType);
			OcclusionCulling.Enabled = false;
			return;
		}
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			OcclusionCulling.staticChanged.Add(i);
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			OcclusionCulling.dynamicChanged.Add(j);
		}
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat = new Material(Shader.Find("Hidden/OcclusionCulling/Culling"))
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		OcclusionCulling.staticSet.Attach(this);
		OcclusionCulling.dynamicSet.Attach(this);
		OcclusionCulling.gridSet.Attach(this);
		this.depthCopyMat = new Material(Shader.Find("Hidden/OcclusionCulling/DepthCopy"))
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		this.InitializeHiZMap();
		this.UpdateCameraMatrices(true);
	}

	// Token: 0x06003AAE RID: 15022 RVA: 0x0015AFF8 File Offset: 0x001591F8
	private bool SupportsNativePath()
	{
		bool result = true;
		try
		{
			OccludeeState.State state = default(OccludeeState.State);
			Color32 color = new Color32(0, 0, 0, 0);
			Vector4 zero = Vector4.zero;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			OcclusionCulling.ProcessOccludees_Native(ref state, ref num, 0, ref color, 0, ref num2, ref num3, ref zero, 0f, 0U);
		}
		catch (EntryPointNotFoundException)
		{
			Debug.Log("[OcclusionCulling] Fast native path not available. Reverting to managed fallback.");
			result = false;
		}
		return result;
	}

	// Token: 0x06003AAF RID: 15023 RVA: 0x0015B068 File Offset: 0x00159268
	private void OnDisable()
	{
		if (this.fallbackMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.fallbackMat);
			this.fallbackMat = null;
		}
		if (this.depthCopyMat != null)
		{
			UnityEngine.Object.DestroyImmediate(this.depthCopyMat);
			this.depthCopyMat = null;
		}
		OcclusionCulling.staticSet.Dispose(true);
		OcclusionCulling.dynamicSet.Dispose(true);
		OcclusionCulling.gridSet.Dispose(true);
		this.FinalizeHiZMap();
	}

	// Token: 0x06003AB0 RID: 15024 RVA: 0x0015B0DC File Offset: 0x001592DC
	public static void MakeAllVisible()
	{
		for (int i = 0; i < OcclusionCulling.staticOccludees.Count; i++)
		{
			if (OcclusionCulling.staticOccludees[i] != null)
			{
				OcclusionCulling.staticOccludees[i].MakeVisible();
			}
		}
		for (int j = 0; j < OcclusionCulling.dynamicOccludees.Count; j++)
		{
			if (OcclusionCulling.dynamicOccludees[j] != null)
			{
				OcclusionCulling.dynamicOccludees[j].MakeVisible();
			}
		}
	}

	// Token: 0x06003AB1 RID: 15025 RVA: 0x0015B14D File Offset: 0x0015934D
	private void Update()
	{
		if (!OcclusionCulling.Enabled)
		{
			base.enabled = false;
			return;
		}
		this.CheckResizeHiZMap();
		this.DebugUpdate();
		this.DebugDraw();
	}

	// Token: 0x06003AB2 RID: 15026 RVA: 0x0015B170 File Offset: 0x00159370
	public static void RecursiveAddOccludees<T>(Transform transform, float minTimeVisible = 0.1f, bool isStatic = true, bool stickyGizmos = false) where T : Occludee
	{
		Renderer component = transform.GetComponent<Renderer>();
		Collider component2 = transform.GetComponent<Collider>();
		if (component != null && component2 != null)
		{
			T t = component.gameObject.GetComponent<T>();
			t = ((t == null) ? component.gameObject.AddComponent<T>() : t);
			t.minTimeVisible = minTimeVisible;
			t.isStatic = isStatic;
			t.stickyGizmos = stickyGizmos;
			t.Register();
		}
		foreach (object obj in transform)
		{
			OcclusionCulling.RecursiveAddOccludees<T>((Transform)obj, minTimeVisible, isStatic, stickyGizmos);
		}
	}

	// Token: 0x06003AB3 RID: 15027 RVA: 0x0015B240 File Offset: 0x00159440
	private static int FindFreeSlot(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled)
	{
		int result;
		if (recycled.Count > 0)
		{
			result = recycled.Dequeue();
		}
		else
		{
			if (occludees.Count == occludees.Capacity)
			{
				int num = Mathf.Min(occludees.Capacity + 2048, 1048576);
				if (num > 0)
				{
					occludees.Capacity = num;
					states.Capacity = num;
				}
			}
			if (occludees.Count < occludees.Capacity)
			{
				result = occludees.Count;
				occludees.Add(null);
				states.Add(default(OccludeeState.State));
			}
			else
			{
				result = -1;
			}
		}
		return result;
	}

	// Token: 0x06003AB4 RID: 15028 RVA: 0x0015B2C8 File Offset: 0x001594C8
	public static OccludeeState GetStateById(int id)
	{
		if (id < 0 || id >= 2097152)
		{
			return null;
		}
		bool flag = id < 1048576;
		int index = flag ? id : (id - 1048576);
		if (flag)
		{
			return OcclusionCulling.staticOccludees[index];
		}
		return OcclusionCulling.dynamicOccludees[index];
	}

	// Token: 0x06003AB5 RID: 15029 RVA: 0x0015B314 File Offset: 0x00159514
	public static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged = null)
	{
		int num;
		if (isStatic)
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged, OcclusionCulling.staticSet, OcclusionCulling.staticVisibilityChanged);
		}
		else
		{
			num = OcclusionCulling.RegisterOccludee(center, radius, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicVisibilityChanged);
		}
		if (num >= 0 && !isStatic)
		{
			return num + 1048576;
		}
		return num;
	}

	// Token: 0x06003AB6 RID: 15030 RVA: 0x0015B398 File Offset: 0x00159598
	private static int RegisterOccludee(Vector3 center, float radius, bool isVisible, float minTimeVisible, bool isStatic, int layer, OcclusionCulling.OnVisibilityChanged onVisibilityChanged, OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, Queue<int> recycled, List<int> changed, OcclusionCulling.BufferSet set, OcclusionCulling.SimpleList<int> visibilityChanged)
	{
		int num = OcclusionCulling.FindFreeSlot(occludees, states, recycled);
		if (num >= 0)
		{
			Vector4 sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OccludeeState occludeeState = OcclusionCulling.Allocate().Initialize(states, set, num, sphereBounds, isVisible, minTimeVisible, isStatic, layer, onVisibilityChanged);
			occludeeState.cell = OcclusionCulling.RegisterToGrid(occludeeState);
			occludees[num] = occludeeState;
			changed.Add(num);
			if (states.array[num].isVisible > 0 != occludeeState.cell.isVisible)
			{
				visibilityChanged.Add(num);
			}
		}
		return num;
	}

	// Token: 0x06003AB7 RID: 15031 RVA: 0x0015B430 File Offset: 0x00159630
	public static void UnregisterOccludee(int id)
	{
		if (id >= 0 && id < 2097152)
		{
			bool flag = id < 1048576;
			int slot = flag ? id : (id - 1048576);
			if (flag)
			{
				OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.staticOccludees, OcclusionCulling.staticRecycled, OcclusionCulling.staticChanged);
				return;
			}
			OcclusionCulling.UnregisterOccludee(slot, OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicRecycled, OcclusionCulling.dynamicChanged);
		}
	}

	// Token: 0x06003AB8 RID: 15032 RVA: 0x0015B48C File Offset: 0x0015968C
	private static void UnregisterOccludee(int slot, OcclusionCulling.SimpleList<OccludeeState> occludees, Queue<int> recycled, List<int> changed)
	{
		OccludeeState occludeeState = occludees[slot];
		OcclusionCulling.UnregisterFromGrid(occludeeState);
		recycled.Enqueue(slot);
		changed.Add(slot);
		OcclusionCulling.Release(occludeeState);
		occludees[slot] = null;
		occludeeState.Invalidate();
	}

	// Token: 0x06003AB9 RID: 15033 RVA: 0x0015B4BC File Offset: 0x001596BC
	public static void UpdateDynamicOccludee(int id, Vector3 center, float radius)
	{
		int num = id - 1048576;
		if (num >= 0 && num < 1048576)
		{
			OcclusionCulling.dynamicStates.array[num].sphereBounds = new Vector4(center.x, center.y, center.z, radius);
			OcclusionCulling.dynamicChanged.Add(num);
		}
	}

	// Token: 0x06003ABA RID: 15034 RVA: 0x0015B518 File Offset: 0x00159718
	private void UpdateBuffers(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.BufferSet set, List<int> changed, bool isStatic)
	{
		int count = occludees.Count;
		bool flag = changed.Count > 0;
		set.CheckResize(count, 2048);
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				if (!isStatic)
				{
					OcclusionCulling.UpdateInGrid(occludeeState);
				}
				set.inputData[num] = states[num].sphereBounds;
			}
			else
			{
				set.inputData[num] = Vector4.zero;
			}
		}
		changed.Clear();
		if (flag)
		{
			set.UploadData();
		}
	}

	// Token: 0x06003ABB RID: 15035 RVA: 0x0015B5BC File Offset: 0x001597BC
	private void UpdateCameraMatrices(bool starting = false)
	{
		if (!starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
		Matrix4x4 proj = Matrix4x4.Perspective(this.camera.fieldOfView, this.camera.aspect, this.camera.nearClipPlane, this.camera.farClipPlane);
		this.viewMatrix = this.camera.worldToCameraMatrix;
		this.projMatrix = GL.GetGPUProjectionMatrix(proj, false);
		this.viewProjMatrix = this.projMatrix * this.viewMatrix;
		this.invViewProjMatrix = Matrix4x4.Inverse(this.viewProjMatrix);
		if (starting)
		{
			this.prevViewProjMatrix = this.viewProjMatrix;
		}
	}

	// Token: 0x06003ABC RID: 15036 RVA: 0x0015B660 File Offset: 0x00159860
	private void OnPreCull()
	{
		this.UpdateCameraMatrices(false);
		this.GenerateHiZMipChain();
		this.PrepareAndDispatch();
		this.IssueRead();
		if (OcclusionCulling.grid.Size <= OcclusionCulling.gridSet.resultData.Length)
		{
			this.RetrieveAndApplyVisibility();
			return;
		}
		Debug.LogWarning(string.Concat(new object[]
		{
			"[OcclusionCulling] Grid size and result capacity are out of sync: ",
			OcclusionCulling.grid.Size,
			", ",
			OcclusionCulling.gridSet.resultData.Length
		}));
	}

	// Token: 0x06003ABD RID: 15037 RVA: 0x0015B6EC File Offset: 0x001598EC
	private void OnPostRender()
	{
		bool sRGBWrite = GL.sRGBWrite;
		RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
		RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
		this.GrabDepthTexture();
		UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		GL.sRGBWrite = sRGBWrite;
	}

	// Token: 0x06003ABE RID: 15038 RVA: 0x0015B71C File Offset: 0x0015991C
	private float[] MatrixToFloatArray(Matrix4x4 m)
	{
		int i = 0;
		int num = 0;
		while (i < 4)
		{
			for (int j = 0; j < 4; j++)
			{
				this.matrixToFloatTemp[num++] = m[j, i];
			}
			i++;
		}
		return this.matrixToFloatTemp;
	}

	// Token: 0x06003ABF RID: 15039 RVA: 0x0015B760 File Offset: 0x00159960
	private void PrepareAndDispatch()
	{
		Vector2 v = new Vector2((float)this.hiZWidth, (float)this.hiZHeight);
		OcclusionCulling.ExtractFrustum(this.viewProjMatrix, ref this.frustumPlanes);
		bool flag = true;
		if (this.usePixelShaderFallback)
		{
			this.fallbackMat.SetTexture("_HiZMap", this.hiZTexture);
			this.fallbackMat.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.fallbackMat.SetMatrix("_ViewMatrix", this.viewMatrix);
			this.fallbackMat.SetMatrix("_ProjMatrix", this.projMatrix);
			this.fallbackMat.SetMatrix("_ViewProjMatrix", this.viewProjMatrix);
			this.fallbackMat.SetVector("_CameraWorldPos", base.transform.position);
			this.fallbackMat.SetVector("_ViewportSize", v);
			this.fallbackMat.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int i = 0; i < 6; i++)
			{
				this.fallbackMat.SetVector(this.frustumPropNames[i], this.frustumPlanes[i]);
			}
		}
		else
		{
			this.computeShader.SetTexture(0, "_HiZMap", this.hiZTexture);
			this.computeShader.SetFloat("_HiZMaxLod", (float)(this.hiZLevelCount - 1));
			this.computeShader.SetFloats("_ViewMatrix", this.MatrixToFloatArray(this.viewMatrix));
			this.computeShader.SetFloats("_ProjMatrix", this.MatrixToFloatArray(this.projMatrix));
			this.computeShader.SetFloats("_ViewProjMatrix", this.MatrixToFloatArray(this.viewProjMatrix));
			this.computeShader.SetVector("_CameraWorldPos", base.transform.position);
			this.computeShader.SetVector("_ViewportSize", v);
			this.computeShader.SetFloat("_FrustumCull", flag ? 0f : 1f);
			for (int j = 0; j < 6; j++)
			{
				this.computeShader.SetVector(this.frustumPropNames[j], this.frustumPlanes[j]);
			}
		}
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticSet, OcclusionCulling.staticChanged, true);
			OcclusionCulling.staticSet.Dispatch(OcclusionCulling.staticOccludees.Count);
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			this.UpdateBuffers(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicSet, OcclusionCulling.dynamicChanged, false);
			OcclusionCulling.dynamicSet.Dispatch(OcclusionCulling.dynamicOccludees.Count);
		}
		this.UpdateGridBuffers();
		OcclusionCulling.gridSet.Dispatch(OcclusionCulling.grid.Size);
	}

	// Token: 0x06003AC0 RID: 15040 RVA: 0x0015BA2C File Offset: 0x00159C2C
	private void IssueRead()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.IssueRead();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.IssueRead();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.IssueRead();
		}
		GL.IssuePluginEvent(RustNative.Graphics.GetRenderEventFunc(), 2);
	}

	// Token: 0x06003AC1 RID: 15041 RVA: 0x0015BA8C File Offset: 0x00159C8C
	public void ResetTiming(OcclusionCulling.SmartList bucket)
	{
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null)
			{
				occludeeState.states.array[occludeeState.slot].waitTime = 0f;
			}
		}
	}

	// Token: 0x06003AC2 RID: 15042 RVA: 0x0015BAD8 File Offset: 0x00159CD8
	public void ResetTiming()
	{
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null)
			{
				this.ResetTiming(cell.staticBucket);
				this.ResetTiming(cell.dynamicBucket);
			}
		}
	}

	// Token: 0x06003AC3 RID: 15043 RVA: 0x0015BB24 File Offset: 0x00159D24
	private static bool FrustumCull(Vector4[] planes, Vector4 testSphere)
	{
		for (int i = 0; i < 6; i++)
		{
			if (planes[i].x * testSphere.x + planes[i].y * testSphere.y + planes[i].z * testSphere.z + planes[i].w < -testSphere.w)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06003AC4 RID: 15044 RVA: 0x0015BB94 File Offset: 0x00159D94
	private static int ProcessOccludees_Safe(OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SmartList bucket, Color32[] results, OcclusionCulling.SimpleList<int> changed, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucket.Size; i++)
		{
			OccludeeState occludeeState = bucket[i];
			if (occludeeState != null && occludeeState.slot < results.Length)
			{
				int slot = occludeeState.slot;
				OccludeeState.State state = states[slot];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[slot].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = (time < state.waitTime);
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						changed.Add(slot);
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[slot] = state;
				num += (int)state.isVisible;
			}
		}
		return num;
	}

	// Token: 0x06003AC5 RID: 15045 RVA: 0x0015BC80 File Offset: 0x00159E80
	private static int ProcessOccludees_Fast(OccludeeState.State[] states, int[] bucket, int bucketCount, Color32[] results, int resultCount, int[] changed, ref int changedCount, Vector4[] frustumPlanes, float time, uint frame)
	{
		int num = 0;
		for (int i = 0; i < bucketCount; i++)
		{
			int num2 = bucket[i];
			if (num2 >= 0 && num2 < resultCount && states[num2].active != 0)
			{
				OccludeeState.State state = states[num2];
				bool flag = OcclusionCulling.FrustumCull(frustumPlanes, state.sphereBounds);
				bool flag2 = results[num2].r > 0 && flag;
				if (flag2 || frame < state.waitFrame)
				{
					state.waitTime = time + state.minTimeVisible;
				}
				if (!flag2)
				{
					flag2 = (time < state.waitTime);
				}
				if (flag2 != state.isVisible > 0)
				{
					if (state.callback != 0)
					{
						int num3 = changedCount;
						changedCount = num3 + 1;
						changed[num3] = num2;
					}
					else
					{
						state.isVisible = (flag2 ? 1 : 0);
					}
				}
				states[num2] = state;
				num += (flag2 ? 0 : 1);
			}
		}
		return num;
	}

	// Token: 0x06003AC6 RID: 15046
	[DllImport("Renderer", EntryPoint = "CULL_ProcessOccludees")]
	private static extern int ProcessOccludees_Native(ref OccludeeState.State states, ref int bucket, int bucketCount, ref Color32 results, int resultCount, ref int changed, ref int changedCount, ref Vector4 frustumPlanes, float time, uint frame);

	// Token: 0x06003AC7 RID: 15047 RVA: 0x0015BD6C File Offset: 0x00159F6C
	private void ApplyVisibility_Safe(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.staticStates, cell.staticBucket, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticVisibilityChanged, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Safe(OcclusionCulling.dynamicStates, cell.dynamicBucket, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicVisibilityChanged, this.frustumPlanes, time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x06003AC8 RID: 15048 RVA: 0x0015BEB0 File Offset: 0x0015A0B0
	private void ApplyVisibility_Fast(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.staticStates.array, cell.staticBucket.Slots, cell.staticBucket.Size, OcclusionCulling.staticSet.resultData, OcclusionCulling.staticSet.resultData.Length, OcclusionCulling.staticVisibilityChanged.array, ref OcclusionCulling.staticVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Fast(OcclusionCulling.dynamicStates.array, cell.dynamicBucket.Slots, cell.dynamicBucket.Size, OcclusionCulling.dynamicSet.resultData, OcclusionCulling.dynamicSet.resultData.Length, OcclusionCulling.dynamicVisibilityChanged.array, ref OcclusionCulling.dynamicVisibilityChanged.count, this.frustumPlanes, time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x06003AC9 RID: 15049 RVA: 0x0015C054 File Offset: 0x0015A254
	private void ApplyVisibility_Native(float time, uint frame)
	{
		bool ready = OcclusionCulling.staticSet.Ready;
		bool ready2 = OcclusionCulling.dynamicSet.Ready;
		for (int i = 0; i < OcclusionCulling.grid.Size; i++)
		{
			OcclusionCulling.Cell cell = OcclusionCulling.grid[i];
			if (cell != null && OcclusionCulling.gridSet.resultData.Length != 0)
			{
				bool flag = OcclusionCulling.FrustumCull(this.frustumPlanes, cell.sphereBounds);
				bool flag2 = OcclusionCulling.gridSet.resultData[i].r > 0 && flag;
				if (cell.isVisible || flag2)
				{
					int num = 0;
					int num2 = 0;
					if (ready && cell.staticBucket.Count > 0)
					{
						num = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.staticStates.array[0], ref cell.staticBucket.Slots[0], cell.staticBucket.Size, ref OcclusionCulling.staticSet.resultData[0], OcclusionCulling.staticSet.resultData.Length, ref OcclusionCulling.staticVisibilityChanged.array[0], ref OcclusionCulling.staticVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					if (ready2 && cell.dynamicBucket.Count > 0)
					{
						num2 = OcclusionCulling.ProcessOccludees_Native(ref OcclusionCulling.dynamicStates.array[0], ref cell.dynamicBucket.Slots[0], cell.dynamicBucket.Size, ref OcclusionCulling.dynamicSet.resultData[0], OcclusionCulling.dynamicSet.resultData.Length, ref OcclusionCulling.dynamicVisibilityChanged.array[0], ref OcclusionCulling.dynamicVisibilityChanged.count, ref this.frustumPlanes[0], time, frame);
					}
					cell.isVisible = (flag2 || num < cell.staticBucket.Count || num2 < cell.dynamicBucket.Count);
				}
			}
		}
	}

	// Token: 0x06003ACA RID: 15050 RVA: 0x0015C23C File Offset: 0x0015A43C
	private void ProcessCallbacks(OcclusionCulling.SimpleList<OccludeeState> occludees, OcclusionCulling.SimpleList<OccludeeState.State> states, OcclusionCulling.SimpleList<int> changed)
	{
		for (int i = 0; i < changed.Count; i++)
		{
			int num = changed[i];
			OccludeeState occludeeState = occludees[num];
			if (occludeeState != null)
			{
				bool flag = states.array[num].isVisible == 0;
				OcclusionCulling.OnVisibilityChanged onVisibilityChanged = occludeeState.onVisibilityChanged;
				if (onVisibilityChanged != null && (UnityEngine.Object)onVisibilityChanged.Target != null)
				{
					onVisibilityChanged(flag);
				}
				if (occludeeState.slot >= 0)
				{
					states.array[occludeeState.slot].isVisible = (flag ? 1 : 0);
				}
			}
		}
		changed.Clear();
	}

	// Token: 0x06003ACB RID: 15051 RVA: 0x0015C2DC File Offset: 0x0015A4DC
	public void RetrieveAndApplyVisibility()
	{
		if (OcclusionCulling.staticOccludees.Count > 0)
		{
			OcclusionCulling.staticSet.GetResults();
		}
		if (OcclusionCulling.dynamicOccludees.Count > 0)
		{
			OcclusionCulling.dynamicSet.GetResults();
		}
		if (OcclusionCulling.grid.Count > 0)
		{
			OcclusionCulling.gridSet.GetResults();
		}
		if (this.debugSettings.showAllVisible)
		{
			for (int i = 0; i < OcclusionCulling.staticSet.resultData.Length; i++)
			{
				OcclusionCulling.staticSet.resultData[i].r = 1;
			}
			for (int j = 0; j < OcclusionCulling.dynamicSet.resultData.Length; j++)
			{
				OcclusionCulling.dynamicSet.resultData[j].r = 1;
			}
			for (int k = 0; k < OcclusionCulling.gridSet.resultData.Length; k++)
			{
				OcclusionCulling.gridSet.resultData[k].r = 1;
			}
		}
		OcclusionCulling.staticVisibilityChanged.EnsureCapacity(OcclusionCulling.staticOccludees.Count);
		OcclusionCulling.dynamicVisibilityChanged.EnsureCapacity(OcclusionCulling.dynamicOccludees.Count);
		float time = Time.time;
		uint frameCount = (uint)Time.frameCount;
		if (this.useNativePath)
		{
			this.ApplyVisibility_Native(time, frameCount);
		}
		else
		{
			this.ApplyVisibility_Fast(time, frameCount);
		}
		this.ProcessCallbacks(OcclusionCulling.staticOccludees, OcclusionCulling.staticStates, OcclusionCulling.staticVisibilityChanged);
		this.ProcessCallbacks(OcclusionCulling.dynamicOccludees, OcclusionCulling.dynamicStates, OcclusionCulling.dynamicVisibilityChanged);
	}

	// Token: 0x02000ED4 RID: 3796
	public class BufferSet
	{
		// Token: 0x04004D2B RID: 19755
		public ComputeBuffer inputBuffer;

		// Token: 0x04004D2C RID: 19756
		public ComputeBuffer resultBuffer;

		// Token: 0x04004D2D RID: 19757
		public int width;

		// Token: 0x04004D2E RID: 19758
		public int height;

		// Token: 0x04004D2F RID: 19759
		public int capacity;

		// Token: 0x04004D30 RID: 19760
		public int count;

		// Token: 0x04004D31 RID: 19761
		public Texture2D inputTexture;

		// Token: 0x04004D32 RID: 19762
		public RenderTexture resultTexture;

		// Token: 0x04004D33 RID: 19763
		public Texture2D resultReadTexture;

		// Token: 0x04004D34 RID: 19764
		public Color[] inputData = new Color[0];

		// Token: 0x04004D35 RID: 19765
		public Color32[] resultData = new Color32[0];

		// Token: 0x04004D36 RID: 19766
		private OcclusionCulling culling;

		// Token: 0x04004D37 RID: 19767
		private const int MaxAsyncGPUReadbackRequests = 10;

		// Token: 0x04004D38 RID: 19768
		private Queue<AsyncGPUReadbackRequest> asyncRequests = new Queue<AsyncGPUReadbackRequest>();

		// Token: 0x04004D39 RID: 19769
		public IntPtr readbackInst = IntPtr.Zero;

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x0600536E RID: 21358 RVA: 0x001B2703 File Offset: 0x001B0903
		public bool Ready
		{
			get
			{
				return this.resultData.Length != 0;
			}
		}

		// Token: 0x0600536F RID: 21359 RVA: 0x001B270F File Offset: 0x001B090F
		public void Attach(OcclusionCulling culling)
		{
			this.culling = culling;
		}

		// Token: 0x06005370 RID: 21360 RVA: 0x001B2718 File Offset: 0x001B0918
		public void Dispose(bool data = true)
		{
			if (this.inputBuffer != null)
			{
				this.inputBuffer.Dispose();
				this.inputBuffer = null;
			}
			if (this.resultBuffer != null)
			{
				this.resultBuffer.Dispose();
				this.resultBuffer = null;
			}
			if (this.inputTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.inputTexture);
				this.inputTexture = null;
			}
			if (this.resultTexture != null)
			{
				RenderTexture.active = null;
				this.resultTexture.Release();
				UnityEngine.Object.DestroyImmediate(this.resultTexture);
				this.resultTexture = null;
			}
			if (this.resultReadTexture != null)
			{
				UnityEngine.Object.DestroyImmediate(this.resultReadTexture);
				this.resultReadTexture = null;
			}
			if (this.readbackInst != IntPtr.Zero)
			{
				RustNative.Graphics.BufferReadback.Destroy(this.readbackInst);
				this.readbackInst = IntPtr.Zero;
			}
			if (data)
			{
				this.inputData = new Color[0];
				this.resultData = new Color32[0];
				this.capacity = 0;
				this.count = 0;
			}
		}

		// Token: 0x06005371 RID: 21361 RVA: 0x001B281C File Offset: 0x001B0A1C
		public bool CheckResize(int count, int granularity)
		{
			if (count > this.capacity || (this.culling.usePixelShaderFallback && this.resultTexture != null && !this.resultTexture.IsCreated()))
			{
				this.Dispose(false);
				int num = this.capacity;
				int num2 = count / granularity * granularity + granularity;
				if (this.culling.usePixelShaderFallback)
				{
					this.width = Mathf.CeilToInt(Mathf.Sqrt((float)num2));
					this.height = Mathf.CeilToInt((float)num2 / (float)this.width);
					this.inputTexture = new Texture2D(this.width, this.height, TextureFormat.RGBAFloat, false, true);
					this.inputTexture.name = "_Input";
					this.inputTexture.filterMode = FilterMode.Point;
					this.inputTexture.wrapMode = TextureWrapMode.Clamp;
					this.resultTexture = new RenderTexture(this.width, this.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
					this.resultTexture.name = "_Result";
					this.resultTexture.filterMode = FilterMode.Point;
					this.resultTexture.wrapMode = TextureWrapMode.Clamp;
					this.resultTexture.useMipMap = false;
					this.resultTexture.Create();
					this.resultReadTexture = new Texture2D(this.width, this.height, TextureFormat.ARGB32, false, true);
					this.resultReadTexture.name = "_ResultRead";
					this.resultReadTexture.filterMode = FilterMode.Point;
					this.resultReadTexture.wrapMode = TextureWrapMode.Clamp;
					if (!this.culling.useAsyncReadAPI)
					{
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForTexture(this.resultTexture.GetNativeTexturePtr(), (uint)this.width, (uint)this.height, (uint)this.resultTexture.format);
					}
					this.capacity = this.width * this.height;
				}
				else
				{
					this.inputBuffer = new ComputeBuffer(num2, 16);
					this.resultBuffer = new ComputeBuffer(num2, 4);
					if (!this.culling.useAsyncReadAPI)
					{
						uint size = (uint)(this.capacity * 4);
						this.readbackInst = RustNative.Graphics.BufferReadback.CreateForBuffer(this.resultBuffer.GetNativeBufferPtr(), size);
					}
					this.capacity = num2;
				}
				Array.Resize<Color>(ref this.inputData, this.capacity);
				Array.Resize<Color32>(ref this.resultData, this.capacity);
				Color32 color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				for (int i = num; i < this.capacity; i++)
				{
					this.resultData[i] = color;
				}
				this.count = count;
				return true;
			}
			return false;
		}

		// Token: 0x06005372 RID: 21362 RVA: 0x001B2A90 File Offset: 0x001B0C90
		public void UploadData()
		{
			if (this.culling.usePixelShaderFallback)
			{
				this.inputTexture.SetPixels(this.inputData);
				this.inputTexture.Apply();
				return;
			}
			this.inputBuffer.SetData(this.inputData);
		}

		// Token: 0x06005373 RID: 21363 RVA: 0x001B2ACD File Offset: 0x001B0CCD
		private int AlignDispatchSize(int dispatchSize)
		{
			return (dispatchSize + 63) / 64;
		}

		// Token: 0x06005374 RID: 21364 RVA: 0x001B2AD8 File Offset: 0x001B0CD8
		public void Dispatch(int count)
		{
			if (this.culling.usePixelShaderFallback)
			{
				RenderBuffer activeColorBuffer = UnityEngine.Graphics.activeColorBuffer;
				RenderBuffer activeDepthBuffer = UnityEngine.Graphics.activeDepthBuffer;
				this.culling.fallbackMat.SetTexture("_Input", this.inputTexture);
				UnityEngine.Graphics.Blit(this.inputTexture, this.resultTexture, this.culling.fallbackMat, 0);
				UnityEngine.Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
				return;
			}
			if (this.inputBuffer != null)
			{
				this.culling.computeShader.SetBuffer(0, "_Input", this.inputBuffer);
				this.culling.computeShader.SetBuffer(0, "_Result", this.resultBuffer);
				this.culling.computeShader.Dispatch(0, this.AlignDispatchSize(count), 1, 1);
			}
		}

		// Token: 0x06005375 RID: 21365 RVA: 0x001B2B98 File Offset: 0x001B0D98
		public void IssueRead()
		{
			if (!OcclusionCulling.SafeMode)
			{
				if (this.culling.useAsyncReadAPI)
				{
					if (this.asyncRequests.Count < 10)
					{
						AsyncGPUReadbackRequest item;
						if (this.culling.usePixelShaderFallback)
						{
							item = AsyncGPUReadback.Request(this.resultTexture, 0, null);
						}
						else
						{
							item = AsyncGPUReadback.Request(this.resultBuffer, null);
						}
						this.asyncRequests.Enqueue(item);
						return;
					}
				}
				else if (this.readbackInst != IntPtr.Zero)
				{
					RustNative.Graphics.BufferReadback.IssueRead(this.readbackInst);
				}
			}
		}

		// Token: 0x06005376 RID: 21366 RVA: 0x001B2C1C File Offset: 0x001B0E1C
		public void GetResults()
		{
			if (this.resultData != null && this.resultData.Length != 0)
			{
				if (!OcclusionCulling.SafeMode)
				{
					if (this.culling.useAsyncReadAPI)
					{
						while (this.asyncRequests.Count > 0)
						{
							AsyncGPUReadbackRequest asyncGPUReadbackRequest = this.asyncRequests.Peek();
							if (asyncGPUReadbackRequest.hasError)
							{
								this.asyncRequests.Dequeue();
							}
							else
							{
								if (!asyncGPUReadbackRequest.done)
								{
									return;
								}
								NativeArray<Color32> data = asyncGPUReadbackRequest.GetData<Color32>(0);
								for (int i = 0; i < data.Length; i++)
								{
									this.resultData[i] = data[i];
								}
								this.asyncRequests.Dequeue();
							}
						}
						return;
					}
					if (this.readbackInst != IntPtr.Zero)
					{
						RustNative.Graphics.BufferReadback.GetData(this.readbackInst, ref this.resultData[0]);
						return;
					}
				}
				else
				{
					if (this.culling.usePixelShaderFallback)
					{
						RenderTexture.active = this.resultTexture;
						this.resultReadTexture.ReadPixels(new Rect(0f, 0f, (float)this.width, (float)this.height), 0, 0);
						this.resultReadTexture.Apply();
						Array.Copy(this.resultReadTexture.GetPixels32(), this.resultData, this.resultData.Length);
						return;
					}
					this.resultBuffer.GetData(this.resultData);
				}
			}
		}
	}

	// Token: 0x02000ED5 RID: 3797
	public enum DebugFilter
	{
		// Token: 0x04004D3B RID: 19771
		Off,
		// Token: 0x04004D3C RID: 19772
		Dynamic,
		// Token: 0x04004D3D RID: 19773
		Static,
		// Token: 0x04004D3E RID: 19774
		Grid,
		// Token: 0x04004D3F RID: 19775
		All
	}

	// Token: 0x02000ED6 RID: 3798
	[Flags]
	public enum DebugMask
	{
		// Token: 0x04004D41 RID: 19777
		Off = 0,
		// Token: 0x04004D42 RID: 19778
		Dynamic = 1,
		// Token: 0x04004D43 RID: 19779
		Static = 2,
		// Token: 0x04004D44 RID: 19780
		Grid = 4,
		// Token: 0x04004D45 RID: 19781
		All = 7
	}

	// Token: 0x02000ED7 RID: 3799
	[Serializable]
	public class DebugSettings
	{
		// Token: 0x04004D46 RID: 19782
		public bool log;

		// Token: 0x04004D47 RID: 19783
		public bool showAllVisible;

		// Token: 0x04004D48 RID: 19784
		public bool showMipChain;

		// Token: 0x04004D49 RID: 19785
		public bool showMain;

		// Token: 0x04004D4A RID: 19786
		public int showMainLod;

		// Token: 0x04004D4B RID: 19787
		public bool showFallback;

		// Token: 0x04004D4C RID: 19788
		public bool showStats;

		// Token: 0x04004D4D RID: 19789
		public bool showScreenBounds;

		// Token: 0x04004D4E RID: 19790
		public OcclusionCulling.DebugMask showMask;

		// Token: 0x04004D4F RID: 19791
		public LayerMask layerFilter = -1;
	}

	// Token: 0x02000ED8 RID: 3800
	public class HashedPoolValue
	{
		// Token: 0x04004D50 RID: 19792
		public ulong hashedPoolKey = ulong.MaxValue;

		// Token: 0x04004D51 RID: 19793
		public int hashedPoolIndex = -1;
	}

	// Token: 0x02000ED9 RID: 3801
	public class HashedPool<ValueType> where ValueType : OcclusionCulling.HashedPoolValue, new()
	{
		// Token: 0x04004D52 RID: 19794
		private int granularity;

		// Token: 0x04004D53 RID: 19795
		private Dictionary<ulong, ValueType> dict;

		// Token: 0x04004D54 RID: 19796
		private List<ValueType> pool;

		// Token: 0x04004D55 RID: 19797
		private List<ValueType> list;

		// Token: 0x04004D56 RID: 19798
		private Queue<ValueType> recycled;

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x0600537A RID: 21370 RVA: 0x001B2DDD File Offset: 0x001B0FDD
		public int Size
		{
			get
			{
				return this.list.Count;
			}
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x0600537B RID: 21371 RVA: 0x001B2DEA File Offset: 0x001B0FEA
		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		// Token: 0x1700070E RID: 1806
		public ValueType this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x0600537E RID: 21374 RVA: 0x001B2E14 File Offset: 0x001B1014
		public HashedPool(int capacity, int granularity)
		{
			this.granularity = granularity;
			this.dict = new Dictionary<ulong, ValueType>(capacity);
			this.pool = new List<ValueType>(capacity);
			this.list = new List<ValueType>(capacity);
			this.recycled = new Queue<ValueType>();
		}

		// Token: 0x0600537F RID: 21375 RVA: 0x001B2E52 File Offset: 0x001B1052
		public void Clear()
		{
			this.dict.Clear();
			this.pool.Clear();
			this.list.Clear();
			this.recycled.Clear();
		}

		// Token: 0x06005380 RID: 21376 RVA: 0x001B2E80 File Offset: 0x001B1080
		public ValueType Add(ulong key, int capacityGranularity = 16)
		{
			ValueType valueType;
			if (this.recycled.Count > 0)
			{
				valueType = this.recycled.Dequeue();
				this.list[valueType.hashedPoolIndex] = valueType;
			}
			else
			{
				int count = this.pool.Count;
				if (count == this.pool.Capacity)
				{
					this.pool.Capacity += this.granularity;
				}
				valueType = Activator.CreateInstance<ValueType>();
				valueType.hashedPoolIndex = count;
				this.pool.Add(valueType);
				this.list.Add(valueType);
			}
			valueType.hashedPoolKey = key;
			this.dict.Add(key, valueType);
			return valueType;
		}

		// Token: 0x06005381 RID: 21377 RVA: 0x001B2F38 File Offset: 0x001B1138
		public void Remove(ValueType value)
		{
			this.dict.Remove(value.hashedPoolKey);
			this.list[value.hashedPoolIndex] = default(ValueType);
			this.recycled.Enqueue(value);
			value.hashedPoolKey = ulong.MaxValue;
		}

		// Token: 0x06005382 RID: 21378 RVA: 0x001B2F94 File Offset: 0x001B1194
		public bool TryGetValue(ulong key, out ValueType value)
		{
			return this.dict.TryGetValue(key, out value);
		}

		// Token: 0x06005383 RID: 21379 RVA: 0x001B2FA3 File Offset: 0x001B11A3
		public bool ContainsKey(ulong key)
		{
			return this.dict.ContainsKey(key);
		}
	}

	// Token: 0x02000EDA RID: 3802
	public class SimpleList<T>
	{
		// Token: 0x04004D57 RID: 19799
		private const int defaultCapacity = 16;

		// Token: 0x04004D58 RID: 19800
		private static readonly T[] emptyArray = new T[0];

		// Token: 0x04004D59 RID: 19801
		public T[] array;

		// Token: 0x04004D5A RID: 19802
		public int count;

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06005384 RID: 21380 RVA: 0x001B2FB1 File Offset: 0x001B11B1
		public int Count
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06005385 RID: 21381 RVA: 0x001B2FB9 File Offset: 0x001B11B9
		// (set) Token: 0x06005386 RID: 21382 RVA: 0x001B2FC4 File Offset: 0x001B11C4
		public int Capacity
		{
			get
			{
				return this.array.Length;
			}
			set
			{
				if (value != this.array.Length)
				{
					if (value > 0)
					{
						T[] destinationArray = new T[value];
						if (this.count > 0)
						{
							Array.Copy(this.array, 0, destinationArray, 0, this.count);
						}
						this.array = destinationArray;
						return;
					}
					this.array = OcclusionCulling.SimpleList<T>.emptyArray;
				}
			}
		}

		// Token: 0x17000711 RID: 1809
		public T this[int index]
		{
			get
			{
				return this.array[index];
			}
			set
			{
				this.array[index] = value;
			}
		}

		// Token: 0x06005389 RID: 21385 RVA: 0x001B3034 File Offset: 0x001B1234
		public SimpleList()
		{
			this.array = OcclusionCulling.SimpleList<T>.emptyArray;
		}

		// Token: 0x0600538A RID: 21386 RVA: 0x001B3047 File Offset: 0x001B1247
		public SimpleList(int capacity)
		{
			this.array = ((capacity == 0) ? OcclusionCulling.SimpleList<T>.emptyArray : new T[capacity]);
		}

		// Token: 0x0600538B RID: 21387 RVA: 0x001B3068 File Offset: 0x001B1268
		public void Add(T item)
		{
			if (this.count == this.array.Length)
			{
				this.EnsureCapacity(this.count + 1);
			}
			T[] array = this.array;
			int num = this.count;
			this.count = num + 1;
			array[num] = item;
		}

		// Token: 0x0600538C RID: 21388 RVA: 0x001B30B0 File Offset: 0x001B12B0
		public void Clear()
		{
			if (this.count > 0)
			{
				Array.Clear(this.array, 0, this.count);
				this.count = 0;
			}
		}

		// Token: 0x0600538D RID: 21389 RVA: 0x001B30D4 File Offset: 0x001B12D4
		public bool Contains(T item)
		{
			for (int i = 0; i < this.count; i++)
			{
				if (this.array[i].Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600538E RID: 21390 RVA: 0x001B3116 File Offset: 0x001B1316
		public void CopyTo(T[] array)
		{
			Array.Copy(this.array, 0, array, 0, this.count);
		}

		// Token: 0x0600538F RID: 21391 RVA: 0x001B312C File Offset: 0x001B132C
		public void EnsureCapacity(int min)
		{
			if (this.array.Length < min)
			{
				int num = (this.array.Length == 0) ? 16 : (this.array.Length * 2);
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}
	}

	// Token: 0x02000EDB RID: 3803
	public class SmartListValue
	{
		// Token: 0x04004D5B RID: 19803
		public int hashedListIndex = -1;
	}

	// Token: 0x02000EDC RID: 3804
	public class SmartList
	{
		// Token: 0x04004D5C RID: 19804
		private const int defaultCapacity = 16;

		// Token: 0x04004D5D RID: 19805
		private static readonly OccludeeState[] emptyList = new OccludeeState[0];

		// Token: 0x04004D5E RID: 19806
		private static readonly int[] emptySlots = new int[0];

		// Token: 0x04004D5F RID: 19807
		private OccludeeState[] list;

		// Token: 0x04004D60 RID: 19808
		private int[] slots;

		// Token: 0x04004D61 RID: 19809
		private Queue<int> recycled;

		// Token: 0x04004D62 RID: 19810
		private int count;

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06005392 RID: 21394 RVA: 0x001B3188 File Offset: 0x001B1388
		public OccludeeState[] List
		{
			get
			{
				return this.list;
			}
		}

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x06005393 RID: 21395 RVA: 0x001B3190 File Offset: 0x001B1390
		public int[] Slots
		{
			get
			{
				return this.slots;
			}
		}

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06005394 RID: 21396 RVA: 0x001B3198 File Offset: 0x001B1398
		public int Size
		{
			get
			{
				return this.count;
			}
		}

		// Token: 0x17000715 RID: 1813
		// (get) Token: 0x06005395 RID: 21397 RVA: 0x001B31A0 File Offset: 0x001B13A0
		public int Count
		{
			get
			{
				return this.count - this.recycled.Count;
			}
		}

		// Token: 0x17000716 RID: 1814
		public OccludeeState this[int i]
		{
			get
			{
				return this.list[i];
			}
			set
			{
				this.list[i] = value;
			}
		}

		// Token: 0x17000717 RID: 1815
		// (get) Token: 0x06005398 RID: 21400 RVA: 0x001B31C9 File Offset: 0x001B13C9
		// (set) Token: 0x06005399 RID: 21401 RVA: 0x001B31D4 File Offset: 0x001B13D4
		public int Capacity
		{
			get
			{
				return this.list.Length;
			}
			set
			{
				if (value != this.list.Length)
				{
					if (value > 0)
					{
						OccludeeState[] destinationArray = new OccludeeState[value];
						int[] destinationArray2 = new int[value];
						if (this.count > 0)
						{
							Array.Copy(this.list, destinationArray, this.count);
							Array.Copy(this.slots, destinationArray2, this.count);
						}
						this.list = destinationArray;
						this.slots = destinationArray2;
						return;
					}
					this.list = OcclusionCulling.SmartList.emptyList;
					this.slots = OcclusionCulling.SmartList.emptySlots;
				}
			}
		}

		// Token: 0x0600539A RID: 21402 RVA: 0x001B3250 File Offset: 0x001B1450
		public SmartList(int capacity)
		{
			this.list = new OccludeeState[capacity];
			this.slots = new int[capacity];
			this.recycled = new Queue<int>();
			this.count = 0;
		}

		// Token: 0x0600539B RID: 21403 RVA: 0x001B3284 File Offset: 0x001B1484
		public void Add(OccludeeState value, int capacityGranularity = 16)
		{
			int num;
			if (this.recycled.Count > 0)
			{
				num = this.recycled.Dequeue();
				this.list[num] = value;
				this.slots[num] = value.slot;
			}
			else
			{
				num = this.count;
				if (num == this.list.Length)
				{
					this.EnsureCapacity(this.count + 1);
				}
				this.list[num] = value;
				this.slots[num] = value.slot;
				this.count++;
			}
			value.hashedListIndex = num;
		}

		// Token: 0x0600539C RID: 21404 RVA: 0x001B3310 File Offset: 0x001B1510
		public void Remove(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			this.list[hashedListIndex] = null;
			this.slots[hashedListIndex] = -1;
			this.recycled.Enqueue(hashedListIndex);
			value.hashedListIndex = -1;
		}

		// Token: 0x0600539D RID: 21405 RVA: 0x001B334C File Offset: 0x001B154C
		public bool Contains(OccludeeState value)
		{
			int hashedListIndex = value.hashedListIndex;
			return hashedListIndex >= 0 && this.list[hashedListIndex] != null;
		}

		// Token: 0x0600539E RID: 21406 RVA: 0x001B3374 File Offset: 0x001B1574
		public void EnsureCapacity(int min)
		{
			if (this.list.Length < min)
			{
				int num = (this.list.Length == 0) ? 16 : (this.list.Length * 2);
				num = ((num < min) ? min : num);
				this.Capacity = num;
			}
		}
	}

	// Token: 0x02000EDD RID: 3805
	[Serializable]
	public class Cell : OcclusionCulling.HashedPoolValue
	{
		// Token: 0x04004D63 RID: 19811
		public int x;

		// Token: 0x04004D64 RID: 19812
		public int y;

		// Token: 0x04004D65 RID: 19813
		public int z;

		// Token: 0x04004D66 RID: 19814
		public Bounds bounds;

		// Token: 0x04004D67 RID: 19815
		public Vector4 sphereBounds;

		// Token: 0x04004D68 RID: 19816
		public bool isVisible;

		// Token: 0x04004D69 RID: 19817
		public OcclusionCulling.SmartList staticBucket;

		// Token: 0x04004D6A RID: 19818
		public OcclusionCulling.SmartList dynamicBucket;

		// Token: 0x060053A0 RID: 21408 RVA: 0x001B33CC File Offset: 0x001B15CC
		public void Reset()
		{
			this.x = (this.y = (this.z = 0));
			this.bounds = default(Bounds);
			this.sphereBounds = Vector4.zero;
			this.isVisible = true;
			this.staticBucket = null;
			this.dynamicBucket = null;
		}

		// Token: 0x060053A1 RID: 21409 RVA: 0x001B3420 File Offset: 0x001B1620
		public OcclusionCulling.Cell Initialize(int x, int y, int z, Bounds bounds)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.bounds = bounds;
			this.sphereBounds = new Vector4(bounds.center.x, bounds.center.y, bounds.center.z, bounds.extents.magnitude);
			this.isVisible = true;
			this.staticBucket = new OcclusionCulling.SmartList(32);
			this.dynamicBucket = new OcclusionCulling.SmartList(32);
			return this;
		}
	}

	// Token: 0x02000EDE RID: 3806
	public struct Sphere
	{
		// Token: 0x04004D6B RID: 19819
		public Vector3 position;

		// Token: 0x04004D6C RID: 19820
		public float radius;

		// Token: 0x060053A3 RID: 21411 RVA: 0x001B34B2 File Offset: 0x001B16B2
		public bool IsValid()
		{
			return this.radius > 0f;
		}

		// Token: 0x060053A4 RID: 21412 RVA: 0x001B34C1 File Offset: 0x001B16C1
		public Sphere(Vector3 position, float radius)
		{
			this.position = position;
			this.radius = radius;
		}
	}

	// Token: 0x02000EDF RID: 3807
	// (Invoke) Token: 0x060053A6 RID: 21414
	public delegate void OnVisibilityChanged(bool visible);
}
