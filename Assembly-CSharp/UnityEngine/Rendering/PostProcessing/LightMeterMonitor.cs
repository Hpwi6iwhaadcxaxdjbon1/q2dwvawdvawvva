using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A76 RID: 2678
	[Serializable]
	public sealed class LightMeterMonitor : Monitor
	{
		// Token: 0x04003930 RID: 14640
		public int width = 512;

		// Token: 0x04003931 RID: 14641
		public int height = 256;

		// Token: 0x04003932 RID: 14642
		public bool showCurves = true;

		// Token: 0x06003FE3 RID: 16355 RVA: 0x0017A07D File Offset: 0x0017827D
		internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
		{
			return context.resources.shaders.lightMeter && context.resources.shaders.lightMeter.isSupported;
		}

		// Token: 0x06003FE4 RID: 16356 RVA: 0x0017A0B0 File Offset: 0x001782B0
		internal override void Render(PostProcessRenderContext context)
		{
			base.CheckOutput(this.width, this.height);
			LogHistogram logHistogram = context.logHistogram;
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lightMeter);
			propertySheet.ClearKeywords();
			propertySheet.properties.SetBuffer(ShaderIDs.HistogramBuffer, logHistogram.data);
			Vector4 histogramScaleOffsetRes = logHistogram.GetHistogramScaleOffsetRes(context);
			histogramScaleOffsetRes.z = 1f / (float)this.width;
			histogramScaleOffsetRes.w = 1f / (float)this.height;
			propertySheet.properties.SetVector(ShaderIDs.ScaleOffsetRes, histogramScaleOffsetRes);
			if (context.logLut != null && this.showCurves)
			{
				propertySheet.EnableKeyword("COLOR_GRADING_HDR");
				propertySheet.properties.SetTexture(ShaderIDs.Lut3D, context.logLut);
			}
			AutoExposure autoExposure = context.autoExposure;
			if (autoExposure != null)
			{
				float num = autoExposure.filtering.value.x;
				float num2 = autoExposure.filtering.value.y;
				num2 = Mathf.Clamp(num2, 1.01f, 99f);
				num = Mathf.Clamp(num, 1f, num2 - 0.01f);
				Vector4 value = new Vector4(num * 0.01f, num2 * 0.01f, RuntimeUtilities.Exp2(autoExposure.minLuminance.value), RuntimeUtilities.Exp2(autoExposure.maxLuminance.value));
				propertySheet.EnableKeyword("AUTO_EXPOSURE");
				propertySheet.properties.SetVector(ShaderIDs.Params, value);
			}
			CommandBuffer command = context.command;
			command.BeginSample("LightMeter");
			command.BlitFullscreenTriangle(BuiltinRenderTextureType.None, base.output, propertySheet, 0, false, null);
			command.EndSample("LightMeter");
		}
	}
}
