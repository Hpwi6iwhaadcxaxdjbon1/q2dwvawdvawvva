using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000638 RID: 1592
[PostProcess(typeof(GreyScaleRenderer), PostProcessEvent.AfterStack, "Custom/GreyScale", true)]
[Serializable]
public class GreyScale : PostProcessEffectSettings
{
	// Token: 0x04002614 RID: 9748
	[Range(0f, 1f)]
	public FloatParameter redLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002615 RID: 9749
	[Range(0f, 1f)]
	public FloatParameter greenLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002616 RID: 9750
	[Range(0f, 1f)]
	public FloatParameter blueLuminance = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002617 RID: 9751
	[Range(0f, 1f)]
	public FloatParameter amount = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002618 RID: 9752
	[ColorUsage(false, true)]
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};
}
