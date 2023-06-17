using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A79 RID: 2681
	[Serializable]
	public sealed class VectorscopeMonitor : Monitor
	{
		// Token: 0x0400393A RID: 14650
		public int size = 256;

		// Token: 0x0400393B RID: 14651
		public float exposure = 0.12f;

		// Token: 0x0400393C RID: 14652
		private ComputeBuffer m_Data;

		// Token: 0x0400393D RID: 14653
		private const int k_ThreadGroupSizeX = 16;

		// Token: 0x0400393E RID: 14654
		private const int k_ThreadGroupSizeY = 16;

		// Token: 0x06003FF0 RID: 16368 RVA: 0x0017A357 File Offset: 0x00178557
		internal override void OnDisable()
		{
			base.OnDisable();
			if (this.m_Data != null)
			{
				this.m_Data.Release();
			}
			this.m_Data = null;
		}

		// Token: 0x06003FF1 RID: 16369 RVA: 0x0000441C File Offset: 0x0000261C
		internal override bool NeedsHalfRes()
		{
			return true;
		}

		// Token: 0x06003FF2 RID: 16370 RVA: 0x0017A379 File Offset: 0x00178579
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.computeShaders.vectorscope;
		}

		// Token: 0x06003FF3 RID: 16371 RVA: 0x0017A390 File Offset: 0x00178590
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.size, this.size);
			this.exposure = Mathf.Max(0f, this.exposure);
			int num = this.size * this.size;
			if (this.m_Data == null)
			{
				this.m_Data = new ComputeBuffer(num, 4);
			}
			else if (this.m_Data.count != num)
			{
				this.m_Data.Release();
				this.m_Data = new ComputeBuffer(num, 4);
			}
			ComputeShader vectorscope = context.resources.computeShaders.vectorscope;
			CommandBuffer command = context.command;
			command.BeginSample("Vectorscope");
			Vector4 vector = new Vector4((float)(context.width / 2), (float)(context.height / 2), (float)this.size, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0));
			int kernelIndex = vectorscope.FindKernel("KVectorscopeClear");
			command.SetComputeBufferParam(vectorscope, kernelIndex, "_VectorscopeBuffer", this.m_Data);
			command.SetComputeVectorParam(vectorscope, "_Params", vector);
			command.DispatchCompute(vectorscope, kernelIndex, Mathf.CeilToInt((float)this.size / 16f), Mathf.CeilToInt((float)this.size / 16f), 1);
			kernelIndex = vectorscope.FindKernel("KVectorscopeGather");
			command.SetComputeBufferParam(vectorscope, kernelIndex, "_VectorscopeBuffer", this.m_Data);
			command.SetComputeTextureParam(vectorscope, kernelIndex, "_Source", ShaderIDs.HalfResFinalCopy);
			command.DispatchCompute(vectorscope, kernelIndex, Mathf.CeilToInt(vector.x / 16f), Mathf.CeilToInt(vector.y / 16f), 1);
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.vectorscope);
			propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)this.size, (float)this.size, this.exposure, 0f));
			propertySheet.properties.SetBuffer(ShaderIDs.VectorscopeBuffer, this.m_Data);
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("Vectorscope");
		}
	}
}
