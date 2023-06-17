using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A58 RID: 2648
	[PostProcess(typeof(ColorGradingRenderer), "Unity/Color Grading", true)]
	[Serializable]
	public sealed class ColorGrading : PostProcessEffectSettings
	{
		// Token: 0x04003896 RID: 14486
		[DisplayName("Mode")]
		[Tooltip("Select a color grading mode that fits your dynamic range and workflow. Use HDR if your camera is set to render in HDR and your target platform supports it. Use LDR for low-end mobiles or devices that don't support HDR. Use External if you prefer authoring a Log LUT in an external software.")]
		public GradingModeParameter gradingMode = new GradingModeParameter
		{
			value = GradingMode.HighDefinitionRange
		};

		// Token: 0x04003897 RID: 14487
		[DisplayName("Lookup Texture")]
		[Tooltip("A custom 3D log-encoded texture.")]
		public TextureParameter externalLut = new TextureParameter
		{
			value = null
		};

		// Token: 0x04003898 RID: 14488
		[DisplayName("Mode")]
		[Tooltip("Select a tonemapping algorithm to use at the end of the color grading process.")]
		public TonemapperParameter tonemapper = new TonemapperParameter
		{
			value = Tonemapper.None
		};

		// Token: 0x04003899 RID: 14489
		[DisplayName("Toe Strength")]
		[Range(0f, 1f)]
		[Tooltip("Affects the transition between the toe and the mid section of the curve. A value of 0 means no toe, a value of 1 means a very hard transition.")]
		public FloatParameter toneCurveToeStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400389A RID: 14490
		[DisplayName("Toe Length")]
		[Range(0f, 1f)]
		[Tooltip("Affects how much of the dynamic range is in the toe. With a small value, the toe will be very short and quickly transition into the linear section, with a larger value, the toe will be longer.")]
		public FloatParameter toneCurveToeLength = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x0400389B RID: 14491
		[DisplayName("Shoulder Strength")]
		[Range(0f, 1f)]
		[Tooltip("Affects the transition between the mid section and the shoulder of the curve. A value of 0 means no shoulder, a value of 1 means a very hard transition.")]
		public FloatParameter toneCurveShoulderStrength = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400389C RID: 14492
		[DisplayName("Shoulder Length")]
		[Min(0f)]
		[Tooltip("Affects how many F-stops (EV) to add to the dynamic range of the curve.")]
		public FloatParameter toneCurveShoulderLength = new FloatParameter
		{
			value = 0.5f
		};

		// Token: 0x0400389D RID: 14493
		[DisplayName("Shoulder Angle")]
		[Range(0f, 1f)]
		[Tooltip("Affects how much overshoot to add to the shoulder.")]
		public FloatParameter toneCurveShoulderAngle = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400389E RID: 14494
		[DisplayName("Gamma")]
		[Min(0.001f)]
		[Tooltip("Applies a gamma function to the curve.")]
		public FloatParameter toneCurveGamma = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x0400389F RID: 14495
		[DisplayName("Lookup Texture")]
		[Tooltip("Custom lookup texture (strip format, for example 256x16) to apply before the rest of the color grading operators. If none is provided, a neutral one will be generated internally.")]
		public TextureParameter ldrLut = new TextureParameter
		{
			value = null,
			defaultState = TextureParameterDefault.Lut2D
		};

		// Token: 0x040038A0 RID: 14496
		[DisplayName("Contribution")]
		[Range(0f, 1f)]
		[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
		public FloatParameter ldrLutContribution = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040038A1 RID: 14497
		[DisplayName("Temperature")]
		[Range(-100f, 100f)]
		[Tooltip("Sets the white balance to a custom color temperature.")]
		public FloatParameter temperature = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A2 RID: 14498
		[DisplayName("Tint")]
		[Range(-100f, 100f)]
		[Tooltip("Sets the white balance to compensate for a green or magenta tint.")]
		public FloatParameter tint = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A3 RID: 14499
		[DisplayName("Color Filter")]
		[ColorUsage(false, true)]
		[Tooltip("Tint the render by multiplying a color.")]
		public ColorParameter colorFilter = new ColorParameter
		{
			value = Color.white
		};

		// Token: 0x040038A4 RID: 14500
		[DisplayName("Hue Shift")]
		[Range(-180f, 180f)]
		[Tooltip("Shift the hue of all colors.")]
		public FloatParameter hueShift = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A5 RID: 14501
		[DisplayName("Saturation")]
		[Range(-100f, 100f)]
		[Tooltip("Pushes the intensity of all colors.")]
		public FloatParameter saturation = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A6 RID: 14502
		[DisplayName("Brightness")]
		[Range(-100f, 100f)]
		[Tooltip("Makes the image brighter or darker.")]
		public FloatParameter brightness = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A7 RID: 14503
		[DisplayName("Post-exposure (EV)")]
		[Tooltip("Adjusts the overall exposure of the scene in EV units. This is applied after the HDR effect and right before tonemapping so it won't affect previous effects in the chain.")]
		public FloatParameter postExposure = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A8 RID: 14504
		[DisplayName("Contrast")]
		[Range(-100f, 100f)]
		[Tooltip("Expands or shrinks the overall range of tonal values.")]
		public FloatParameter contrast = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038A9 RID: 14505
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerRedOutRedIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x040038AA RID: 14506
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerRedOutGreenIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038AB RID: 14507
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerRedOutBlueIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038AC RID: 14508
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerGreenOutRedIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038AD RID: 14509
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerGreenOutGreenIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x040038AE RID: 14510
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerGreenOutBlueIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038AF RID: 14511
		[DisplayName("Red")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the red channel in the overall mix.")]
		public FloatParameter mixerBlueOutRedIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038B0 RID: 14512
		[DisplayName("Green")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the green channel in the overall mix.")]
		public FloatParameter mixerBlueOutGreenIn = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038B1 RID: 14513
		[DisplayName("Blue")]
		[Range(-200f, 200f)]
		[Tooltip("Modify influence of the blue channel in the overall mix.")]
		public FloatParameter mixerBlueOutBlueIn = new FloatParameter
		{
			value = 100f
		};

		// Token: 0x040038B2 RID: 14514
		[DisplayName("Lift")]
		[Tooltip("Controls the darkest portions of the render.")]
		[Trackball(TrackballAttribute.Mode.Lift)]
		public Vector4Parameter lift = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x040038B3 RID: 14515
		[DisplayName("Gamma")]
		[Tooltip("Power function that controls mid-range tones.")]
		[Trackball(TrackballAttribute.Mode.Gamma)]
		public Vector4Parameter gamma = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x040038B4 RID: 14516
		[DisplayName("Gain")]
		[Tooltip("Controls the lightest portions of the render.")]
		[Trackball(TrackballAttribute.Mode.Gain)]
		public Vector4Parameter gain = new Vector4Parameter
		{
			value = new Vector4(1f, 1f, 1f, 0f)
		};

		// Token: 0x040038B5 RID: 14517
		public SplineParameter masterCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040038B6 RID: 14518
		public SplineParameter redCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040038B7 RID: 14519
		public SplineParameter greenCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040038B8 RID: 14520
		public SplineParameter blueCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f, 1f, 1f),
				new Keyframe(1f, 1f, 1f, 1f)
			}), 0f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040038B9 RID: 14521
		public SplineParameter hueVsHueCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f))
		};

		// Token: 0x040038BA RID: 14522
		public SplineParameter hueVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, true, new Vector2(0f, 1f))
		};

		// Token: 0x040038BB RID: 14523
		public SplineParameter satVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f))
		};

		// Token: 0x040038BC RID: 14524
		public SplineParameter lumVsSatCurve = new SplineParameter
		{
			value = new Spline(new AnimationCurve(), 0.5f, false, new Vector2(0f, 1f))
		};

		// Token: 0x06003F6B RID: 16235 RVA: 0x00174901 File Offset: 0x00172B01
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return (this.gradingMode.value != GradingMode.External || (SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders)) && this.enabled.value;
		}
	}
}
