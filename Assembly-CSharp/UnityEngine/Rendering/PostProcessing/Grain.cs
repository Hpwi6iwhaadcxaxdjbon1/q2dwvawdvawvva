using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A61 RID: 2657
	[PostProcess(typeof(GrainRenderer), "Unity/Grain", true)]
	[Serializable]
	public sealed class Grain : PostProcessEffectSettings
	{
		// Token: 0x040038D7 RID: 14551
		[Tooltip("Enable the use of colored grain.")]
		public BoolParameter colored = new BoolParameter
		{
			value = true
		};

		// Token: 0x040038D8 RID: 14552
		[Range(0f, 1f)]
		[Tooltip("Grain strength. Higher values mean more visible grain.")]
		public FloatParameter intensity = new FloatParameter
		{
			value = 0f
		};

		// Token: 0x040038D9 RID: 14553
		[Range(0.3f, 3f)]
		[Tooltip("Grain particle size.")]
		public FloatParameter size = new FloatParameter
		{
			value = 1f
		};

		// Token: 0x040038DA RID: 14554
		[Range(0f, 1f)]
		[DisplayName("Luminance Contribution")]
		[Tooltip("Controls the noise response curve based on scene luminance. Lower values mean less noise in dark areas.")]
		public FloatParameter lumContrib = new FloatParameter
		{
			value = 0.8f
		};

		// Token: 0x06003F8B RID: 16267 RVA: 0x00176AE6 File Offset: 0x00174CE6
		public override bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled.value && this.intensity.value > 0f;
		}
	}
}
