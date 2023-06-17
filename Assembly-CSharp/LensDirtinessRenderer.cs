using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200063B RID: 1595
public class LensDirtinessRenderer : PostProcessEffectRenderer<LensDirtinessEffect>
{
	// Token: 0x04002623 RID: 9763
	private int dataProperty = Shader.PropertyToID("_data");

	// Token: 0x04002624 RID: 9764
	private Shader lensDirtinessShader;

	// Token: 0x06002E6D RID: 11885 RVA: 0x00116DB2 File Offset: 0x00114FB2
	public override void Init()
	{
		base.Init();
		this.lensDirtinessShader = Shader.Find("Hidden/PostProcessing/LensDirtiness");
	}

	// Token: 0x06002E6E RID: 11886 RVA: 0x00116DCC File Offset: 0x00114FCC
	public override void Render(PostProcessRenderContext context)
	{
		float value = base.settings.bloomSize.value;
		float value2 = base.settings.gain.value;
		float value3 = base.settings.threshold.value;
		float value4 = base.settings.dirtiness.value;
		Color value5 = base.settings.bloomColor.value;
		Texture value6 = base.settings.dirtinessTexture.value;
		bool value7 = base.settings.sceneTintsBloom.value;
		CommandBuffer command = context.command;
		command.BeginSample("LensDirtinessEffect");
		if (value7)
		{
			command.EnableShaderKeyword("_SCENE_TINTS_BLOOM");
		}
		PropertySheet propertySheet = context.propertySheets.Get(this.lensDirtinessShader);
		RenderTargetIdentifier source = context.source;
		RenderTargetIdentifier destination = context.destination;
		int width = context.width;
		int height = context.height;
		int nameID = Shader.PropertyToID("_RTT_BloomThreshold");
		int nameID2 = Shader.PropertyToID("_RTT_1");
		int nameID3 = Shader.PropertyToID("_RTT_2");
		int nameID4 = Shader.PropertyToID("_RTT_3");
		int nameID5 = Shader.PropertyToID("_RTT_4");
		int nameID6 = Shader.PropertyToID("_RTT_Bloom_1");
		int nameID7 = Shader.PropertyToID("_RTT_Bloom_2");
		propertySheet.properties.SetFloat("_Gain", value2);
		propertySheet.properties.SetFloat("_Threshold", value3);
		command.GetTemporaryRT(nameID, width / 2, height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(source, nameID, propertySheet, 0, false, null);
		propertySheet.properties.SetVector("_Offset", new Vector4(1f / (float)width, 1f / (float)height, 0f, 0f) * 2f);
		command.GetTemporaryRT(nameID2, width / 2, height / 2, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(nameID, nameID2, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(nameID);
		command.GetTemporaryRT(nameID3, width / 4, height / 4, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(nameID2, nameID3, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(nameID2);
		command.GetTemporaryRT(nameID4, width / 8, height / 8, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(nameID3, nameID4, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(nameID3);
		command.GetTemporaryRT(nameID5, width / 16, height / 16, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(nameID4, nameID5, propertySheet, 1, false, null);
		command.ReleaseTemporaryRT(nameID4);
		command.GetTemporaryRT(nameID6, width / 16, height / 16, 0, FilterMode.Bilinear, context.sourceFormat);
		command.GetTemporaryRT(nameID7, width / 16, height / 16, 0, FilterMode.Bilinear, context.sourceFormat);
		command.BlitFullscreenTriangle(nameID5, nameID6, false, null);
		command.ReleaseTemporaryRT(nameID5);
		for (int i = 1; i <= 8; i++)
		{
			float x = value * (float)i / (float)width;
			float y = value * (float)i / (float)height;
			propertySheet.properties.SetVector("_Offset", new Vector4(x, y, 0f, 0f));
			command.BlitFullscreenTriangle(nameID6, nameID7, propertySheet, 1, false, null);
			command.BlitFullscreenTriangle(nameID7, nameID6, propertySheet, 1, false, null);
		}
		command.SetGlobalTexture("_Bloom", nameID7);
		propertySheet.properties.SetFloat("_Dirtiness", value4);
		propertySheet.properties.SetColor("_BloomColor", value5);
		propertySheet.properties.SetTexture("_DirtinessTexture", value6);
		command.BlitFullscreenTriangle(source, destination, propertySheet, 2, false, null);
		command.ReleaseTemporaryRT(nameID6);
		command.ReleaseTemporaryRT(nameID7);
		command.EndSample("LensDirtinessEffect");
	}

	// Token: 0x02000D96 RID: 3478
	private enum Pass
	{
		// Token: 0x0400481A RID: 18458
		Threshold,
		// Token: 0x0400481B RID: 18459
		Kawase,
		// Token: 0x0400481C RID: 18460
		Compose
	}
}
