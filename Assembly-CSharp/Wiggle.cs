using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000644 RID: 1604
[PostProcess(typeof(WiggleRenderer), PostProcessEvent.AfterStack, "Custom/Wiggle", true)]
[Serializable]
public class Wiggle : PostProcessEffectSettings
{
	// Token: 0x0400263D RID: 9789
	public FloatParameter speed = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x0400263E RID: 9790
	public FloatParameter scale = new FloatParameter
	{
		value = 12f
	};
}
