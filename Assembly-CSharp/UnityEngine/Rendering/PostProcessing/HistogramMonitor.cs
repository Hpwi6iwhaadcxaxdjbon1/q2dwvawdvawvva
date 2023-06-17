using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A75 RID: 2677
	[Serializable]
	public sealed class HistogramMonitor : Monitor
	{
		// Token: 0x04003929 RID: 14633
		public int width = 512;

		// Token: 0x0400392A RID: 14634
		public int height = 256;

		// Token: 0x0400392B RID: 14635
		public HistogramMonitor.Channel channel = HistogramMonitor.Channel.Master;

		// Token: 0x0400392C RID: 14636
		private ComputeBuffer m_Data;

		// Token: 0x0400392D RID: 14637
		private const int k_NumBins = 256;

		// Token: 0x0400392E RID: 14638
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x0400392F RID: 14639
		private const int k_ThreadGroupSizeY = 16;

		// Token: 0x06003FDE RID: 16350 RVA: 0x00179E6B File Offset: 0x0017806B
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003FDF RID: 16351 RVA: 0x0000441C File Offset: 0x0000261C
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x00179E8D File Offset: 0x0017808D
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.gammaHistogram;
		}

		// Token: 0x06003FE1 RID: 16353 RVA: 0x00179EA4 File Offset: 0x001780A4
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.width, this.height);
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(256, 4);
			}
			ComputeShader gammaHistogram = context.resources.computeShaders.gammaHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("GammaHistogram");
			int kernelIndex = gammaHistogram.FindKernel("KHistogramClear");
			command.SetComputeBufferParam(gammaHistogram, kernelIndex, "_HistogramBuffer", this.m_Data);
			command.DispatchCompute(gammaHistogram, kernelIndex, Mathf.CeilToInt(16f), 1, 1);
			kernelIndex = gammaHistogram.FindKernel("KHistogramGather");
			Vector4 vector = new Vector4((float)(context.width / 2), (float)(context.height / 2), (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), (float)this.channel);
			command.SetComputeVectorParam(gammaHistogram, "_Params", vector);
			command.SetComputeTextureParam(gammaHistogram, kernelIndex, "_Source", ShaderIDs.HalfResFinalCopy);
			command.SetComputeBufferParam(gammaHistogram, kernelIndex, "_HistogramBuffer", this.m_Data);
			command.DispatchCompute(gammaHistogram, kernelIndex, Mathf.CeilToInt(vector.x / 16f), Mathf.CeilToInt(vector.y / 16f), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.gammaHistogram);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)this.width, (float)this.height, 0f, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("GammaHistogram");
		}

		// Token: 0x02000F2C RID: 3884
		public enum Channel
		{
			// Token: 0x04004EB6 RID: 20150
			Red,
			// Token: 0x04004EB7 RID: 20151
			Green,
			// Token: 0x04004EB8 RID: 20152
			Blue,
			// Token: 0x04004EB9 RID: 20153
			Master
		}
	}
}
