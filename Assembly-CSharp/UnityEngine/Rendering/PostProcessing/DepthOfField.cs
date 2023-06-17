using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5C RID: 2652
	[PostProcess(typeof(DepthOfFieldRenderer), "Unity/Depth of Field", false)]
	[Serializable]
	public sealed class DepthOfField : PostProcessEffectSettings
	{
		// Token: 0x040038C9 RID: 14537
		[Min(0.1f)]
		[Tooltip("Distance to the point of focus.")]
		public FloatParameter focusDistance = new FloatParameter
		{
			value = 10f
		};

		// Token: 0x040038CA RID: 14538
		[Range(0.05f, 32f)]
		[Tooltip("Ratio of aperture (known as f-stop or f-number). The smaller the value is, the shallower the depth of field is.")]
		public FloatParameter aperture = new FloatParameter
		{
			value = 5.6f
		};

		// Token: 0x040038CB RID: 14539
		[Range(1f, 300f)]
		[Tooltip("Distance between the lens and the film. The larger the value is, the shallower the depth of field is.")]
		public FloatParameter focalLength = new FloatParameter
		{
			value = 50f
		};

		// Token: 0x040038CC RID: 14540
		[DisplayName("Max Blur Size")]
		[Tooltip("Convolution kernel size of the bokeh filter, which determines the maximum radius of bokeh. It also affects performances (the larger the kernel is, the longer the GPU time is required).")]
		public KernelSizeParameter kernelSize = new KernelSizeParameter
		{
			value = KernelSize.Medium
		};

		// Token: 0x06003F7B RID: 16251 RVA: 0x001762FE File Offset: 0x001744FE
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && SystemInfo.graphicsShaderLevel >= 35;
		}
	}
}
