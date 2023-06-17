using System;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000640 RID: 1600
[PostProcess(typeof(ScreenOverlayRenderer), PostProcessEvent.AfterStack, "Custom/ScreenOverlay", true)]
[Serializable]
public class ScreenOverlay : PostProcessEffectSettings
{
	// Token: 0x04002631 RID: 9777
	public OverlayBlendModeParameter blendMode = new OverlayBlendModeParameter
	{
		value = OverlayBlendMode.Multiply
	};

	// Token: 0x04002632 RID: 9778
	public FloatParameter intensity = new FloatParameter
	{
		value = 0f
	};

	// Token: 0x04002633 RID: 9779
	public TextureParameter texture = new TextureParameter
	{
		value = null
	};

	// Token: 0x04002634 RID: 9780
	public TextureParameter normals = new TextureParameter
	{
		value = null
	};
}
