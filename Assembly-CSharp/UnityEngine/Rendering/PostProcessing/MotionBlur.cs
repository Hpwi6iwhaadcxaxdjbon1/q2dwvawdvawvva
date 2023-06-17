using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A65 RID: 2661
	[PostProcess(typeof(MotionBlurRenderer), "Unity/Motion Blur", false)]
	[Serializable]
	public sealed class MotionBlur : PostProcessEffectSettings
	{
		// Token: 0x040038E4 RID: 14564
		[Range(0f, 360f)]
		[Tooltip("The angle of rotary shutter. Larger values give longer exposure.")]
		public FloatParameter shutterAngle = new FloatParameter
		{
			value = 270f
		};

		// Token: 0x040038E5 RID: 14565
		[Range(4f, 32f)]
		[Tooltip("The amount of sample points. This affects quality and performance.")]
		public IntParameter sampleCount = new IntParameter
		{
			value = 10
		};

		// Token: 0x06003F95 RID: 16277 RVA: 0x00177031 File Offset: 0x00175231
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.shutterAngle.value > 0f && SystemInfo.supportsMotionVectors && RenderTextureFormat.RGHalf.IsSupported() && !RuntimeUtilities.isVREnabled;
		}
	}
}
