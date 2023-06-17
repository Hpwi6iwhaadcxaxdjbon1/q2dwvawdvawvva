using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000645 RID: 1605
public class WiggleRenderer : PostProcessEffectRenderer<Wiggle>
{
	// Token: 0x0400263F RID: 9791
	private int timerProperty = Shader.PropertyToID("_timer");

	// Token: 0x04002640 RID: 9792
	private int scaleProperty = Shader.PropertyToID("_scale");

	// Token: 0x04002641 RID: 9793
	private Shader wiggleShader;

	// Token: 0x04002642 RID: 9794
	private float timer;

	// Token: 0x06002E7E RID: 11902 RVA: 0x00117820 File Offset: 0x00115A20
	public override void Init()
	{
		base.Init();
		this.wiggleShader = Shader.Find("Hidden/PostProcessing/Wiggle");
	}

	// Token: 0x06002E7F RID: 11903 RVA: 0x00117838 File Offset: 0x00115A38
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Wiggle");
		this.timer += base.settings.speed.value * Time.deltaTime;
		PropertySheet propertySheet = context.propertySheets.Get(this.wiggleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat(this.timerProperty, this.timer);
		propertySheet.properties.SetFloat(this.scaleProperty, base.settings.scale.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("Wiggle");
	}
}
