using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000625 RID: 1573
[PostProcess(typeof(BlurOptimizedRenderer), PostProcessEvent.AfterStack, "Custom/BlurOptimized", true)]
[Serializable]
public class BlurOptimized : PostProcessEffectSettings
{
	// Token: 0x040025D8 RID: 9688
	[Range(0f, 2f)]
	public FixedIntParameter downsample = new FixedIntParameter
	{
		value = 0
	};

	// Token: 0x040025D9 RID: 9689
	[Range(1f, 4f)]
	public FixedIntParameter blurIterations = new FixedIntParameter
	{
		value = 1
	};

	// Token: 0x040025DA RID: 9690
	[Range(0f, 10f)]
	public FloatParameter blurSize = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040025DB RID: 9691
	public FloatParameter fadeToBlurDistance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040025DC RID: 9692
	public BlurTypeParameter blurType = new BlurTypeParameter
	{
		value = BlurType.StandardGauss
	};
}
