using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000631 RID: 1585
public class FrostRenderer : PostProcessEffectRenderer<Frost>
{
	// Token: 0x040025FD RID: 9725
	private int scaleProperty = Shader.PropertyToID("_scale");

	// Token: 0x040025FE RID: 9726
	private int sharpnessProperty = Shader.PropertyToID("_sharpness");

	// Token: 0x040025FF RID: 9727
	private int darknessProperty = Shader.PropertyToID("_darkness");

	// Token: 0x04002600 RID: 9728
	private Shader frostShader;

	// Token: 0x06002E5D RID: 11869 RVA: 0x001163B3 File Offset: 0x001145B3
	public override void Init()
	{
		base.Init();
		this.frostShader = Shader.Find("Hidden/PostProcessing/Frost");
	}

	// Token: 0x06002E5E RID: 11870 RVA: 0x001163CC File Offset: 0x001145CC
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Frost");
		PropertySheet propertySheet = context.propertySheets.Get(this.frostShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(this.scaleProperty, base.settings.scale.value);
		propertySheet.properties.SetFloat(this.sharpnessProperty, base.settings.sharpness.value * 0.01f);
		propertySheet.properties.SetFloat(this.darknessProperty, base.settings.darkness.value * 0.02f);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, base.settings.enableVignette.value ? 1 : 0, false, null);
		command.EndSample("Frost");
	}
}
