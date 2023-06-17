using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000639 RID: 1593
public class GreyScaleRenderer : PostProcessEffectRenderer<GreyScale>
{
	// Token: 0x04002619 RID: 9753
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x0400261A RID: 9754
	private int colorProperty = Shader.PropertyToID("_color");

	// Token: 0x0400261B RID: 9755
	private Shader greyScaleShader;

	// Token: 0x06002E69 RID: 11881 RVA: 0x00116BF1 File Offset: 0x00114DF1
	public override void Init()
	{
		base.Init();
		this.greyScaleShader = Shader.Find("Hidden/PostProcessing/GreyScale");
	}

	// Token: 0x06002E6A RID: 11882 RVA: 0x00116C0C File Offset: 0x00114E0C
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("GreyScale");
		PropertySheet propertySheet = context.propertySheets.Get(this.greyScaleShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetVector(this.dataProperty, new Vector4(base.settings.redLuminance.value, base.settings.greenLuminance.value, base.settings.blueLuminance.value, base.settings.amount.value));
		propertySheet.properties.SetColor(this.colorProperty, base.settings.color.value);
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("GreyScale");
	}
}
