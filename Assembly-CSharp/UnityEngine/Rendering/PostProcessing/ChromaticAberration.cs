using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A52 RID: 2642
	[PostProcess(typeof(ChromaticAberrationRenderer), "Unity/Chromatic Aberration", true)]
	[Serializable]
	public sealed class ChromaticAberration : PostProcessEffectSettings
	{
		// Token: 0x04003889 RID: 14473
		[Tooltip("Shifts the hue of chromatic aberrations.")]
		public TextureParameter spectralLut = new TextureParameter
		{
			value = null
		};

		// Token: 0x0400388A RID: 14474
		[Range(0f, 1f)]
		[Tooltip("Amount of tangential distortion.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x0400388B RID: 14475
		[FormerlySerializedAs("mobileOptimized")]
		[Tooltip("Boost performances by lowering the effect quality. This settings is meant to be used on mobile and other low-end platforms but can also provide a nice performance boost on desktops and consoles.")]
		public BoolParameter fastMode = new BoolParameter
		{
			value = false
		};

		// Token: 0x06003F64 RID: 16228 RVA: 0x0017470C File Offset: 0x0017290C
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}
	}
}
