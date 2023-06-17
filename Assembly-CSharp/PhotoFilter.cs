using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200063C RID: 1596
[PostProcess(typeof(PhotoFilterRenderer), PostProcessEvent.AfterStack, "Custom/PhotoFilter", true)]
[Serializable]
public class PhotoFilter : PostProcessEffectSettings
{
	// Token: 0x04002625 RID: 9765
	public ColorParameter color = new ColorParameter
	{
		value = Color.white
	};

	// Token: 0x04002626 RID: 9766
	[Range(0f, 1f)]
	public FloatParameter density = new FloatParameter
	{
		value = 0f
	};
}
