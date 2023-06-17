using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9C RID: 2716
	public static class RuntimeUtilities
	{
		// Token: 0x040039AB RID: 14763
		private static Texture2D m_WhiteTexture;

		// Token: 0x040039AC RID: 14764
		private static Texture3D m_WhiteTexture3D;

		// Token: 0x040039AD RID: 14765
		private static Texture2D m_BlackTexture;

		// Token: 0x040039AE RID: 14766
		private static Texture3D m_BlackTexture3D;

		// Token: 0x040039AF RID: 14767
		private static Texture2D m_TransparentTexture;

		// Token: 0x040039B0 RID: 14768
		private static Texture3D m_TransparentTexture3D;

		// Token: 0x040039B1 RID: 14769
		private static Dictionary<int, Texture2D> m_LutStrips = new Dictionary<int, Texture2D>();

		// Token: 0x040039B2 RID: 14770
		internal static PostProcessResources s_Resources;

		// Token: 0x040039B3 RID: 14771
		private static Mesh s_FullscreenTriangle;

		// Token: 0x040039B4 RID: 14772
		private static Material s_CopyStdMaterial;

		// Token: 0x040039B5 RID: 14773
		private static Material s_CopyStdFromDoubleWideMaterial;

		// Token: 0x040039B6 RID: 14774
		private static Material s_CopyMaterial;

		// Token: 0x040039B7 RID: 14775
		private static Material s_CopyFromTexArrayMaterial;

		// Token: 0x040039B8 RID: 14776
		private static PropertySheet s_CopySheet;

		// Token: 0x040039B9 RID: 14777
		private static PropertySheet s_CopyFromTexArraySheet;

		// Token: 0x040039BA RID: 14778
		private static IEnumerable<Type> m_AssemblyTypes;

		// Token: 0x17000589 RID: 1417
		// (get) Token: 0x060040B6 RID: 16566 RVA: 0x0017D6D4 File Offset: 0x0017B8D4
		public static Texture2D whiteTexture
		{
			get
			{
				if (RuntimeUtilities.m_WhiteTexture == null)
				{
					RuntimeUtilities.m_WhiteTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "White Texture"
					};
					RuntimeUtilities.m_WhiteTexture.SetPixel(0, 0, Color.white);
					RuntimeUtilities.m_WhiteTexture.Apply();
				}
				return RuntimeUtilities.m_WhiteTexture;
			}
		}

		// Token: 0x1700058A RID: 1418
		// (get) Token: 0x060040B7 RID: 16567 RVA: 0x0017D728 File Offset: 0x0017B928
		public static Texture3D whiteTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_WhiteTexture3D == null)
				{
					RuntimeUtilities.m_WhiteTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "White Texture 3D"
					};
					RuntimeUtilities.m_WhiteTexture3D.SetPixels(new Color[]
					{
						Color.white
					});
					RuntimeUtilities.m_WhiteTexture3D.Apply();
				}
				return RuntimeUtilities.m_WhiteTexture3D;
			}
		}

		// Token: 0x1700058B RID: 1419
		// (get) Token: 0x060040B8 RID: 16568 RVA: 0x0017D788 File Offset: 0x0017B988
		public static Texture2D blackTexture
		{
			get
			{
				if (RuntimeUtilities.m_BlackTexture == null)
				{
					RuntimeUtilities.m_BlackTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "Black Texture"
					};
					RuntimeUtilities.m_BlackTexture.SetPixel(0, 0, Color.black);
					RuntimeUtilities.m_BlackTexture.Apply();
				}
				return RuntimeUtilities.m_BlackTexture;
			}
		}

		// Token: 0x1700058C RID: 1420
		// (get) Token: 0x060040B9 RID: 16569 RVA: 0x0017D7DC File Offset: 0x0017B9DC
		public static Texture3D blackTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_BlackTexture3D == null)
				{
					RuntimeUtilities.m_BlackTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "Black Texture 3D"
					};
					RuntimeUtilities.m_BlackTexture3D.SetPixels(new Color[]
					{
						Color.black
					});
					RuntimeUtilities.m_BlackTexture3D.Apply();
				}
				return RuntimeUtilities.m_BlackTexture3D;
			}
		}

		// Token: 0x1700058D RID: 1421
		// (get) Token: 0x060040BA RID: 16570 RVA: 0x0017D83C File Offset: 0x0017BA3C
		public static Texture2D transparentTexture
		{
			get
			{
				if (RuntimeUtilities.m_TransparentTexture == null)
				{
					RuntimeUtilities.m_TransparentTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false)
					{
						name = "Transparent Texture"
					};
					RuntimeUtilities.m_TransparentTexture.SetPixel(0, 0, Color.clear);
					RuntimeUtilities.m_TransparentTexture.Apply();
				}
				return RuntimeUtilities.m_TransparentTexture;
			}
		}

		// Token: 0x1700058E RID: 1422
		// (get) Token: 0x060040BB RID: 16571 RVA: 0x0017D890 File Offset: 0x0017BA90
		public static Texture3D transparentTexture3D
		{
			get
			{
				if (RuntimeUtilities.m_TransparentTexture3D == null)
				{
					RuntimeUtilities.m_TransparentTexture3D = new Texture3D(1, 1, 1, TextureFormat.ARGB32, false)
					{
						name = "Transparent Texture 3D"
					};
					RuntimeUtilities.m_TransparentTexture3D.SetPixels(new Color[]
					{
						Color.clear
					});
					RuntimeUtilities.m_TransparentTexture3D.Apply();
				}
				return RuntimeUtilities.m_TransparentTexture3D;
			}
		}

		// Token: 0x060040BC RID: 16572 RVA: 0x0017D8F0 File Offset: 0x0017BAF0
		public static Texture2D GetLutStrip(int size)
		{
			Texture2D texture2D;
			if (!RuntimeUtilities.m_LutStrips.TryGetValue(size, out texture2D))
			{
				int num = size * size;
				Color[] array = new Color[num * size];
				float num2 = 1f / ((float)size - 1f);
				for (int i = 0; i < size; i++)
				{
					int num3 = i * size;
					float b = (float)i * num2;
					for (int j = 0; j < size; j++)
					{
						float g = (float)j * num2;
						for (int k = 0; k < size; k++)
						{
							float r = (float)k * num2;
							array[j * num + num3 + k] = new Color(r, g, b);
						}
					}
				}
				TextureFormat textureFormat = TextureFormat.RGBAHalf;
				if (!textureFormat.IsSupported())
				{
					textureFormat = TextureFormat.ARGB32;
				}
				texture2D = new Texture2D(size * size, size, textureFormat, false, true)
				{
					name = "Strip Lut" + size,
					hideFlags = HideFlags.DontSave,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					anisoLevel = 0
				};
				texture2D.SetPixels(array);
				texture2D.Apply();
				RuntimeUtilities.m_LutStrips.Add(size, texture2D);
			}
			return texture2D;
		}

		// Token: 0x1700058F RID: 1423
		// (get) Token: 0x060040BD RID: 16573 RVA: 0x0017DA04 File Offset: 0x0017BC04
		public static Mesh fullscreenTriangle
		{
			get
			{
				if (RuntimeUtilities.s_FullscreenTriangle != null)
				{
					return RuntimeUtilities.s_FullscreenTriangle;
				}
				RuntimeUtilities.s_FullscreenTriangle = new Mesh
				{
					name = "Fullscreen Triangle"
				};
				RuntimeUtilities.s_FullscreenTriangle.SetVertices(new List<Vector3>
				{
					new Vector3(-1f, -1f, 0f),
					new Vector3(-1f, 3f, 0f),
					new Vector3(3f, -1f, 0f)
				});
				RuntimeUtilities.s_FullscreenTriangle.SetIndices(new int[]
				{
					0,
					1,
					2
				}, MeshTopology.Triangles, 0, false);
				RuntimeUtilities.s_FullscreenTriangle.UploadMeshData(false);
				return RuntimeUtilities.s_FullscreenTriangle;
			}
		}

		// Token: 0x17000590 RID: 1424
		// (get) Token: 0x060040BE RID: 16574 RVA: 0x0017DAC4 File Offset: 0x0017BCC4
		public static Material copyStdMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyStdMaterial != null)
				{
					return RuntimeUtilities.s_CopyStdMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyStdMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStd)
				{
					name = "PostProcess - CopyStd",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyStdMaterial;
			}
		}

		// Token: 0x17000591 RID: 1425
		// (get) Token: 0x060040BF RID: 16575 RVA: 0x0017DB20 File Offset: 0x0017BD20
		public static Material copyStdFromDoubleWideMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyStdFromDoubleWideMaterial != null)
				{
					return RuntimeUtilities.s_CopyStdFromDoubleWideMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyStdFromDoubleWideMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStdFromDoubleWide)
				{
					name = "PostProcess - CopyStdFromDoubleWide",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyStdFromDoubleWideMaterial;
			}
		}

		// Token: 0x17000592 RID: 1426
		// (get) Token: 0x060040C0 RID: 16576 RVA: 0x0017DB7C File Offset: 0x0017BD7C
		public static Material copyMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyMaterial != null)
				{
					return RuntimeUtilities.s_CopyMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copy)
				{
					name = "PostProcess - Copy",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyMaterial;
			}
		}

		// Token: 0x17000593 RID: 1427
		// (get) Token: 0x060040C1 RID: 16577 RVA: 0x0017DBD8 File Offset: 0x0017BDD8
		public static Material copyFromTexArrayMaterial
		{
			get
			{
				if (RuntimeUtilities.s_CopyFromTexArrayMaterial != null)
				{
					return RuntimeUtilities.s_CopyFromTexArrayMaterial;
				}
				Assert.IsNotNull<PostProcessResources>(RuntimeUtilities.s_Resources);
				RuntimeUtilities.s_CopyFromTexArrayMaterial = new Material(RuntimeUtilities.s_Resources.shaders.copyStdFromTexArray)
				{
					name = "PostProcess - CopyFromTexArray",
					hideFlags = HideFlags.HideAndDontSave
				};
				return RuntimeUtilities.s_CopyFromTexArrayMaterial;
			}
		}

		// Token: 0x17000594 RID: 1428
		// (get) Token: 0x060040C2 RID: 16578 RVA: 0x0017DC33 File Offset: 0x0017BE33
		public static PropertySheet copySheet
		{
			get
			{
				if (RuntimeUtilities.s_CopySheet == null)
				{
					RuntimeUtilities.s_CopySheet = new PropertySheet(RuntimeUtilities.copyMaterial);
				}
				return RuntimeUtilities.s_CopySheet;
			}
		}

		// Token: 0x17000595 RID: 1429
		// (get) Token: 0x060040C3 RID: 16579 RVA: 0x0017DC50 File Offset: 0x0017BE50
		public static PropertySheet copyFromTexArraySheet
		{
			get
			{
				if (RuntimeUtilities.s_CopyFromTexArraySheet == null)
				{
					RuntimeUtilities.s_CopyFromTexArraySheet = new PropertySheet(RuntimeUtilities.copyFromTexArrayMaterial);
				}
				return RuntimeUtilities.s_CopyFromTexArraySheet;
			}
		}

		// Token: 0x060040C4 RID: 16580 RVA: 0x0017DC6D File Offset: 0x0017BE6D
		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
		{
			cmd.SetRenderTarget(rt, loadAction, storeAction);
		}

		// Token: 0x060040C5 RID: 16581 RVA: 0x0017DC78 File Offset: 0x0017BE78
		public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
		{
			cmd.SetRenderTarget(color, colorLoadAction, colorStoreAction, depth, depthLoadAction, depthStoreAction);
		}

		// Token: 0x060040C6 RID: 16582 RVA: 0x0017DC8C File Offset: 0x0017BE8C
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTargetWithLoadStoreAction(destination, (viewport == null) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, RuntimeUtilities.copyMaterial, 0, 0);
		}

		// Token: 0x060040C7 RID: 16583 RVA: 0x0017DCF8 File Offset: 0x0017BEF8
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, RenderBufferLoadAction loadAction, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			bool flag = loadAction == RenderBufferLoadAction.Clear;
			if (flag)
			{
				loadAction = RenderBufferLoadAction.DontCare;
			}
			cmd.SetRenderTargetWithLoadStoreAction(destination, (viewport == null) ? loadAction : RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (flag)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040C8 RID: 16584 RVA: 0x0017DD76 File Offset: 0x0017BF76
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.BlitFullscreenTriangle(source, destination, propertySheet, pass, clear ? RenderBufferLoadAction.Clear : RenderBufferLoadAction.DontCare, viewport);
		}

		// Token: 0x060040C9 RID: 16585 RVA: 0x0017DD90 File Offset: 0x0017BF90
		public static void BlitFullscreenTriangleFromDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass, int eye)
		{
			Vector4 value = new Vector4(0.5f, 1f, 0f, 0f);
			if (eye == 1)
			{
				value.z = 0.5f;
			}
			cmd.SetGlobalVector(ShaderIDs.UVScaleOffset, value);
			cmd.BuiltinBlit(source, destination, material, pass);
		}

		// Token: 0x060040CA RID: 16586 RVA: 0x0017DDE0 File Offset: 0x0017BFE0
		public static void BlitFullscreenTriangleToDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, int eye)
		{
			Vector4 value = new Vector4(0.5f, 1f, -0.5f, 0f);
			if (eye == 1)
			{
				value.z = 0.5f;
			}
			propertySheet.EnableKeyword("STEREO_DOUBLEWIDE_TARGET");
			propertySheet.properties.SetVector(ShaderIDs.PosScaleOffset, value);
			cmd.BlitFullscreenTriangle(source, destination, propertySheet, 0, false, null);
		}

		// Token: 0x060040CB RID: 16587 RVA: 0x0017DE4C File Offset: 0x0017C04C
		public static void BlitFullscreenTriangleFromTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
			cmd.SetRenderTargetWithLoadStoreAction(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040CC RID: 16588 RVA: 0x0017DEAC File Offset: 0x0017C0AC
		public static void BlitFullscreenTriangleToTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
			cmd.SetRenderTarget(destination, 0, CubemapFace.Unknown, -1);
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040CD RID: 16589 RVA: 0x0017DF10 File Offset: 0x0017C110
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			RenderBufferLoadAction renderBufferLoadAction = (viewport == null) ? RenderBufferLoadAction.DontCare : RenderBufferLoadAction.Load;
			if (clear)
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, renderBufferLoadAction, RenderBufferStoreAction.Store, depth, renderBufferLoadAction, RenderBufferStoreAction.Store);
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			else
			{
				cmd.SetRenderTargetWithLoadStoreAction(destination, renderBufferLoadAction, RenderBufferStoreAction.Store, depth, RenderBufferLoadAction.Load, RenderBufferStoreAction.Store);
			}
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040CE RID: 16590 RVA: 0x0017DF9C File Offset: 0x0017C19C
		public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
		{
			cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
			cmd.SetRenderTarget(destinations, depth);
			if (viewport != null)
			{
				cmd.SetViewport(viewport.Value);
			}
			if (clear)
			{
				cmd.ClearRenderTarget(true, true, Color.clear);
			}
			cmd.DrawMesh(RuntimeUtilities.fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
		}

		// Token: 0x060040CF RID: 16591 RVA: 0x0017E005 File Offset: 0x0017C205
		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			destination = BuiltinRenderTextureType.CurrentActive;
			cmd.Blit(source, destination);
		}

		// Token: 0x060040D0 RID: 16592 RVA: 0x0017E020 File Offset: 0x0017C220
		public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material mat, int pass = 0)
		{
			cmd.SetRenderTarget(destination, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
			destination = BuiltinRenderTextureType.CurrentActive;
			cmd.Blit(source, destination, mat, pass);
		}

		// Token: 0x060040D1 RID: 16593 RVA: 0x0017E040 File Offset: 0x0017C240
		public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
		{
			if (SystemInfo.copyTextureSupport > CopyTextureSupport.None)
			{
				cmd.CopyTexture(source, destination);
				return;
			}
			cmd.BlitFullscreenTriangle(source, destination, false, null);
		}

		// Token: 0x17000596 RID: 1430
		// (get) Token: 0x060040D2 RID: 16594 RVA: 0x0017E070 File Offset: 0x0017C270
		public static bool scriptableRenderPipelineActive
		{
			get
			{
				return GraphicsSettings.renderPipelineAsset != null;
			}
		}

		// Token: 0x17000597 RID: 1431
		// (get) Token: 0x060040D3 RID: 16595 RVA: 0x0017E07D File Offset: 0x0017C27D
		public static bool supportsDeferredShading
		{
			get
			{
				return RuntimeUtilities.scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DeferredShading) > BuiltinShaderMode.Disabled;
			}
		}

		// Token: 0x17000598 RID: 1432
		// (get) Token: 0x060040D4 RID: 16596 RVA: 0x0017E091 File Offset: 0x0017C291
		public static bool supportsDepthNormals
		{
			get
			{
				return RuntimeUtilities.scriptableRenderPipelineActive || GraphicsSettings.GetShaderMode(BuiltinShaderType.DepthNormals) > BuiltinShaderMode.Disabled;
			}
		}

		// Token: 0x17000599 RID: 1433
		// (get) Token: 0x060040D5 RID: 16597 RVA: 0x00007A3C File Offset: 0x00005C3C
		public static bool isSinglePassStereoEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700059A RID: 1434
		// (get) Token: 0x060040D6 RID: 16598 RVA: 0x00007A3C File Offset: 0x00005C3C
		public static bool isVREnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700059B RID: 1435
		// (get) Token: 0x060040D7 RID: 16599 RVA: 0x0017E0A5 File Offset: 0x0017C2A5
		public static bool isAndroidOpenGL
		{
			get
			{
				return Application.platform == RuntimePlatform.Android && SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan;
			}
		}

		// Token: 0x1700059C RID: 1436
		// (get) Token: 0x060040D8 RID: 16600 RVA: 0x00040DE9 File Offset: 0x0003EFE9
		public static RenderTextureFormat defaultHDRRenderTextureFormat
		{
			get
			{
				return RenderTextureFormat.DefaultHDR;
			}
		}

		// Token: 0x060040D9 RID: 16601 RVA: 0x0017E0BE File Offset: 0x0017C2BE
		public static bool isFloatingPointFormat(RenderTextureFormat format)
		{
			return format == RenderTextureFormat.DefaultHDR || format == RenderTextureFormat.ARGBHalf || format == RenderTextureFormat.ARGBFloat || format == RenderTextureFormat.RGFloat || format == RenderTextureFormat.RGHalf || format == RenderTextureFormat.RFloat || format == RenderTextureFormat.RHalf || format == RenderTextureFormat.RGB111110Float;
		}

		// Token: 0x060040DA RID: 16602 RVA: 0x0017E0E9 File Offset: 0x0017C2E9
		public static void Destroy(Object obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
		}

		// Token: 0x1700059D RID: 1437
		// (get) Token: 0x060040DB RID: 16603 RVA: 0x0017E0FA File Offset: 0x0017C2FA
		public static bool isLinearColorSpace
		{
			get
			{
				return QualitySettings.activeColorSpace == ColorSpace.Linear;
			}
		}

		// Token: 0x060040DC RID: 16604 RVA: 0x0017E104 File Offset: 0x0017C304
		public static bool IsResolvedDepthAvailable(Camera camera)
		{
			GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
			return camera.actualRenderingPath == RenderingPath.DeferredShading && (graphicsDeviceType == GraphicsDeviceType.Direct3D11 || graphicsDeviceType == GraphicsDeviceType.Direct3D12 || graphicsDeviceType == GraphicsDeviceType.XboxOne);
		}

		// Token: 0x060040DD RID: 16605 RVA: 0x0017E134 File Offset: 0x0017C334
		public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
		{
			if (destroyEffects)
			{
				foreach (PostProcessEffectSettings obj in profile.settings)
				{
					RuntimeUtilities.Destroy(obj);
				}
			}
			RuntimeUtilities.Destroy(profile);
		}

		// Token: 0x060040DE RID: 16606 RVA: 0x0017E190 File Offset: 0x0017C390
		public static void DestroyVolume(PostProcessVolume volume, bool destroyProfile, bool destroyGameObject = false)
		{
			if (destroyProfile)
			{
				RuntimeUtilities.DestroyProfile(volume.profileRef, true);
			}
			GameObject gameObject = volume.gameObject;
			RuntimeUtilities.Destroy(volume);
			if (destroyGameObject)
			{
				RuntimeUtilities.Destroy(gameObject);
			}
		}

		// Token: 0x060040DF RID: 16607 RVA: 0x0017E1C2 File Offset: 0x0017C3C2
		public static bool IsPostProcessingActive(PostProcessLayer layer)
		{
			return layer != null && layer.enabled;
		}

		// Token: 0x060040E0 RID: 16608 RVA: 0x0017E1D5 File Offset: 0x0017C3D5
		public static bool IsTemporalAntialiasingActive(PostProcessLayer layer)
		{
			return RuntimeUtilities.IsPostProcessingActive(layer) && layer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing && layer.temporalAntialiasing.IsSupported();
		}

		// Token: 0x060040E1 RID: 16609 RVA: 0x0017E1F5 File Offset: 0x0017C3F5
		public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
		{
			Queue<Transform> queue = new Queue<Transform>();
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (GameObject gameObject in rootGameObjects)
			{
				queue.Enqueue(gameObject.transform);
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					yield return component;
				}
			}
			GameObject[] array = null;
			while (queue.Count > 0)
			{
				foreach (object obj in queue.Dequeue())
				{
					Transform transform = (Transform)obj;
					queue.Enqueue(transform);
					T component2 = transform.GetComponent<T>();
					if (component2 != null)
					{
						yield return component2;
					}
				}
				IEnumerator enumerator = null;
			}
			yield break;
			yield break;
		}

		// Token: 0x060040E2 RID: 16610 RVA: 0x0017E1FE File Offset: 0x0017C3FE
		public static void CreateIfNull<T>(ref T obj) where T : class, new()
		{
			if (obj == null)
			{
				obj = Activator.CreateInstance<T>();
			}
		}

		// Token: 0x060040E3 RID: 16611 RVA: 0x0017E218 File Offset: 0x0017C418
		public static float Exp2(float x)
		{
			return Mathf.Exp(x * 0.6931472f);
		}

		// Token: 0x060040E4 RID: 16612 RVA: 0x0017E228 File Offset: 0x0017C428
		public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(Camera camera, Vector2 offset)
		{
			float nearClipPlane = camera.nearClipPlane;
			float farClipPlane = camera.farClipPlane;
			float num = Mathf.Tan(0.008726646f * camera.fieldOfView) * nearClipPlane;
			float num2 = num * camera.aspect;
			offset.x *= num2 / (0.5f * (float)camera.pixelWidth);
			offset.y *= num / (0.5f * (float)camera.pixelHeight);
			Matrix4x4 projectionMatrix = camera.projectionMatrix;
			ref Matrix4x4 ptr = ref projectionMatrix;
			ptr[0, 2] = ptr[0, 2] + offset.x / num2;
			ptr = ref projectionMatrix;
			ptr[1, 2] = ptr[1, 2] + offset.y / num;
			return projectionMatrix;
		}

		// Token: 0x060040E5 RID: 16613 RVA: 0x0017E2DC File Offset: 0x0017C4DC
		public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(Camera camera, Vector2 offset)
		{
			float orthographicSize = camera.orthographicSize;
			float num = orthographicSize * camera.aspect;
			offset.x *= num / (0.5f * (float)camera.pixelWidth);
			offset.y *= orthographicSize / (0.5f * (float)camera.pixelHeight);
			float left = offset.x - num;
			float right = offset.x + num;
			float top = offset.y + orthographicSize;
			float bottom = offset.y - orthographicSize;
			return Matrix4x4.Ortho(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
		}

		// Token: 0x060040E6 RID: 16614 RVA: 0x0017E368 File Offset: 0x0017C568
		public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(PostProcessRenderContext context, Matrix4x4 origProj, Vector2 jitter)
		{
			FrustumPlanes decomposeProjection = origProj.decomposeProjection;
			float num = Math.Abs(decomposeProjection.top) + Math.Abs(decomposeProjection.bottom);
			float num2 = Math.Abs(decomposeProjection.left) + Math.Abs(decomposeProjection.right);
			Vector2 vector = new Vector2(jitter.x * num2 / (float)context.screenWidth, jitter.y * num / (float)context.screenHeight);
			decomposeProjection.left += vector.x;
			decomposeProjection.right += vector.x;
			decomposeProjection.top += vector.y;
			decomposeProjection.bottom += vector.y;
			return Matrix4x4.Frustum(decomposeProjection);
		}

		// Token: 0x060040E7 RID: 16615 RVA: 0x0017E420 File Offset: 0x0017C620
		public static IEnumerable<Type> GetAllAssemblyTypes()
		{
			if (RuntimeUtilities.m_AssemblyTypes == null)
			{
				RuntimeUtilities.m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly t)
				{
					Type[] result = new Type[0];
					try
					{
						result = t.GetTypes();
					}
					catch
					{
					}
					return result;
				});
			}
			return RuntimeUtilities.m_AssemblyTypes;
		}

		// Token: 0x060040E8 RID: 16616 RVA: 0x0017E46C File Offset: 0x0017C66C
		public static T GetAttribute<T>(this Type type) where T : Attribute
		{
			Assert.IsTrue(type.IsDefined(typeof(T), false), "Attribute not found");
			return (T)((object)type.GetCustomAttributes(typeof(T), false)[0]);
		}

		// Token: 0x060040E9 RID: 16617 RVA: 0x0017E4A4 File Offset: 0x0017C6A4
		public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			Expression expression = expr;
			if (expression is LambdaExpression)
			{
				expression = ((LambdaExpression)expression).Body;
			}
			ExpressionType nodeType = expression.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				return ((FieldInfo)((MemberExpression)expression).Member).GetCustomAttributes(false).Cast<Attribute>().ToArray<Attribute>();
			}
			throw new InvalidOperationException();
		}

		// Token: 0x060040EA RID: 16618 RVA: 0x0017E4FC File Offset: 0x0017C6FC
		public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
		{
			ExpressionType nodeType = expr.Body.NodeType;
			if (nodeType == ExpressionType.MemberAccess)
			{
				MemberExpression memberExpression = expr.Body as MemberExpression;
				List<string> list = new List<string>();
				while (memberExpression != null)
				{
					list.Add(memberExpression.Member.Name);
					memberExpression = (memberExpression.Expression as MemberExpression);
				}
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = list.Count - 1; i >= 0; i--)
				{
					stringBuilder.Append(list[i]);
					if (i > 0)
					{
						stringBuilder.Append('.');
					}
				}
				return stringBuilder.ToString();
			}
			throw new InvalidOperationException();
		}
	}
}
