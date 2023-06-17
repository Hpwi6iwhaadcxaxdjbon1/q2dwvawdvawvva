using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A66 RID: 2662
	[Preserve]
	internal sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
	{
		// Token: 0x06003F97 RID: 16279 RVA: 0x0002179A File Offset: 0x0001F99A
		public override DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
		}

		// Token: 0x06003F98 RID: 16280 RVA: 0x001770A0 File Offset: 0x001752A0
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			if (this.m_ResetHistory)
			{
				command.BlitFullscreenTriangle(context.source, context.destination, false, null);
				this.m_ResetHistory = false;
				return;
			}
			RenderTextureFormat format = RenderTextureFormat.RGHalf;
			RenderTextureFormat format2 = RenderTextureFormat.ARGB2101010.IsSupported() ? RenderTextureFormat.ARGB2101010 : RenderTextureFormat.ARGB32;
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
			command.BeginSample("MotionBlur");
			int num = (int)(5f * (float)context.height / 100f);
			int num2 = ((num - 1) / 8 + 1) * 8;
			float value = base.settings.shutterAngle / 360f;
			propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, value);
			propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, (float)num);
			propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float)num);
			int velocityTex = ShaderIDs.VelocityTex;
			command.GetTemporaryRT(velocityTex, context.width, context.height, 0, FilterMode.Point, format2, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, velocityTex, propertySheet, 0, false, null);
			int tile2RT = ShaderIDs.Tile2RT;
			command.GetTemporaryRT(tile2RT, context.width / 2, context.height / 2, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(velocityTex, tile2RT, propertySheet, 1, false, null);
			int tile4RT = ShaderIDs.Tile4RT;
			command.GetTemporaryRT(tile4RT, context.width / 4, context.height / 4, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile2RT, tile4RT, propertySheet, 2, false, null);
			command.ReleaseTemporaryRT(tile2RT);
			int tile8RT = ShaderIDs.Tile8RT;
			command.GetTemporaryRT(tile8RT, context.width / 8, context.height / 8, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile4RT, tile8RT, propertySheet, 2, false, null);
			command.ReleaseTemporaryRT(tile4RT);
			Vector2 v = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
			propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, v);
			propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (float)((int)((float)num2 / 8f)));
			int tileVRT = ShaderIDs.TileVRT;
			command.GetTemporaryRT(tileVRT, context.width / num2, context.height / num2, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tile8RT, tileVRT, propertySheet, 3, false, null);
			command.ReleaseTemporaryRT(tile8RT);
			int neighborMaxTex = ShaderIDs.NeighborMaxTex;
			int width = context.width / num2;
			int height = context.height / num2;
			command.GetTemporaryRT(neighborMaxTex, width, height, 0, FilterMode.Point, format, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(tileVRT, neighborMaxTex, propertySheet, 4, false, null);
			command.ReleaseTemporaryRT(tileVRT);
			propertySheet.properties.SetFloat(ShaderIDs.LoopCount, (float)Mathf.Clamp(base.settings.sampleCount / 2, 1, 64));
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5, false, null);
			command.ReleaseTemporaryRT(velocityTex);
			command.ReleaseTemporaryRT(neighborMaxTex);
			command.EndSample("MotionBlur");
		}

		// Token: 0x02000F23 RID: 3875
		private enum Pass
		{
			// Token: 0x04004E86 RID: 20102
			VelocitySetup,
			// Token: 0x04004E87 RID: 20103
			TileMax1,
			// Token: 0x04004E88 RID: 20104
			TileMax2,
			// Token: 0x04004E89 RID: 20105
			TileMaxV,
			// Token: 0x04004E8A RID: 20106
			NeighborMax,
			// Token: 0x04004E8B RID: 20107
			Reconstruction
		}
	}
}
