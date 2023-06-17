using System;
using Rust.Workshop;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConVar
{
	// Token: 0x02000ABE RID: 2750
	[ConsoleSystem.Factory("graphics")]
	public class Graphics : ConsoleSystem
	{
		// Token: 0x04003B53 RID: 15187
		private const float MinShadowDistance = 100f;

		// Token: 0x04003B54 RID: 15188
		private const float MaxShadowDistance2Split = 600f;

		// Token: 0x04003B55 RID: 15189
		private const float MaxShadowDistance4Split = 1000f;

		// Token: 0x04003B56 RID: 15190
		private static float _shadowdistance = 1000f;

		// Token: 0x04003B57 RID: 15191
		[ClientVar(Saved = true)]
		public static int shadowmode = 2;

		// Token: 0x04003B58 RID: 15192
		[ClientVar(Saved = true)]
		public static int shadowlights = 1;

		// Token: 0x04003B59 RID: 15193
		private static int _shadowquality = 1;

		// Token: 0x04003B5A RID: 15194
		[ClientVar(Saved = true)]
		public static bool grassshadows = false;

		// Token: 0x04003B5B RID: 15195
		[ClientVar(Saved = true)]
		public static bool contactshadows = false;

		// Token: 0x04003B5C RID: 15196
		[ClientVar(Saved = true)]
		public static float drawdistance = 2500f;

		// Token: 0x04003B5D RID: 15197
		private static float _fov = 75f;

		// Token: 0x04003B5E RID: 15198
		[ClientVar]
		public static bool hud = true;

		// Token: 0x04003B5F RID: 15199
		[ClientVar(Saved = true)]
		public static bool chat = true;

		// Token: 0x04003B60 RID: 15200
		[ClientVar(Saved = true)]
		public static bool branding = true;

		// Token: 0x04003B61 RID: 15201
		[ClientVar(Saved = true)]
		public static int compass = 1;

		// Token: 0x04003B62 RID: 15202
		[ClientVar(Saved = true)]
		public static bool dof = false;

		// Token: 0x04003B63 RID: 15203
		[ClientVar(Saved = true)]
		public static float dof_aper = 12f;

		// Token: 0x04003B64 RID: 15204
		[ClientVar(Saved = true)]
		public static float dof_blur = 1f;

		// Token: 0x04003B65 RID: 15205
		[ClientVar(Saved = true, Help = "0 = auto 1 = manual 2 = dynamic based on target")]
		public static int dof_mode = 0;

		// Token: 0x04003B66 RID: 15206
		[ClientVar(Saved = true, Help = "distance from camera to focus on")]
		public static float dof_focus_dist = 10f;

		// Token: 0x04003B67 RID: 15207
		[ClientVar(Saved = true)]
		public static float dof_focus_time = 0.2f;

		// Token: 0x04003B68 RID: 15208
		[ClientVar(Saved = true, ClientAdmin = true)]
		public static bool dof_debug = false;

		// Token: 0x04003B69 RID: 15209
		[ClientVar(Saved = true, Help = "Goes from 0 - 3, higher = more dof samples but slower perf")]
		public static int dof_kernel_count = 0;

		// Token: 0x04003B6A RID: 15210
		public static BaseEntity dof_focus_target_entity = null;

		// Token: 0x04003B6B RID: 15211
		[ClientVar(Saved = true, Help = "Whether to scale vm models with fov")]
		public static bool vm_fov_scale = true;

		// Token: 0x04003B6C RID: 15212
		[ClientVar(Saved = true, Help = "FLips viewmodels horizontally (for left handed players)")]
		public static bool vm_horizontal_flip = false;

		// Token: 0x04003B6D RID: 15213
		private static float _uiscale = 1f;

		// Token: 0x04003B6E RID: 15214
		private static int _anisotropic = 1;

		// Token: 0x04003B6F RID: 15215
		private static int _parallax = 0;

		// Token: 0x170005B1 RID: 1457
		// (get) Token: 0x0600421C RID: 16924 RVA: 0x00187F86 File Offset: 0x00186186
		// (set) Token: 0x0600421D RID: 16925 RVA: 0x00187F8D File Offset: 0x0018618D
		[ClientVar(Help = "The currently selected quality level")]
		public static int quality
		{
			get
			{
				return QualitySettings.GetQualityLevel();
			}
			set
			{
				int shadowcascades = Graphics.shadowcascades;
				QualitySettings.SetQualityLevel(value, true);
				Graphics.shadowcascades = shadowcascades;
			}
		}

		// Token: 0x0600421E RID: 16926 RVA: 0x00187FA0 File Offset: 0x001861A0
		public static float EnforceShadowDistanceBounds(float distance)
		{
			if (QualitySettings.shadowCascades == 1)
			{
				distance = Mathf.Clamp(distance, 100f, 100f);
			}
			else if (QualitySettings.shadowCascades == 2)
			{
				distance = Mathf.Clamp(distance, 100f, 600f);
			}
			else
			{
				distance = Mathf.Clamp(distance, 100f, 1000f);
			}
			return distance;
		}

		// Token: 0x170005B2 RID: 1458
		// (get) Token: 0x0600421F RID: 16927 RVA: 0x00187FF8 File Offset: 0x001861F8
		// (set) Token: 0x06004220 RID: 16928 RVA: 0x00187FFF File Offset: 0x001861FF
		[ClientVar(Saved = true)]
		public static float shadowdistance
		{
			get
			{
				return Graphics._shadowdistance;
			}
			set
			{
				Graphics._shadowdistance = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics._shadowdistance);
			}
		}

		// Token: 0x170005B3 RID: 1459
		// (get) Token: 0x06004221 RID: 16929 RVA: 0x00188016 File Offset: 0x00186216
		// (set) Token: 0x06004222 RID: 16930 RVA: 0x0018801D File Offset: 0x0018621D
		[ClientVar(Saved = true)]
		public static int shadowcascades
		{
			get
			{
				return QualitySettings.shadowCascades;
			}
			set
			{
				QualitySettings.shadowCascades = value;
				QualitySettings.shadowDistance = Graphics.EnforceShadowDistanceBounds(Graphics.shadowdistance);
			}
		}

		// Token: 0x170005B4 RID: 1460
		// (get) Token: 0x06004223 RID: 16931 RVA: 0x00188034 File Offset: 0x00186234
		// (set) Token: 0x06004224 RID: 16932 RVA: 0x0018803C File Offset: 0x0018623C
		[ClientVar(Saved = true)]
		public static int shadowquality
		{
			get
			{
				return Graphics._shadowquality;
			}
			set
			{
				Graphics._shadowquality = Mathf.Clamp(value, 0, 3);
				Graphics.shadowmode = Graphics._shadowquality + 1;
				bool flag = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore;
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_HIGH", !flag && Graphics._shadowquality == 2);
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_VERYHIGH", !flag && Graphics._shadowquality == 3);
			}
		}

		// Token: 0x170005B5 RID: 1461
		// (get) Token: 0x06004225 RID: 16933 RVA: 0x0018809C File Offset: 0x0018629C
		// (set) Token: 0x06004226 RID: 16934 RVA: 0x001880A3 File Offset: 0x001862A3
		[ClientVar(Saved = true)]
		public static float fov
		{
			get
			{
				return Graphics._fov;
			}
			set
			{
				Graphics._fov = Mathf.Clamp(value, 70f, 90f);
			}
		}

		// Token: 0x170005B6 RID: 1462
		// (get) Token: 0x06004227 RID: 16935 RVA: 0x001880BA File Offset: 0x001862BA
		// (set) Token: 0x06004228 RID: 16936 RVA: 0x001880C1 File Offset: 0x001862C1
		[ClientVar]
		public static float lodbias
		{
			get
			{
				return QualitySettings.lodBias;
			}
			set
			{
				QualitySettings.lodBias = Mathf.Clamp(value, 0.25f, 5f);
			}
		}

		// Token: 0x06004229 RID: 16937 RVA: 0x000063A5 File Offset: 0x000045A5
		[ClientVar(ClientAdmin = true)]
		public static void dof_focus_target(ConsoleSystem.Arg arg)
		{
		}

		// Token: 0x0600422A RID: 16938 RVA: 0x001880D8 File Offset: 0x001862D8
		[ClientVar]
		public static void dof_nudge(ConsoleSystem.Arg arg)
		{
			float @float = arg.GetFloat(0, 0f);
			Graphics.dof_focus_dist += @float;
			if (Graphics.dof_focus_dist < 0f)
			{
				Graphics.dof_focus_dist = 0f;
			}
		}

		// Token: 0x170005B7 RID: 1463
		// (get) Token: 0x0600422B RID: 16939 RVA: 0x00188114 File Offset: 0x00186314
		// (set) Token: 0x0600422C RID: 16940 RVA: 0x0018811B File Offset: 0x0018631B
		[ClientVar(Saved = true)]
		public static int shaderlod
		{
			get
			{
				return Shader.globalMaximumLOD;
			}
			set
			{
				Shader.globalMaximumLOD = Mathf.Clamp(value, 100, 600);
			}
		}

		// Token: 0x170005B8 RID: 1464
		// (get) Token: 0x0600422D RID: 16941 RVA: 0x0018812F File Offset: 0x0018632F
		// (set) Token: 0x0600422E RID: 16942 RVA: 0x00188136 File Offset: 0x00186336
		[ClientVar(Saved = true)]
		public static float uiscale
		{
			get
			{
				return Graphics._uiscale;
			}
			set
			{
				Graphics._uiscale = Mathf.Clamp(value, 0.5f, 1f);
			}
		}

		// Token: 0x170005B9 RID: 1465
		// (get) Token: 0x0600422F RID: 16943 RVA: 0x0018814D File Offset: 0x0018634D
		// (set) Token: 0x06004230 RID: 16944 RVA: 0x00188154 File Offset: 0x00186354
		[ClientVar(Saved = true)]
		public static int af
		{
			get
			{
				return Graphics._anisotropic;
			}
			set
			{
				value = Mathf.Clamp(value, 1, 16);
				Texture.SetGlobalAnisotropicFilteringLimits(1, value);
				if (value <= 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Disable;
				}
				if (value > 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Enable;
				}
				Graphics._anisotropic = value;
			}
		}

		// Token: 0x170005BA RID: 1466
		// (get) Token: 0x06004231 RID: 16945 RVA: 0x00188182 File Offset: 0x00186382
		// (set) Token: 0x06004232 RID: 16946 RVA: 0x0018818C File Offset: 0x0018638C
		[ClientVar(Saved = true)]
		public static int parallax
		{
			get
			{
				return Graphics._parallax;
			}
			set
			{
				if (value != 1)
				{
					if (value != 2)
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
					else
					{
						Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
						Shader.EnableKeyword("TERRAIN_PARALLAX_OCCLUSION");
					}
				}
				else
				{
					Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				Graphics._parallax = value;
			}
		}

		// Token: 0x170005BB RID: 1467
		// (get) Token: 0x06004233 RID: 16947 RVA: 0x001881E7 File Offset: 0x001863E7
		// (set) Token: 0x06004234 RID: 16948 RVA: 0x001881EE File Offset: 0x001863EE
		[ClientVar(ClientAdmin = true)]
		public static bool itemskins
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.AllowApply;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.AllowApply = value;
			}
		}

		// Token: 0x170005BC RID: 1468
		// (get) Token: 0x06004235 RID: 16949 RVA: 0x001881F6 File Offset: 0x001863F6
		// (set) Token: 0x06004236 RID: 16950 RVA: 0x001881FD File Offset: 0x001863FD
		[ClientVar]
		public static bool itemskinunload
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.AllowUnload;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.AllowUnload = value;
			}
		}

		// Token: 0x170005BD RID: 1469
		// (get) Token: 0x06004237 RID: 16951 RVA: 0x00188205 File Offset: 0x00186405
		// (set) Token: 0x06004238 RID: 16952 RVA: 0x0018820C File Offset: 0x0018640C
		[ClientVar(ClientAdmin = true)]
		public static float itemskintimeout
		{
			get
			{
				return Rust.Workshop.WorkshopSkin.DownloadTimeout;
			}
			set
			{
				Rust.Workshop.WorkshopSkin.DownloadTimeout = value;
			}
		}
	}
}
