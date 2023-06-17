using System;
using UnityEngine;

namespace VLB
{
	// Token: 0x020009B7 RID: 2487
	public static class Noise3D
	{
		// Token: 0x040035E6 RID: 13798
		private static bool ms_IsSupportedChecked;

		// Token: 0x040035E7 RID: 13799
		private static bool ms_IsSupported;

		// Token: 0x040035E8 RID: 13800
		private static Texture3D ms_NoiseTexture;

		// Token: 0x040035E9 RID: 13801
		private const HideFlags kHideFlags = HideFlags.HideAndDontSave;

		// Token: 0x040035EA RID: 13802
		private const int kMinShaderLevel = 35;

		// Token: 0x170004B6 RID: 1206
		// (get) Token: 0x06003B47 RID: 15175 RVA: 0x0015F8D0 File Offset: 0x0015DAD0
		public static bool isSupported
		{
			get
			{
				if (!Noise3D.ms_IsSupportedChecked)
				{
					Noise3D.ms_IsSupported = (SystemInfo.graphicsShaderLevel >= 35);
					if (!Noise3D.ms_IsSupported)
					{
						Debug.LogWarning(Noise3D.isNotSupportedString);
					}
					Noise3D.ms_IsSupportedChecked = true;
				}
				return Noise3D.ms_IsSupported;
			}
		}

		// Token: 0x170004B7 RID: 1207
		// (get) Token: 0x06003B48 RID: 15176 RVA: 0x0015F906 File Offset: 0x0015DB06
		public static bool isProperlyLoaded
		{
			get
			{
				return Noise3D.ms_NoiseTexture != null;
			}
		}

		// Token: 0x170004B8 RID: 1208
		// (get) Token: 0x06003B49 RID: 15177 RVA: 0x0015F913 File Offset: 0x0015DB13
		public static string isNotSupportedString
		{
			get
			{
				return string.Format("3D Noise requires higher shader capabilities (Shader Model 3.5 / OpenGL ES 3.0), which are not available on the current platform: graphicsShaderLevel (current/required) = {0} / {1}", SystemInfo.graphicsShaderLevel, 35);
			}
		}

		// Token: 0x06003B4A RID: 15178 RVA: 0x0015F930 File Offset: 0x0015DB30
		[RuntimeInitializeOnLoadMethod]
		private static void OnStartUp()
		{
			Noise3D.LoadIfNeeded();
		}

		// Token: 0x06003B4B RID: 15179 RVA: 0x0015F938 File Offset: 0x0015DB38
		public static void LoadIfNeeded()
		{
			if (!Noise3D.isSupported)
			{
				return;
			}
			if (Noise3D.ms_NoiseTexture == null)
			{
				Noise3D.ms_NoiseTexture = Noise3D.LoadTexture3D(Config.Instance.noise3DData, Config.Instance.noise3DSize);
				if (Noise3D.ms_NoiseTexture)
				{
					Noise3D.ms_NoiseTexture.hideFlags = HideFlags.HideAndDontSave;
				}
			}
			Shader.SetGlobalTexture("_VLB_NoiseTex3D", Noise3D.ms_NoiseTexture);
			Shader.SetGlobalVector("_VLB_NoiseGlobal", Config.Instance.globalNoiseParam);
		}

		// Token: 0x06003B4C RID: 15180 RVA: 0x0015F9B4 File Offset: 0x0015DBB4
		private static Texture3D LoadTexture3D(TextAsset textData, int size)
		{
			if (textData == null)
			{
				Debug.LogErrorFormat("Fail to open Noise 3D Data", Array.Empty<object>());
				return null;
			}
			byte[] bytes = textData.bytes;
			Debug.Assert(bytes != null);
			int num = Mathf.Max(0, size * size * size);
			if (bytes.Length != num)
			{
				Debug.LogErrorFormat("Noise 3D Data file has not the proper size {0}x{0}x{0}", new object[]
				{
					size
				});
				return null;
			}
			Texture3D texture3D = new Texture3D(size, size, size, TextureFormat.Alpha8, false);
			Color[] array = new Color[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new Color32(0, 0, 0, bytes[i]);
			}
			texture3D.SetPixels(array);
			texture3D.Apply();
			return texture3D;
		}
	}
}
