using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000641 RID: 1601
public class ScreenOverlayRenderer : PostProcessEffectRenderer<ScreenOverlay>
{
	// Token: 0x04002635 RID: 9781
	private Shader overlayShader;

	// Token: 0x06002E76 RID: 11894 RVA: 0x001173BB File Offset: 0x001155BB
	public override void Init()
	{
		base.Init();
		this.overlayShader = Shader.Find("Hidden/PostProcessing/ScreenOverlay");
	}

	// Token: 0x06002E77 RID: 11895 RVA: 0x001173D4 File Offset: 0x001155D4
	public override void Render(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("ScreenOverlay");
		PropertySheet propertySheet = context.propertySheets.Get(this.overlayShader);
		propertySheet.properties.Clear();
		Vector4 value = new Vector4(1f, 0f, 0f, 1f);
		propertySheet.properties.SetVector("_UV_Transform", value);
		propertySheet.properties.SetFloat("_Intensity", base.settings.intensity);
		if (TOD_Sky.Instance)
		{
			propertySheet.properties.SetVector("_LightDir", context.camera.transform.InverseTransformDirection(TOD_Sky.Instance.LightDirection));
			propertySheet.properties.SetColor("_LightCol", TOD_Sky.Instance.LightColor * TOD_Sky.Instance.LightIntensity);
		}
		if (base.settings.texture.value)
		{
			propertySheet.properties.SetTexture("_Overlay", base.settings.texture.value);
		}
		if (base.settings.normals.value)
		{
			propertySheet.properties.SetTexture("_Normals", base.settings.normals.value);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, (int)base.settings.blendMode.value, false, null);
		command.EndSample("ScreenOverlay");
	}
}
