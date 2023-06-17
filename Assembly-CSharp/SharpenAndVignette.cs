using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000642 RID: 1602
[PostProcess(typeof(SharpenAndVignetteRenderer), PostProcessEvent.AfterStack, "Custom/SharpenAndVignette", true)]
[Serializable]
public class SharpenAndVignette : PostProcessEffectSettings
{
	// Token: 0x04002636 RID: 9782
	[Header("Sharpen")]
	public BoolParameter applySharpen = new BoolParameter
	{
		value = true
	};

	// Token: 0x04002637 RID: 9783
	[Range(0f, 5f)]
	public FloatParameter strength = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002638 RID: 9784
	[Range(0f, 1f)]
	public FloatParameter clamp = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002639 RID: 9785
	[Header("Vignette")]
	public BoolParameter applyVignette = new BoolParameter
	{
		value = true
	};

	// Token: 0x0400263A RID: 9786
	[Range(-100f, 100f)]
	public FloatParameter sharpness = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x0400263B RID: 9787
	[Range(0f, 100f)]
	public FloatParameter darkness = new FloatParameter
	{
		value = 0f
	};
}
