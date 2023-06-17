using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A68 RID: 2664
	[Preserve]
	[Serializable]
	internal sealed class ScalableAO : IAmbientOcclusionMethod
	{
		// Token: 0x040038F0 RID: 14576
		private RenderTexture m_Result;

		// Token: 0x040038F1 RID: 14577
		private PropertySheet m_PropertySheet;

		// Token: 0x040038F2 RID: 14578
		private AmbientOcclusion m_Settings;

		// Token: 0x040038F3 RID: 14579
		private readonly RenderTargetIdentifier[] m_MRT = new RenderTargetIdentifier[]
		{
			BuiltinRenderTextureType.GBuffer0,
			BuiltinRenderTextureType.CameraTarget
		};

		// Token: 0x040038F4 RID: 14580
		private readonly int[] m_SampleCount = new int[]
		{
			4,
			6,
			10,
			8,
			12
		};

		// Token: 0x06003FB1 RID: 16305 RVA: 0x0017862C File Offset: 0x0017682C
		public ScalableAO(AmbientOcclusion settings)
		{
			this.m_Settings = settings;
		}

		// Token: 0x06003FB2 RID: 16306 RVA: 0x000037BE File Offset: 0x000019BE
		public DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
		}

		// Token: 0x06003FB3 RID: 16307 RVA: 0x00178684 File Offset: 0x00176884
		private void DoLazyInitialization(PostProcessRenderContext context)
		{
			this.m_PropertySheet = context.propertySheets.Get(context.resources.shaders.scalableAO);
			bool flag = false;
			if (this.m_Result == null || !this.m_Result.IsCreated())
			{
				this.m_Result = context.GetScreenSpaceTemporaryRT(0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, 0, 0);
				this.m_Result.hideFlags = HideFlags.DontSave;
				this.m_Result.filterMode = FilterMode.Bilinear;
				flag = true;
			}
			else if (this.m_Result.width != context.width || this.m_Result.height != context.height)
			{
				this.m_Result.Release();
				this.m_Result.width = context.width;
				this.m_Result.height = context.height;
				flag = true;
			}
			if (flag)
			{
				this.m_Result.Create();
			}
		}

		// Token: 0x06003FB4 RID: 16308 RVA: 0x00178764 File Offset: 0x00176964
		private void Render(PostProcessRenderContext context, CommandBuffer cmd, int occlusionSource)
		{
			this.DoLazyInitialization(context);
			this.m_Settings.radius.value = Mathf.Max(this.m_Settings.radius.value, 0.0001f);
			bool flag = this.m_Settings.quality.value < AmbientOcclusionQuality.High;
			float value = this.m_Settings.intensity.value;
			float value2 = this.m_Settings.radius.value;
			float z = flag ? 0.5f : 1f;
			float w = (float)this.m_SampleCount[(int)this.m_Settings.quality.value];
			PropertySheet propertySheet = this.m_PropertySheet;
			propertySheet.ClearKeywords();
			propertySheet.properties.SetVector(ShaderIDs.AOParams, new Vector4(value, value2, z, w));
			propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.white - this.m_Settings.color.value);
			if (context.camera.actualRenderingPath == RenderingPath.Forward && RenderSettings.fog)
			{
				propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
				propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			}
			int num = flag ? 2 : 1;
			int occlusionTexture = ShaderIDs.OcclusionTexture1;
			int widthOverride = context.width / num;
			int heightOverride = context.height / num;
			context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Bilinear, widthOverride, heightOverride);
			cmd.BlitFullscreenTriangle(BuiltinRenderTextureType.None, occlusionTexture, propertySheet, occlusionSource, false, null);
			int occlusionTexture2 = ShaderIDs.OcclusionTexture2;
			context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture2, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, FilterMode.Bilinear, 0, 0);
			cmd.BlitFullscreenTriangle(occlusionTexture, occlusionTexture2, propertySheet, 2 + occlusionSource, false, null);
			cmd.ReleaseTemporaryRT(occlusionTexture);
			cmd.BlitFullscreenTriangle(occlusionTexture2, this.m_Result, propertySheet, 4, false, null);
			cmd.ReleaseTemporaryRT(occlusionTexture2);
			if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
			{
				context.PushDebugOverlay(cmd, this.m_Result, propertySheet, 7);
			}
		}

		// Token: 0x06003FB5 RID: 16309 RVA: 0x0017898C File Offset: 0x00176B8C
		public void RenderAfterOpaque(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion");
			this.Render(context, command, 0);
			command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, this.m_Result);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 5, RenderBufferLoadAction.Load, null);
			command.EndSample("Ambient Occlusion");
		}

		// Token: 0x06003FB6 RID: 16310 RVA: 0x001789F8 File Offset: 0x00176BF8
		public void RenderAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Render");
			this.Render(context, command, 1);
			command.EndSample("Ambient Occlusion Render");
		}

		// Token: 0x06003FB7 RID: 16311 RVA: 0x00178A2C File Offset: 0x00176C2C
		public void CompositeAmbientOnly(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("Ambient Occlusion Composite");
			command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, this.m_Result);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, this.m_MRT, BuiltinRenderTextureType.CameraTarget, this.m_PropertySheet, 6, false, null);
			command.EndSample("Ambient Occlusion Composite");
		}

		// Token: 0x06003FB8 RID: 16312 RVA: 0x00178A93 File Offset: 0x00176C93
		public void Release()
		{
			RuntimeUtilities.Destroy(this.m_Result);
			this.m_Result = null;
		}

		// Token: 0x02000F26 RID: 3878
		private enum Pass
		{
			// Token: 0x04004E9A RID: 20122
			OcclusionEstimationForward,
			// Token: 0x04004E9B RID: 20123
			OcclusionEstimationDeferred,
			// Token: 0x04004E9C RID: 20124
			HorizontalBlurForward,
			// Token: 0x04004E9D RID: 20125
			HorizontalBlurDeferred,
			// Token: 0x04004E9E RID: 20126
			VerticalBlur,
			// Token: 0x04004E9F RID: 20127
			CompositionForward,
			// Token: 0x04004EA0 RID: 20128
			CompositionDeferred,
			// Token: 0x04004EA1 RID: 20129
			DebugOverlay
		}
	}
}
