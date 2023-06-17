using System;
using ConVar;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062B RID: 1579
public class DepthOfFieldEffectRenderer : PostProcessEffectRenderer<DepthOfFieldEffect>
{
	// Token: 0x040025EC RID: 9708
	private float focalDistance01 = 10f;

	// Token: 0x040025ED RID: 9709
	private float internalBlurWidth = 1f;

	// Token: 0x040025EE RID: 9710
	private Shader dofShader;

	// Token: 0x06002E4C RID: 11852 RVA: 0x00115C2B File Offset: 0x00113E2B
	public override void Init()
	{
		this.dofShader = Shader.Find("Hidden/PostProcessing/DepthOfFieldEffect");
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x00115C40 File Offset: 0x00113E40
	private float FocalDistance01(Camera cam, float worldDist)
	{
		return cam.WorldToViewportPoint((worldDist - cam.nearClipPlane) * cam.transform.forward + cam.transform.position).z / (cam.farClipPlane - cam.nearClipPlane);
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x00115C90 File Offset: 0x00113E90
	private void WriteCoc(PostProcessRenderContext context, PropertySheet sheet)
	{
		CommandBuffer command = context.command;
		RenderTargetIdentifier source = context.source;
		RenderTextureFormat sourceFormat = context.sourceFormat;
		float num = 1f;
		int width = context.width / 2;
		int height = context.height / 2;
		int nameID = Shader.PropertyToID("DOFtemp1");
		int nameID2 = Shader.PropertyToID("DOFtemp2");
		command.GetTemporaryRT(nameID2, width, height, 0, FilterMode.Bilinear, sourceFormat);
		command.BlitFullscreenTriangle(source, nameID2, sheet, 1, false, null);
		float num2 = this.internalBlurWidth * num;
		sheet.properties.SetVector("_Offsets", new Vector4(0f, num2, 0f, num2));
		command.GetTemporaryRT(nameID, width, height, 0, FilterMode.Bilinear, sourceFormat);
		command.BlitFullscreenTriangle(nameID2, nameID, sheet, 0, false, null);
		command.ReleaseTemporaryRT(nameID2);
		sheet.properties.SetVector("_Offsets", new Vector4(num2, 0f, 0f, num2));
		command.GetTemporaryRT(nameID2, width, height, 0, FilterMode.Bilinear, sourceFormat);
		command.BlitFullscreenTriangle(nameID, nameID2, sheet, 0, false, null);
		command.ReleaseTemporaryRT(nameID);
		command.SetGlobalTexture("_FgOverlap", nameID2);
		command.BlitFullscreenTriangle(source, source, sheet, 3, RenderBufferLoadAction.Load, null);
		command.ReleaseTemporaryRT(nameID2);
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x00115DF8 File Offset: 0x00113FF8
	public override void Render(PostProcessRenderContext context)
	{
		PropertySheet propertySheet = context.propertySheets.Get(this.dofShader);
		CommandBuffer command = context.command;
		int width = context.width;
		int height = context.height;
		RenderTextureFormat sourceFormat = context.sourceFormat;
		bool value = base.settings.highResolution.value;
		DOFBlurSampleCountParameter blurSampleCount = base.settings.blurSampleCount;
		float num = base.settings.focalSize.value;
		float value2 = base.settings.focalLength.value;
		float num2 = base.settings.aperture.value;
		float num3 = base.settings.maxBlurSize.value;
		int nameID = Shader.PropertyToID("DOFrtLow");
		int nameID2 = Shader.PropertyToID("DOFrtLow2");
		num2 = Math.Max(num2, 0f);
		num3 = Math.Max(num3, 0.1f);
		num = Mathf.Clamp(num, 0f, 2f);
		this.internalBlurWidth = Mathf.Max(num3, 0f);
		this.focalDistance01 = this.FocalDistance01(context.camera, value2);
		propertySheet.properties.SetVector("_CurveParams", new Vector4(1f, num, num2 / 10f, this.focalDistance01));
		if (value)
		{
			this.internalBlurWidth *= 2f;
		}
		this.WriteCoc(context, propertySheet);
		if (ConVar.Graphics.dof_debug)
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5, false, null);
			return;
		}
		command.GetTemporaryRT(nameID, width >> 1, height >> 1, 0, FilterMode.Bilinear, sourceFormat);
		command.GetTemporaryRT(nameID2, width >> 1, height >> 1, 0, FilterMode.Bilinear, sourceFormat);
		int pass = 2;
		propertySheet.properties.SetVector("_Offsets", new Vector4(0f, this.internalBlurWidth, 0.025f, this.internalBlurWidth));
		propertySheet.properties.SetInt("_BlurCountMode", (int)blurSampleCount.value);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, pass, false, null);
		command.ReleaseTemporaryRT(nameID);
		command.ReleaseTemporaryRT(nameID2);
	}
}
