using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000636 RID: 1590
[PostProcess(typeof(GodRaysRenderer), PostProcessEvent.BeforeStack, "Custom/GodRays", true)]
[Serializable]
public class GodRays : PostProcessEffectSettings
{
	// Token: 0x04002608 RID: 9736
	public BoolParameter UseDepth = new BoolParameter
	{
		value = true
	};

	// Token: 0x04002609 RID: 9737
	public BlendModeTypeParameter BlendMode = new BlendModeTypeParameter
	{
		value = BlendModeType.Screen
	};

	// Token: 0x0400260A RID: 9738
	public FloatParameter Intensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400260B RID: 9739
	public ResolutionTypeParameter Resolution = new ResolutionTypeParameter
	{
		value = ResolutionType.High
	};

	// Token: 0x0400260C RID: 9740
	public IntParameter BlurIterations = new IntParameter
	{
		value = 2
	};

	// Token: 0x0400260D RID: 9741
	public FloatParameter BlurRadius = new FloatParameter
	{
		value = 2f
	};

	// Token: 0x0400260E RID: 9742
	public FloatParameter MaxRadius = new FloatParameter
	{
		value = 0.5f
	};
}
