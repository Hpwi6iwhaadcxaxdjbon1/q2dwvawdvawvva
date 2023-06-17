using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4E RID: 2638
	[PostProcess(typeof(AutoExposureRenderer), "Unity/Auto Exposure", true)]
	[Serializable]
	public sealed class AutoExposure : PostProcessEffectSettings
	{
		// Token: 0x04003871 RID: 14449
		[MinMax(1f, 99f)]
		[DisplayName("Filtering (%)")]
		[Tooltip("Filters the bright and dark parts of the histogram when computing the average luminance. This is to avoid very dark pixels and very bright pixels from contributing to the auto exposure. Unit is in percent.")]
		public Vector2Parameter filtering = new Vector2Parameter
		{
			value = new Vector2(50f, 95f)
		};

		// Token: 0x04003872 RID: 14450
		[Range(-9f, 9f)]
		[DisplayName("Minimum (EV)")]
		[Tooltip("Minimum average luminance to consider for auto exposure. Unit is EV.")]
		public FloatParameter minLuminance = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003873 RID: 14451
		[Range(-9f, 9f)]
		[DisplayName("Maximum (EV)")]
		[Tooltip("Maximum average luminance to consider for auto exposure. Unit is EV.")]
		public FloatParameter maxLuminance = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003874 RID: 14452
		[Min(0f)]
		[DisplayName("Exposure Compensation")]
		[Tooltip("Use this to scale the global exposure of the scene.")]
		public FloatParameter keyValue = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003875 RID: 14453
		[DisplayName("Type")]
		[Tooltip("Use \"Progressive\" if you want auto exposure to be animated. Use \"Fixed\" otherwise.")]
		public EyeAdaptationParameter eyeAdaptation = new EyeAdaptationParameter
		{
			value = EyeAdaptation.Progressive
		};

		// Token: 0x04003876 RID: 14454
		[Min(0f)]
		[Tooltip("Adaptation speed from a dark to a light environment.")]
		public FloatParameter speedUp = new FloatParameter
		{
			value = 2f
		};

		// Token: 0x04003877 RID: 14455
		[Min(0f)]
		[Tooltip("Adaptation speed from a light to a dark environment.")]
		public FloatParameter speedDown = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x06003F59 RID: 16217 RVA: 0x00173A3C File Offset: 0x00171C3C
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && RenderTextureFormat.RFloat.IsSupported() && context.resources.computeShaders.autoExposure && context.resources.computeShaders.exposureHistogram;
		}
	}
}
