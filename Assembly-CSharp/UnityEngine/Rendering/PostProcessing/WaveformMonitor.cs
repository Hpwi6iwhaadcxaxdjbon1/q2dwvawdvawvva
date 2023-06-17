using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A7A RID: 2682
	[Serializable]
	public sealed class WaveformMonitor : Monitor
	{
		// Token: 0x0400393F RID: 14655
		public float exposure = 0.12f;

		// Token: 0x04003940 RID: 14656
		public int height = 256;

		// Token: 0x04003941 RID: 14657
		private ComputeBuffer m_Data;

		// Token: 0x04003942 RID: 14658
		private const int k_ThreadGroupSize = 256;

		// Token: 0x04003943 RID: 14659
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x04003944 RID: 14660
		private const int k_ThreadGroupSizeY = 16;

		// Token: 0x06003FF5 RID: 16373 RVA: 0x0017A5C8 File Offset: 0x001787C8
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003FF6 RID: 16374 RVA: 0x0000441C File Offset: 0x0000261C
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003FF7 RID: 16375 RVA: 0x0017A5EA File Offset: 0x001787EA
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.waveform;
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x0017A604 File Offset: 0x00178804
		internal override void Render(PostProcessRenderContext context)
		{
			float num = (float)context.width / 2f / ((float)context.height / 2f);
			int num2 = Mathf.FloorToInt((float)this.height * num);
			base.CheckOutput(num2, this.height);
			this.exposure = Mathf.Max(0f, this.exposure);
			int num3 = num2 * this.height;
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(num3, 16);
			}
			else if (this.m_Data.count < num3)
			{
				this.m_Data.Release();
				this.m_Data = new ComputeBuffer(num3, 16);
			}
			ComputeShader waveform = context.resources.computeShaders.waveform;
			CommandBuffer command = context.command;
			command.BeginSample("Waveform");
			Vector4 val = new Vector4((float)num2, (float)this.height, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), 0f);
			int kernelIndex = waveform.FindKernel("KWaveformClear");
			command.SetComputeBufferParam(waveform, kernelIndex, "_WaveformBuffer", this.m_Data);
			command.SetComputeVectorParam(waveform, "_Params", val);
			command.DispatchCompute(waveform, kernelIndex, Mathf.CeilToInt((float)num2 / 16f), Mathf.CeilToInt((float)this.height / 16f), 1);
			command.GetTemporaryRT(ShaderIDs.WaveformSource, num2, this.height, 0, FilterMode.Bilinear, context.sourceFormat);
			command.BlitFullscreenTriangle(ShaderIDs.HalfResFinalCopy, ShaderIDs.WaveformSource, false, null);
			kernelIndex = waveform.FindKernel("KWaveformGather");
			command.SetComputeBufferParam(waveform, kernelIndex, "_WaveformBuffer", this.m_Data);
			command.SetComputeTextureParam(waveform, kernelIndex, "_Source", ShaderIDs.WaveformSource);
			command.SetComputeVectorParam(waveform, "_Params", val);
			command.DispatchCompute(waveform, kernelIndex, num2, Mathf.CeilToInt((float)this.height / 256f), 1);
			command.ReleaseTemporaryRT(ShaderIDs.WaveformSource);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.waveform);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)num2, (float)this.height, this.exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.WaveformBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("Waveform");
		}
	}
}
