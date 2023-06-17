using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200063D RID: 1597
public class PhotoFilterRenderer : PostProcessEffectRenderer<PhotoFilter>
{
	// Token: 0x04002627 RID: 9767
	private int rgbProperty = Shader.PropertyToID("_rgb");

	// Token: 0x04002628 RID: 9768
	private int densityProperty = Shader.PropertyToID("_density");

	// Token: 0x04002629 RID: 9769
	private Shader greyScaleShader;

	// Token: 0x06002E71 RID: 11889 RVA: 0x0011726A File Offset: 0x0011546A
	public override void Init()
	{
		base.Init();
		this.greyScaleShader = Shader.Find("Hidden/PostProcessing/PhotoFilter");
	}

	// Token: 0x06002E72 RID: 11890 RVA: 0x00117284 File Offset: 0x00115484
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("PhotoFilter");
		PropertySheet propertySheet = context.propertySheets.Get(this.greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetColor(this.rgbProperty, base.settings.color.value);
		propertySheet.properties.SetFloat(this.densityProperty, base.settings.density.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("PhotoFilter");
	}
}
