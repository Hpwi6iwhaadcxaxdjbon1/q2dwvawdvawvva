using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062C RID: 1580
[PostProcess(typeof(DoubleVisionRenderer), PostProcessEvent.AfterStack, "Custom/DoubleVision", true)]
[Serializable]
public class DoubleVision : PostProcessEffectSettings
{
	// Token: 0x040025EF RID: 9711
	[Range(0f, 1f)]
	public Vector2Parameter displace = new Vector2Parameter
	{
		value = Vector2.zero
	};

	// Token: 0x040025F0 RID: 9712
	[Range(0f, 1f)]
	public FloatParameter amount = new FloatParameter
	{
		value = 0f
	};
}
