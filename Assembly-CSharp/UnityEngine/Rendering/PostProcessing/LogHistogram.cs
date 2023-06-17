using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A98 RID: 2712
	internal sealed class LogHistogram
	{
		// Token: 0x040039A2 RID: 14754
		public const int rangeMin = -9;

		// Token: 0x040039A3 RID: 14755
		public const int rangeMax = 9;

		// Token: 0x040039A4 RID: 14756
		private const int k_Bins = 128;

		// Token: 0x17000586 RID: 1414
		// (get) Token: 0x0600409F RID: 16543 RVA: 0x0017D28E File Offset: 0x0017B48E
		// (set) Token: 0x060040A0 RID: 16544 RVA: 0x0017D296 File Offset: 0x0017B496
		public ComputeBuffer data { get; private set; }

		// Token: 0x060040A1 RID: 16545 RVA: 0x0017D2A0 File Offset: 0x0017B4A0
		public void Generate(PostProcessRenderContext context)
		{
			if (this.data == null)
			{
				this.data = new ComputeBuffer(128, 4);
			}
			Vector4 histogramScaleOffsetRes = this.GetHistogramScaleOffsetRes(context);
			ComputeShader exposureHistogram = context.resources.computeShaders.exposureHistogram;
			CommandBuffer command = context.command;
			command.BeginSample("LogHistogram");
			int kernelIndex = exposureHistogram.FindKernel("KEyeHistogramClear");
			command.SetComputeBufferParam(exposureHistogram, kernelIndex, "_HistogramBuffer", this.data);
			uint num;
			uint num2;
			uint num3;
			exposureHistogram.GetKernelThreadGroupSizes(kernelIndex, out num, out num2, out num3);
			command.DispatchCompute(exposureHistogram, kernelIndex, Mathf.CeilToInt(128f / num), 1, 1);
			kernelIndex = exposureHistogram.FindKernel("KEyeHistogram");
			command.SetComputeBufferParam(exposureHistogram, kernelIndex, "_HistogramBuffer", this.data);
			command.SetComputeTextureParam(exposureHistogram, kernelIndex, "_Source", context.source);
			command.SetComputeVectorParam(exposureHistogram, "_ScaleOffsetRes", histogramScaleOffsetRes);
			exposureHistogram.GetKernelThreadGroupSizes(kernelIndex, out num, out num2, out num3);
			command.DispatchCompute(exposureHistogram, kernelIndex, Mathf.CeilToInt(histogramScaleOffsetRes.z / 2f / num), Mathf.CeilToInt(histogramScaleOffsetRes.w / 2f / num2), 1);
			command.EndSample("LogHistogram");
		}

		// Token: 0x060040A2 RID: 16546 RVA: 0x0017D3D4 File Offset: 0x0017B5D4
		public Vector4 GetHistogramScaleOffsetRes(PostProcessRenderContext context)
		{
			float num = 18f;
			float num2 = 1f / num;
			float y = 9f * num2;
			return new Vector4(num2, y, (float)context.width, (float)context.height);
		}

		// Token: 0x060040A3 RID: 16547 RVA: 0x0017D40C File Offset: 0x0017B60C
		public void Release()
		{
			if (this.data != null)
			{
				this.data.Release();
			}
			this.data = null;
		}
	}
}
