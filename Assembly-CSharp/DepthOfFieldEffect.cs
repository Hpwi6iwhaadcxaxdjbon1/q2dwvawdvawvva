using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062A RID: 1578
[PostProcess(typeof(DepthOfFieldEffectRenderer), "Unity/Depth of Field (Custom)", false)]
[Serializable]
public class DepthOfFieldEffect : PostProcessEffectSettings
{
	// Token: 0x040025E5 RID: 9701
	public FloatParameter focalLength = new FloatParameter
	{
		value = 10f
	};

	// Token: 0x040025E6 RID: 9702
	public FloatParameter focalSize = new FloatParameter
	{
		value = 0.05f
	};

	// Token: 0x040025E7 RID: 9703
	public FloatParameter aperture = new FloatParameter
	{
		value = 11.5f
	};

	// Token: 0x040025E8 RID: 9704
	public FloatParameter maxBlurSize = new FloatParameter
	{
		value = 2f
	};

	// Token: 0x040025E9 RID: 9705
	public BoolParameter highResolution = new BoolParameter
	{
		value = true
	};

	// Token: 0x040025EA RID: 9706
	public DOFBlurSampleCountParameter blurSampleCount = new DOFBlurSampleCountParameter
	{
		value = DOFBlurSampleCount.Low
	};

	// Token: 0x040025EB RID: 9707
	public Transform focalTransform;
}
