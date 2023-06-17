using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062D RID: 1581
public class DoubleVisionRenderer : PostProcessEffectRenderer<DoubleVision>
{
	// Token: 0x040025F1 RID: 9713
	private int displaceProperty = Shader.PropertyToID("_displace");

	// Token: 0x040025F2 RID: 9714
	private int amountProperty = Shader.PropertyToID("_amount");

	// Token: 0x040025F3 RID: 9715
	private Shader doubleVisionShader;

	// Token: 0x06002E52 RID: 11858 RVA: 0x0011605E File Offset: 0x0011425E
	public override void Init()
	{
		base.Init();
		this.doubleVisionShader = Shader.Find("Hidden/PostProcessing/DoubleVision");
	}

	// Token: 0x06002E53 RID: 11859 RVA: 0x00116078 File Offset: 0x00114278
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("DoubleVision");
		PropertySheet propertySheet = context.propertySheets.Get(this.doubleVisionShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(this.displaceProperty, base.settings.displace.value);
		propertySheet.properties.SetFloat(this.amountProperty, base.settings.amount.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("DoubleVision");
	}
}
