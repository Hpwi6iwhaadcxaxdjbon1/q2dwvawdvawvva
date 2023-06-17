using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200062F RID: 1583
public class FlashbangEffectRenderer : PostProcessEffectRenderer<FlashbangEffect>
{
	// Token: 0x040025F6 RID: 9718
	public static bool needsCapture;

	// Token: 0x040025F7 RID: 9719
	private Shader flashbangEffectShader;

	// Token: 0x040025F8 RID: 9720
	private RenderTexture screenRT;

	// Token: 0x06002E56 RID: 11862 RVA: 0x0011617E File Offset: 0x0011437E
	public override void Init()
	{
		base.Init();
		this.flashbangEffectShader = Shader.Find("Hidden/PostProcessing/FlashbangEffect");
	}

	// Token: 0x06002E57 RID: 11863 RVA: 0x00116198 File Offset: 0x00114398
	public override void Render(PostProcessRenderContext context)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		CommandBuffer command = context.command;
		FlashbangEffectRenderer.CheckCreateRenderTexture(ref this.screenRT, "Flashbang", context.width, context.height, context.sourceFormat);
		command.BeginSample("FlashbangEffect");
		if (FlashbangEffectRenderer.needsCapture)
		{
			command.CopyTexture(context.source, this.screenRT);
			FlashbangEffectRenderer.needsCapture = false;
		}
		PropertySheet propertySheet = context.propertySheets.Get(this.flashbangEffectShader);
		propertySheet.properties.Clear();
		propertySheet.properties.SetFloat("_BurnIntensity", base.settings.burnIntensity.value);
		propertySheet.properties.SetFloat("_WhiteoutIntensity", base.settings.whiteoutIntensity.value);
		if (this.screenRT)
		{
			propertySheet.properties.SetTexture("_BurnOverlay", this.screenRT);
		}
		context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0, false, null);
		command.EndSample("FlashbangEffect");
	}

	// Token: 0x06002E58 RID: 11864 RVA: 0x001162B3 File Offset: 0x001144B3
	public override void Release()
	{
		base.Release();
		FlashbangEffectRenderer.SafeDestroyRenderTexture(ref this.screenRT);
	}

	// Token: 0x06002E59 RID: 11865 RVA: 0x001162C8 File Offset: 0x001144C8
	private static void CheckCreateRenderTexture(ref RenderTexture rt, string name, int width, int height, RenderTextureFormat format)
	{
		if (rt == null || rt.width != width || rt.height != height)
		{
			FlashbangEffectRenderer.SafeDestroyRenderTexture(ref rt);
			rt = new RenderTexture(width, height, 0, format)
			{
				hideFlags = HideFlags.DontSave
			};
			rt.name = name;
			rt.wrapMode = TextureWrapMode.Clamp;
			rt.Create();
		}
	}

	// Token: 0x06002E5A RID: 11866 RVA: 0x00116325 File Offset: 0x00114525
	private static void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if (rt != null)
		{
			rt.Release();
			UnityEngine.Object.DestroyImmediate(rt);
			rt = null;
		}
	}
}
