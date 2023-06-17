using System;
using UnityEngine;

namespace UnityStandardAssets.CinematicEffects
{
	// Token: 0x02000A15 RID: 2581
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Cinematic/Tonemapping and Color Grading")]
	[ImageEffectAllowedInSceneView]
	public class TonemappingColorGrading : MonoBehaviour
	{
		// Token: 0x0400374A RID: 14154
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.EyeAdaptationSettings m_EyeAdaptation = TonemappingColorGrading.EyeAdaptationSettings.defaultSettings;

		// Token: 0x0400374B RID: 14155
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.TonemappingSettings m_Tonemapping = TonemappingColorGrading.TonemappingSettings.defaultSettings;

		// Token: 0x0400374C RID: 14156
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.ColorGradingSettings m_ColorGrading = TonemappingColorGrading.ColorGradingSettings.defaultSettings;

		// Token: 0x0400374D RID: 14157
		[SerializeField]
		[TonemappingColorGrading.SettingsGroup]
		private TonemappingColorGrading.LUTSettings m_Lut = TonemappingColorGrading.LUTSettings.defaultSettings;

		// Token: 0x0400374E RID: 14158
		[SerializeField]
		private Shader m_Shader;

		// Token: 0x02000EF9 RID: 3833
		[AttributeUsage(AttributeTargets.Field)]
		public class SettingsGroup : Attribute
		{
		}

		// Token: 0x02000EFA RID: 3834
		public class IndentedGroup : PropertyAttribute
		{
		}

		// Token: 0x02000EFB RID: 3835
		public class ChannelMixer : PropertyAttribute
		{
		}

		// Token: 0x02000EFC RID: 3836
		public class ColorWheelGroup : PropertyAttribute
		{
			// Token: 0x04004DE3 RID: 19939
			public int minSizePerWheel = 60;

			// Token: 0x04004DE4 RID: 19940
			public int maxSizePerWheel = 150;

			// Token: 0x060053E6 RID: 21478 RVA: 0x001B431F File Offset: 0x001B251F
			public ColorWheelGroup()
			{
			}

			// Token: 0x060053E7 RID: 21479 RVA: 0x001B433A File Offset: 0x001B253A
			public ColorWheelGroup(int minSizePerWheel, int maxSizePerWheel)
			{
				this.minSizePerWheel = minSizePerWheel;
				this.maxSizePerWheel = maxSizePerWheel;
			}
		}

		// Token: 0x02000EFD RID: 3837
		public class Curve : PropertyAttribute
		{
			// Token: 0x04004DE5 RID: 19941
			public Color color = Color.white;

			// Token: 0x060053E8 RID: 21480 RVA: 0x001B4363 File Offset: 0x001B2563
			public Curve()
			{
			}

			// Token: 0x060053E9 RID: 21481 RVA: 0x001B4376 File Offset: 0x001B2576
			public Curve(float r, float g, float b, float a)
			{
				this.color = new Color(r, g, b, a);
			}
		}

		// Token: 0x02000EFE RID: 3838
		[Serializable]
		public struct EyeAdaptationSettings
		{
			// Token: 0x04004DE6 RID: 19942
			public bool enabled;

			// Token: 0x04004DE7 RID: 19943
			[Min(0f)]
			[Tooltip("Midpoint Adjustment.")]
			public float middleGrey;

			// Token: 0x04004DE8 RID: 19944
			[Tooltip("The lowest possible exposure value; adjust this value to modify the brightest areas of your level.")]
			public float min;

			// Token: 0x04004DE9 RID: 19945
			[Tooltip("The highest possible exposure value; adjust this value to modify the darkest areas of your level.")]
			public float max;

			// Token: 0x04004DEA RID: 19946
			[Min(0f)]
			[Tooltip("Speed of linear adaptation. Higher is faster.")]
			public float speed;

			// Token: 0x04004DEB RID: 19947
			[Tooltip("Displays a luminosity helper in the GameView.")]
			public bool showDebug;

