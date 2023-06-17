using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062E RID: 1582
[PostProcess(typeof(FlashbangEffectRenderer), PostProcessEvent.AfterStack, "Custom/FlashbangEffect", false)]
[Serializable]
public class FlashbangEffect : PostProcessEffectSettings
{
	// Token: 0x040025F4 RID: 9716
	[Range(0f, 1f)]
	public FloatParameter burnIntensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x040025F5 RID: 9717
	[Range(0f, 1f)]
	public FloatParameter whiteoutIntensity = new FloatParameter
	{
		value = 0f
	};
}
