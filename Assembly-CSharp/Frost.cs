using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000630 RID: 1584
[PostProcess(typeof(FrostRenderer), PostProcessEvent.AfterStack, "Custom/Frost", true)]
[Serializable]
public class Frost : PostProcessEffectSettings
{
	// Token: 0x040025F9 RID: 9721
	[Range(0f, 16f)]
	public FloatParameter scale = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040025FA RID: 9722
	public BoolParameter enableVignette = new BoolParameter
	{
		value = true
	};

	// Token: 0x040025FB RID: 9723
	[Range(0f, 100f)]
	public FloatParameter sharpness = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040025FC RID: 9724
	[Range(0f, 100f)]
	public FloatParameter darkness = new FloatParameter
	{
		value = 0f
	};
}
