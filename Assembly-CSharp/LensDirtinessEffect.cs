using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200063A RID: 1594
[PostProcess(typeof(LensDirtinessRenderer), PostProcessEvent.AfterStack, "Custom/LensDirtiness", true)]
[Serializable]
public class LensDirtinessEffect : PostProcessEffectSettings
{
	// Token: 0x0400261C RID: 9756
	public TextureParameter dirtinessTexture = new TextureParameter();

	// Token: 0x0400261D RID: 9757
	public BoolParameter sceneTintsBloom = new BoolParameter
	{
		value = false
	};

	// Token: 0x0400261E RID: 9758
	public FloatParameter gain = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x0400261F RID: 9759
	public FloatParameter threshold = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002620 RID: 9760
	public FloatParameter bloomSize = new FloatParameter
	{
		value = 5f
	};

	// Token: 0x04002621 RID: 9761
	public FloatParameter dirtiness = new FloatParameter
	{
		value = 1f
	};

	// Token: 0x04002622 RID: 9762
	public ColorParameter bloomColor = new ColorParameter
	{
		value = Color.white
	};
}
