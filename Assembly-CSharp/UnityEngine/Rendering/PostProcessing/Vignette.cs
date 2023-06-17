using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A73 RID: 2675
	[PostProcess(typeof(VignetteRenderer), "Unity/Vignette", true)]
	[Serializable]
	public sealed class Vignette : PostProcessEffectSettings
	{
		// Token: 0x04003920 RID: 14624
		[Tooltip("Use the \"Classic\" mode for parametric controls. Use the \"Masked\" mode to use your own texture mask.")]
		public VignetteModeParameter mode = new VignetteModeParameter
		{
			value = VignetteMode.Classic
		};

		// Token: 0x04003921 RID: 14625
		[Tooltip("Vignette color.")]
		public ColorParameter color = new ColorParameter
		{
			value = new Color(0f, 0f, 0f, 1f)
		};

		// Token: 0x04003922 RID: 14626
		[Tooltip("Sets the vignette center point (screen center is [0.5, 0.5]).")]
		public Vector2Parameter center = new Vector2Parameter
		{
			value = new Vector2(0.5f, 0.5f)
		};

		// Token: 0x04003923 RID: 14627
		[Range(0f, 1f)]
		[Tooltip("Amount of vignetting on screen.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x04003924 RID: 14628
		[Range(0.01f, 1f)]
		[Tooltip("Smoothness of the vignette borders.")]
		public FloatParameter smoothness = new FloatParameter
		{
			value = 0.2f
		};

		// Token: 0x04003925 RID: 14629
		[Range(0f, 1f)]
		[Tooltip("Lower values will make a square-ish vignette.")]
		public FloatParameter roundness = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x04003926 RID: 14630
		[Tooltip("Set to true to mark the vignette to be perfectly round. False will make its shape dependent on the current aspect ratio.")]
		public BoolParameter rounded = new BoolParameter
		{
			value = false
		};

		// Token: 0x04003927 RID: 14631
		[Tooltip("A black and white mask to use as a vignette.")]
		public TextureParameter mask = new TextureParameter
		{
			value = null
		};

		// Token: 0x04003928 RID: 14632
		[Range(0f, 1f)]
		[Tooltip("Mask opacity.")]
		public FloatParameter opacity = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x06003FDA RID: 16346 RVA: 0x00179B90 File Offset: 0x00177D90
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && ((this.mode.value == VignetteMode.Classic && this.intensity.value > 0f) || (this.mode.value == VignetteMode.Masked && this.opacity.value > 0f && this.mask.value != null));
		}
	}
}