			// Token: 0x1700071A RID: 1818
			// (get) Token: 0x060053EA RID: 21482 RVA: 0x001B439C File Offset: 0x001B259C
			public static TonemappingColorGrading.EyeAdaptationSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.EyeAdaptationSettings
					{
						enabled = false,
						showDebug = false,
						middleGrey = 0.5f,
						min = -3f,
						max = 3f,
						speed = 1.5f
					};
				}
			}
		}

		// Token: 0x02000EFF RID: 3839
		public enum Tonemapper
		{
			// Token: 0x04004DED RID: 19949
			ACES,
			// Token: 0x04004DEE RID: 19950
			Curve,
			// Token: 0x04004DEF RID: 19951
			Hable,
			// Token: 0x04004DF0 RID: 19952
			HejlDawson,
			// Token: 0x04004DF1 RID: 19953
			Photographic,
			// Token: 0x04004DF2 RID: 19954
			Reinhard,
			// Token: 0x04004DF3 RID: 19955
			Neutral
		}

		// Token: 0x02000F00 RID: 3840
		[Serializable]
		public struct TonemappingSettings
		{
			// Token: 0x04004DF4 RID: 19956
			public bool enabled;

			// Token: 0x04004DF5 RID: 19957
			[Tooltip("Tonemapping technique to use. ACES is the recommended one.")]
			public TonemappingColorGrading.Tonemapper tonemapper;

			// Token: 0x04004DF6 RID: 19958
			[Min(0f)]
			[Tooltip("Adjusts the overall exposure of the scene.")]
			public float exposure;

			// Token: 0x04004DF7 RID: 19959
			[Tooltip("Custom tonemapping curve.")]
			public AnimationCurve curve;

			// Token: 0x04004DF8 RID: 19960
			[Range(-0.1f, 0.1f)]
			public float neutralBlackIn;

			// Token: 0x04004DF9 RID: 19961
			[Range(1f, 20f)]
			public float neutralWhiteIn;

			// Token: 0x04004DFA RID: 19962
			[Range(-0.09f, 0.1f)]
			public float neutralBlackOut;

			// Token: 0x04004DFB RID: 19963
			[Range(1f, 19f)]
			public float neutralWhiteOut;

			// Token: 0x04004DFC RID: 19964
			[Range(0.1f, 20f)]
			public float neutralWhiteLevel;

			// Token: 0x04004DFD RID: 19965
			[Range(1f, 10f)]
			public float neutralWhiteClip;

			// Token: 0x1700071B RID: 1819
			// (get) Token: 0x060053EB RID: 21483 RVA: 0x001B43F4 File Offset: 0x001B25F4
			public static TonemappingColorGrading.TonemappingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.TonemappingSettings
					{
						enabled = false,
						tonemapper = TonemappingColorGrading.Tonemapper.Neutral,
						exposure = 1f,
						curve = TonemappingColorGrading.CurvesSettings.defaultCurve,
						neutralBlackIn = 0.02f,
						neutralWhiteIn = 10f,
						neutralBlackOut = 0f,
						neutralWhiteOut = 10f,
						neutralWhiteLevel = 5.3f,
						neutralWhiteClip = 10f
					};
				}
			}
		}

		// Token: 0x02000F01 RID: 3841
		[Serializable]
		public struct LUTSettings
		{
			// Token: 0x04004DFE RID: 19966
			public bool enabled;

			// Token: 0x04004DFF RID: 19967
			[Tooltip("Custom lookup texture (strip format, e.g. 256x16).")]
			public Texture texture;

			// Token: 0x04004E00 RID: 19968
			[Range(0f, 1f)]
			[Tooltip("Blending factor.")]
			public float contribution;

			// Token: 0x1700071C RID: 1820
			// (get) Token: 0x060053EC RID: 21484 RVA: 0x001B447C File Offset: 0x001B267C
			public static TonemappingColorGrading.LUTSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.LUTSettings
					{
						enabled = false,
						texture = null,
						contribution = 1f
					};
				}
			}
		}

		// Token: 0x02000F02 RID: 3842
		[Serializable]
		public struct ColorWheelsSettings
		{
			// Token: 0x04004E01 RID: 19969
			[ColorUsage(false)]
			public Color shadows;

			// Token: 0x04004E02 RID: 19970
			[ColorUsage(false)]
			public Color midtones;

			// Token: 0x04004E03 RID: 19971
			[ColorUsage(false)]
			public Color highlights;

			// Token: 0x1700071D RID: 1821
			// (get) Token: 0x060053ED RID: 21485 RVA: 0x001B44B0 File Offset: 0x001B26B0
			public static TonemappingColorGrading.ColorWheelsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorWheelsSettings
					{
						shadows = Color.white,
						midtones = Color.white,
						highlights = Color.white
					};
				}
			}
		}

		// Token: 0x02000F03 RID: 3843
		[Serializable]
		public struct BasicsSettings
		{
			// Token: 0x04004E04 RID: 19972
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to a custom color temperature.")]
			public float temperatureShift;

			// Token: 0x04004E05 RID: 19973
			[Range(-2f, 2f)]
			[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
			public float tint;

			// Token: 0x04004E06 RID: 19974
			[Space]
			[Range(-0.5f, 0.5f)]
			[Tooltip("Shift the hue of all colors.")]
			public float hue;

			// Token: 0x04004E07 RID: 19975
			[Range(0f, 2f)]
			[Tooltip("Pushes the intensity of all colors.")]
			public float saturation;

			// Token: 0x04004E08 RID: 19976
			[Range(-1f, 1f)]
			[Tooltip("Adjusts the saturation so that clipping is minimized as colors approach full saturation.")]
			public float vibrance;

			// Token: 0x04004E09 RID: 19977
			[Range(0f, 10f)]
			[Tooltip("Brightens or darkens all colors.")]
			public float value;

			// Token: 0x04004E0A RID: 19978
			[Space]
			[Range(0f, 2f)]
			[Tooltip("Expands or shrinks the overall range of tonal values.")]
			public float contrast;

			// Token: 0x04004E0B RID: 19979
			[Range(0.01f, 5f)]
			[Tooltip("Contrast gain curve. Controls the steepness of the curve.")]
			public float gain;

			// Token: 0x04004E0C RID: 19980
			[Range(0.01f, 5f)]
			[Tooltip("Applies a pow function to the source.")]
			public float gamma;

			// Token: 0x1700071E RID: 1822
			// (get) Token: 0x060053EE RID: 21486 RVA: 0x001B44EC File Offset: 0x001B26EC
			public static TonemappingColorGrading.BasicsSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.BasicsSettings
					{
						temperatureShift = 0f,
						tint = 0f,
						contrast = 1f,
						hue = 0f,
						saturation = 1f,
						value = 1f,
						vibrance = 0f,
						gain = 1f,
						gamma = 1f
					};
				}
			}
		}

		// Token: 0x02000F04 RID: 3844
		[Serializable]
		public struct ChannelMixerSettings
		{
			// Token: 0x04004E0D RID: 19981
			public int currentChannel;

			// Token: 0x04004E0E RID: 19982
			public Vector3[] channels;

			// Token: 0x1700071F RID: 1823
			// (get) Token: 0x060053EF RID: 21487 RVA: 0x001B4570 File Offset: 0x001B2770
			public static TonemappingColorGrading.ChannelMixerSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ChannelMixerSettings
					{
						currentChannel = 0,
						channels = new Vector3[]
						{
							new Vector3(1f, 0f, 0f),
							new Vector3(0f, 1f, 0f),
							new Vector3(0f, 0f, 1f)
						}
					};
				}
			}
		}

		// Token: 0x02000F05 RID: 3845
		[Serializable]
		public struct CurvesSettings
		{
			// Token: 0x04004E0F RID: 19983
			[TonemappingColorGrading.Curve]
			public AnimationCurve master;

			// Token: 0x04004E10 RID: 19984
			[TonemappingColorGrading.Curve(1f, 0f, 0f, 1f)]
			public AnimationCurve red;

			// Token: 0x04004E11 RID: 19985
			[TonemappingColorGrading.Curve(0f, 1f, 0f, 1f)]
			public AnimationCurve green;

			// Token: 0x04004E12 RID: 19986
			[TonemappingColorGrading.Curve(0f, 1f, 1f, 1f)]
			public AnimationCurve blue;

			// Token: 0x17000720 RID: 1824
			// (get) Token: 0x060053F0 RID: 21488 RVA: 0x001B45EC File Offset: 0x001B27EC
			public static TonemappingColorGrading.CurvesSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.CurvesSettings
					{
						master = TonemappingColorGrading.CurvesSettings.defaultCurve,
						red = TonemappingColorGrading.CurvesSettings.defaultCurve,
						green = TonemappingColorGrading.CurvesSettings.defaultCurve,
						blue = TonemappingColorGrading.CurvesSettings.defaultCurve
					};
				}
			}

			// Token: 0x17000721 RID: 1825
			// (get) Token: 0x060053F1 RID: 21489 RVA: 0x001B4634 File Offset: 0x001B2834
			public static AnimationCurve defaultCurve
			{
				get
				{
					return new AnimationCurve(new Keyframe[]
					{
						new Keyframe(0f, 0f, 1f, 1f),
						new Keyframe(1f, 1f, 1f, 1f)
					});
				}
			}
		}

		// Token: 0x02000F06 RID: 3846
		public enum ColorGradingPrecision
		{
			// Token: 0x04004E14 RID: 19988
			Normal = 16,
			// Token: 0x04004E15 RID: 19989
			High = 32
		}

		// Token: 0x02000F07 RID: 3847
		[Serializable]
		public struct ColorGradingSettings
		{
			// Token: 0x04004E16 RID: 19990
			public bool enabled;

			// Token: 0x04004E17 RID: 19991
			[Tooltip("Internal LUT precision. \"Normal\" is 256x16, \"High\" is 1024x32. Prefer \"Normal\" on mobile devices.")]
			public TonemappingColorGrading.ColorGradingPrecision precision;

			// Token: 0x04004E18 RID: 19992
			[Space]
			[TonemappingColorGrading.ColorWheelGroup]
			public TonemappingColorGrading.ColorWheelsSettings colorWheels;

			// Token: 0x04004E19 RID: 19993
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.BasicsSettings basics;

			// Token: 0x04004E1A RID: 19994
			[Space]
			[TonemappingColorGrading.ChannelMixer]
			public TonemappingColorGrading.ChannelMixerSettings channelMixer;

			// Token: 0x04004E1B RID: 19995
			[Space]
			[TonemappingColorGrading.IndentedGroup]
			public TonemappingColorGrading.CurvesSettings curves;

			// Token: 0x04004E1C RID: 19996
			[Space]
			[Tooltip("Use dithering to try and minimize color banding in dark areas.")]
			public bool useDithering;

			// Token: 0x04004E1D RID: 19997
			[Tooltip("Displays the generated LUT in the top left corner of the GameView.")]
			public bool showDebug;

			// Token: 0x17000722 RID: 1826
			// (get) Token: 0x060053F2 RID: 21490 RVA: 0x001B468C File Offset: 0x001B288C
			public static TonemappingColorGrading.ColorGradingSettings defaultSettings
			{
				get
				{
					return new TonemappingColorGrading.ColorGradingSettings
					{
						enabled = false,
						useDithering = false,
						showDebug = false,
						precision = TonemappingColorGrading.ColorGradingPrecision.Normal,
						colorWheels = TonemappingColorGrading.ColorWheelsSettings.defaultSettings,
						basics = TonemappingColorGrading.BasicsSettings.defaultSettings,
						channelMixer = TonemappingColorGrading.ChannelMixerSettings.defaultSettings,
						curves = TonemappingColorGrading.CurvesSettings.defaultSettings
					};
				}
			}

			// Token: 0x060053F3 RID: 21491 RVA: 0x001B46F3 File Offset: 0x001B28F3
			internal void Reset()
			{
				this.curves = TonemappingColorGrading.CurvesSettings.defaultSettings;
			}
		}
	}
}
