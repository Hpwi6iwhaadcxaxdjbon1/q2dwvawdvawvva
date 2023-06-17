using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A6F RID: 2671
	[Preserve]
	[Serializable]
	public sealed class SubpixelMorphologicalAntialiasing
	{
		// Token: 0x0400390D RID: 14605
		[Tooltip("Lower quality is faster at the expense of visual quality (Low = ~60%, Medium = ~80%).")]
		public SubpixelMorphologicalAntialiasing.Quality quality = SubpixelMorphologicalAntialiasing.Quality.High;

		// Token: 0x06003FC2 RID: 16322 RVA: 0x001793E8 File Offset: 0x001775E8
		public bool IsSupported()
		{
			return !RuntimeUtilities.isSinglePassStereoEnabled;
		}

		// Token: 0x06003FC3 RID: 16323 RVA: 0x001793F4 File Offset: 0x001775F4
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.subpixelMorphologicalAntialiasing);
			propertySheet.properties.SetTexture("_AreaTex", context.resources.smaaLuts.area);
			propertySheet.properties.SetTexture("_SearchTex", context.resources.smaaLuts.search);
			CommandBuffer command = context.command;
			command.BeginSample("SubpixelMorphologicalAntialiasing");
			command.GetTemporaryRT(ShaderIDs.SMAA_Flip, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
			command.GetTemporaryRT(ShaderIDs.SMAA_Flop, context.width, context.height, 0, FilterMode.Bilinear, context.sourceFormat, RenderTextureReadWrite.Linear);
			command.BlitFullscreenTriangle(context.source, ShaderIDs.SMAA_Flip, propertySheet, (int)this.quality, true, null);
			command.BlitFullscreenTriangle(ShaderIDs.SMAA_Flip, ShaderIDs.SMAA_Flop, propertySheet, (int)(3 + this.quality), false, null);
			command.SetGlobalTexture("_BlendTex", ShaderIDs.SMAA_Flop);
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 6, false, null);
			command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flip);
			command.ReleaseTemporaryRT(ShaderIDs.SMAA_Flop);
			command.EndSample("SubpixelMorphologicalAntialiasing");
		}

		// Token: 0x02000F29 RID: 3881
		private enum Pass
		{
			// Token: 0x04004EAB RID: 20139
			EdgeDetection,
			// Token: 0x04004EAC RID: 20140
			BlendWeights = 3,
			// Token: 0x04004EAD RID: 20141
			NeighborhoodBlending = 6
		}

		// Token: 0x02000F2A RID: 3882
		public enum Quality
		{
			// Token: 0x04004EAF RID: 20143
			Low,
			// Token: 0x04004EB0 RID: 20144
			Medium,
			// Token: 0x04004EB1 RID: 20145
			High
		}
	}
}
