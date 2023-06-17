using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000643 RID: 1603
public class SharpenAndVignetteRenderer : PostProcessEffectRenderer<SharpenAndVignette>
{
	// Token: 0x0400263C RID: 9788
	private Shader sharpenAndVigenetteShader;

	// Token: 0x06002E7A RID: 11898 RVA: 0x00117603 File Offset: 0x00115803
	public override void Init()
	{
		base.Init();
		this.sharpenAndVigenetteShader = Shader.Find("Hidden/PostProcessing/SharpenAndVignette");
	}

	// Token: 0x06002E7B RID: 11899 RVA: 0x0011761C File Offset: 0x0011581C
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("SharpenAndVignette");
		PropertySheet propertySheet = context.propertySheets.Get(this.sharpenAndVigenetteShader);
		propertySheet.properties.Clear();
		bool value = base.settings.applySharpen.value;
		bool value2 = base.settings.applyVignette.value;
		if (value)
		{
			propertySheet.properties.SetFloat("_px", 1f / (float)Screen.width);
			propertySheet.properties.SetFloat("_py", 1f / (float)Screen.height);
			propertySheet.properties.SetFloat("_strength", base.settings.strength.value);
			propertySheet.properties.SetFloat("_clamp", base.settings.clamp.value);
		}
		if (value2)
		{
			propertySheet.properties.SetFloat("_sharpness", base.settings.sharpness.value * 0.01f);
			propertySheet.properties.SetFloat("_darkness", base.settings.darkness.value * 0.02f);
		}
		if (value && !value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		else if (value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1, false, null);
		}
		else if (!value && value2)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 2, false, null);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		}
		command.EndSample("SharpenAndVignette");
	}
}
