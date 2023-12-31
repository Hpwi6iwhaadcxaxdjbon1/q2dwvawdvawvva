﻿using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4F RID: 2639
	[Preserve]
	internal sealed class AutoExposureRenderer : PostProcessEffectRenderer<AutoExposure>
	{
		// Token: 0x04003878 RID: 14456
		private const int k_NumEyes = 2;

		// Token: 0x04003879 RID: 14457
		private const int k_NumAutoExposureTextures = 2;

		// Token: 0x0400387A RID: 14458
		private readonly RenderTexture[][] m_AutoExposurePool = new RenderTexture[2][];

		// Token: 0x0400387B RID: 14459
		private int[] m_AutoExposurePingPong = new int[2];

		// Token: 0x0400387C RID: 14460
		private RenderTexture m_CurrentAutoExposure;

		// Token: 0x06003F5B RID: 16219 RVA: 0x00173B50 File Offset: 0x00171D50
		public AutoExposureRenderer()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_AutoExposurePool[i] = new RenderTexture[2];
				this.m_AutoExposurePingPong[i] = 0;
			}
		}

		// Token: 0x06003F5C RID: 16220 RVA: 0x00173BA0 File Offset: 0x00171DA0
		private void CheckTexture(int eye, int id)
		{
			if (this.m_AutoExposurePool[eye][id] == null || !this.m_AutoExposurePool[eye][id].IsCreated())
			{
				this.m_AutoExposurePool[eye][id] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat)
				{
					enableRandomWrite = true
				};
				this.m_AutoExposurePool[eye][id].Create();
			}
		}

		// Token: 0x06003F5D RID: 16221 RVA: 0x00173BFC File Offset: 0x00171DFC
		public override void Render(PostProcessRenderContext context)
		{
			CommandBuffer command = context.command;
			command.BeginSample("AutoExposureLookup");
			this.CheckTexture(context.xrActiveEye, 0);
			this.CheckTexture(context.xrActiveEye, 1);
			float num = base.settings.filtering.value.x;
			float num2 = base.settings.filtering.value.y;
			num2 = Mathf.Clamp(num2, 1.01f, 99f);
			num = Mathf.Clamp(num, 1f, num2 - 0.01f);
			float value = base.settings.minLuminance.value;
			float value2 = base.settings.maxLuminance.value;
			base.settings.minLuminance.value = Mathf.Min(value, value2);
			base.settings.maxLuminance.value = Mathf.Max(value, value2);
			bool flag = this.m_ResetHistory || !Application.isPlaying;
			string name;
			if (flag || base.settings.eyeAdaptation.value == EyeAdaptation.Fixed)
			{
				name = "KAutoExposureAvgLuminance_fixed";
			}
			else
			{
				name = "KAutoExposureAvgLuminance_progressive";
			}
			ComputeShader autoExposure = context.resources.computeShaders.autoExposure;
			int kernelIndex = autoExposure.FindKernel(name);
			command.SetComputeBufferParam(autoExposure, kernelIndex, "_HistogramBuffer", context.logHistogram.data);
			command.SetComputeVectorParam(autoExposure, "_Params1", new Vector4(num * 0.01f, num2 * 0.01f, RuntimeUtilities.Exp2(base.settings.minLuminance.value), RuntimeUtilities.Exp2(base.settings.maxLuminance.value)));
			command.SetComputeVectorParam(autoExposure, "_Params2", new Vector4(base.settings.speedDown.value, base.settings.speedUp.value, base.settings.keyValue.value, Time.deltaTime));
			command.SetComputeVectorParam(autoExposure, "_ScaleOffsetRes", context.logHistogram.GetHistogramScaleOffsetRes(context));
			if (flag)
			{
				this.m_CurrentAutoExposure = this.m_AutoExposurePool[context.xrActiveEye][0];
				command.SetComputeTextureParam(autoExposure, kernelIndex, "_Destination", this.m_CurrentAutoExposure);
				command.DispatchCompute(autoExposure, kernelIndex, 1, 1, 1);
				RuntimeUtilities.CopyTexture(command, this.m_AutoExposurePool[context.xrActiveEye][0], this.m_AutoExposurePool[context.xrActiveEye][1]);
				this.m_ResetHistory = false;
			}
			else
			{
				int num3 = this.m_AutoExposurePingPong[context.xrActiveEye];
				RenderTexture tex = this.m_AutoExposurePool[context.xrActiveEye][++num3 % 2];
				RenderTexture renderTexture = this.m_AutoExposurePool[context.xrActiveEye][++num3 % 2];
				command.SetComputeTextureParam(autoExposure, kernelIndex, "_Source", tex);
				command.SetComputeTextureParam(autoExposure, kernelIndex, "_Destination", renderTexture);
				command.DispatchCompute(autoExposure, kernelIndex, 1, 1, 1);
				this.m_AutoExposurePingPong[context.xrActiveEye] = (num3 + 1) % 2;
				this.m_CurrentAutoExposure = renderTexture;
			}
			command.EndSample("AutoExposureLookup");
			context.autoExposureTexture = this.m_CurrentAutoExposure;
			context.autoExposure = base.settings;
		}

		// Token: 0x06003F5E RID: 16222 RVA: 0x00173F28 File Offset: 0x00172128
		public override void Release()
		{
			foreach (RenderTexture[] array in this.m_AutoExposurePool)
			{
				for (int j = 0; j < array.Length; j++)
				{
					RuntimeUtilities.Destroy(array[j]);
				}
			}
		}
	}
}
