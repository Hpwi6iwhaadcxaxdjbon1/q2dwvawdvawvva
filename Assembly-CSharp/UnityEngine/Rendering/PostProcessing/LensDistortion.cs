using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A63 RID: 2659
	[PostProcess(typeof(LensDistortionRenderer), "Unity/Lens Distortion", true)]
	[Serializable]
	public sealed class LensDistortion : PostProcessEffectSettings
	{
		// Token: 0x040038DE RID: 14558
		[Range(-100f, 100f)]
		[Tooltip("Total distortion amount.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038DF RID: 14559
		[Range(0f, 1f)]
		[DisplayName("X Multiplier")]
		[Tooltip("Intensity multiplier on the x-axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityX = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040038E0 RID: 14560
		[Range(0f, 1f)]
		[DisplayName("Y Multiplier")]
		[Tooltip("Intensity multiplier on the y-axis. Set it to 0 to disable distortion on this axis.")]
		public FloatParameter intensityY = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040038E1 RID: 14561
		[Space]
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (x-axis).")]
		public FloatParameter centerX = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038E2 RID: 14562
		[Range(-1f, 1f)]
		[Tooltip("Distortion center point (y-axis).")]
		public FloatParameter centerY = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038E3 RID: 14563
		[Space]
		[Range(0.01f, 5f)]
		[Tooltip("Global screen scaling.")]
		public FloatParameter scale = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x06003F91 RID: 16273 RVA: 0x00176DFC File Offset: 0x00174FFC
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && !Mathf.Approximately(this.intensity, 0f) && (this.intensityX > 0f || this.intensityY > 0f) && !RuntimeUtilities.isVREnabled;
		}
	}
}
