using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000A12 RID: 2578
	public static class ImageEffectHelper
	{
		// Token: 0x06003D5D RID: 15709 RVA: 0x00169060 File Offset: 0x00167260
		public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
		{
			if (s == null || !s.isSupported)
			{
				Debug.LogWarningFormat("Missing shader for image effect {0}", new object[]
				{
					effect
				});
				return false;
			}
			if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
			{
				Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", new object[]
				{
					effect
				});
				return false;
			}
			if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", new object[]
				{
					effect
				});
				return false;
			}
			if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
			{
				Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", new object[]
				{
					effect
				});
				return false;
			}
			return true;
		}

		// Token: 0x06003D5E RID: 15710 RVA: 0x001586A0 File Offset: 0x001568A0
		public static Material CheckShaderAndCreateMaterial(Shader s)
		{
			if (s == null || !s.isSupported)
			{
				return null;
			}
			return new Material(s)
			{
				hideFlags = HideFlags.DontSave
			};
		}

		// Token: 0x1700050A RID: 1290
		// (get) Token: 0x06003D5F RID: 15711 RVA: 0x001586C3 File Offset: 0x001568C3
		public static bool supportsDX11
		{
			get
			{
				return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;
			}
		}
	}
}
