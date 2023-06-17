using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A5D RID: 2653
	[Preserve]
	internal sealed class DepthOfFieldRenderer : PostProcessEffectRenderer<DepthOfField>
	{
		// Token: 0x040038CD RID: 14541
		private const int k_NumEyes = 2;

		// Token: 0x040038CE RID: 14542
		private const int k_NumCoCHistoryTextures = 2;

		// Token: 0x040038CF RID: 14543
		private readonly RenderTexture[][] m_CoCHistoryTextures = new RenderTexture[2][];

		// Token: 0x040038D0 RID: 14544
		private int[] m_HistoryPingPong = new int[2];

		// Token: 0x040038D1 RID: 14545
		private const float k_FilmHeight = 0.024f;

		// Token: 0x06003F7D RID: 16253 RVA: 0x00176384 File Offset: 0x00174584
		public DepthOfFieldRenderer()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_CoCHistoryTextures[i] = new RenderTexture[2];
				this.m_HistoryPingPong[i] = 0;
			}
		}

		// Token: 0x06003F7E RID: 16254 RVA: 0x0000441C File Offset: 0x0000261C
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003F7F RID: 16255 RVA: 0x001763D2 File Offset: 0x001745D2
		private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
		{
			if (primary.IsSupported())
			{
				return primary;
			}
			if (secondary.IsSupported())
			{
				return secondary;
			}
			return RenderTextureFormat.Default;
		}

		// Token: 0x06003F80 RID: 16256 RVA: 0x001763EC File Offset: 0x001745EC
		private float CalculateMaxCoCRadius(int screenHeight)
		{
			float num = (float)base.settings.kernelSize.value * 4f + 6f;
			return Mathf.Min(0.05f, num / (float)screenHeight);
		}

		// Token: 0x06003F81 RID: 16257 RVA: 0x00176428 File Offset: 0x00174628
		private RenderTexture CheckHistory(int eye, int id, PostProcessRenderContext context, RenderTextureFormat format)
		{
			RenderTexture renderTexture = this.m_CoCHistoryTextures[eye][id];
			if (this.m_ResetHistory || renderTexture == null || !renderTexture.IsCreated() || renderTexture.width != context.width || renderTexture.height != context.height)
			{
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = context.GetScreenSpaceTemporaryRT(0, format, RenderTextureReadWrite.Linear, 0, 0);
				renderTexture.name = string.Concat(new object[]
				{
					"CoC History, Eye: ",
					eye,
					", ID: ",
					id
				});
				renderTexture.filterMode = FilterMode.Bilinear;
				renderTexture.Create();
				this.m_CoCHistoryTextures[eye][id] = renderTexture;
			}
			return renderTexture;
		}

		// Token: 0x06003F82 RID: 16258 RVA: 0x001764D8 File Offset: 0x001746D8
		public override void Render(PostProcessRenderContext context)
		{
			RenderTextureFormat sourceFormat = context.sourceFormat;
			RenderTextureFormat renderTextureFormat = this.SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);
			float num = 0.024f * ((float)context.height / 1080f);
			float num2 = base.settings.focalLength.value / 1000f;
			float num3 = Mathf.Max(base.settings.focusDistance.value, num2);
			float num4 = (float)context.screenWidth / (float)context.screenHeight;
			float value = num2 * num2 / (base.settings.aperture.value * (num3 - num2) * num * 2f);
			float num5 = this.CalculateMaxCoCRadius(context.screenHeight);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.depthOfField);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat(ShaderIDs.Distance, num3);
			propertySheet.properties.SetFloat(ShaderIDs.LensCoeff, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxCoC, num5);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxCoC, 1f / num5);
			propertySheet.properties.SetFloat(ShaderIDs.RcpAspect, 1f / num4);
			CommandBuffer command = context.command;
			command.BeginSample("DepthOfField");
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.CoCTex, 0, renderTextureFormat, RenderTextureReadWrite.Linear, FilterMode.Bilinear, 0, 0);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, ShaderIDs.CoCTex, propertySheet, 0, false, null);
			if (context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				float motionBlending = context.temporalAntialiasing.motionBlending;
				float z = this.m_ResetHistory ? 0f : motionBlending;
				Vector2 jitter = context.temporalAntialiasing.jitter;
				propertySheet.properties.SetVector(ShaderIDs.TaaParams, new Vector3(jitter.x, jitter.y, z));
				int num6 = this.m_HistoryPingPong[context.xrActiveEye];
				RenderTexture tex = this.CheckHistory(context.xrActiveEye, ++num6 % 2, context, renderTextureFormat);
				RenderTexture tex2 = this.CheckHistory(context.xrActiveEye, ++num6 % 2, context, renderTextureFormat);
				this.m_HistoryPingPong[context.xrActiveEye] = (num6 + 1) % 2;
				command.BlitFullscreenTriangle(tex, tex2, propertySheet, 1, false, null);
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
				command.SetGlobalTexture(ShaderIDs.CoCTex, tex2);
			}
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTex, 0, sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.DepthOfFieldTex, propertySheet, 2, false, null);
			context.GetScreenSpaceTemporaryRT(command, ShaderIDs.DepthOfFieldTemp, 0, sourceFormat, RenderTextureReadWrite.Default, FilterMode.Bilinear, context.width / 2, context.height / 2);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTex, ShaderIDs.DepthOfFieldTemp, propertySheet, (int)(3 + base.settings.kernelSize.value), false, null);
			command.BlitFullscreenTriangle(ShaderIDs.DepthOfFieldTemp, ShaderIDs.DepthOfFieldTex, propertySheet, 7, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTemp);
			if (context.IsDebugOverlayEnabled(DebugOverlay.DepthOfField))
			{
				context.PushDebugOverlay(command, context.source, propertySheet, 9);
			}
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 8, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.DepthOfFieldTex);
			if (!context.IsTemporalAntialiasingActive() || context.dlssEnabled)
			{
				command.ReleaseTemporaryRT(ShaderIDs.CoCTex);
			}
			command.EndSample("DepthOfField");
			this.m_ResetHistory = false;
		}

		// Token: 0x06003F83 RID: 16259 RVA: 0x001768B0 File Offset: 0x00174AB0
		public override void Release()
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < this.m_CoCHistoryTextures[i].Length; j++)
				{
					RenderTexture.ReleaseTemporary(this.m_CoCHistoryTextures[i][j]);
					this.m_CoCHistoryTextures[i][j] = null;
				}
				this.m_HistoryPingPong[i] = 0;
			}
			this.ResetHistory();
		}

		// Token: 0x02000F22 RID: 3874
		private enum Pass
		{
			// Token: 0x04004E7B RID: 20091
			CoCCalculation,
			// Token: 0x04004E7C RID: 20092
			CoCTemporalFilter,
			// Token: 0x04004E7D RID: 20093
			DownsampleAndPrefilter,
			// Token: 0x04004E7E RID: 20094
			BokehSmallKernel,
			// Token: 0x04004E7F RID: 20095
			BokehMediumKernel,
			// Token: 0x04004E80 RID: 20096
			BokehLargeKernel,
			// Token: 0x04004E81 RID: 20097
			BokehVeryLargeKernel,
			// Token: 0x04004E82 RID: 20098
			PostFilter,
			// Token: 0x04004E83 RID: 20099
			Combine,
			// Token: 0x04004E84 RID: 20100
			DebugOverlay
		}
	}
}
