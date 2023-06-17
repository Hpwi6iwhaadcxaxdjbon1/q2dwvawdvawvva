using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A60 RID: 2656
	[Preserve]
	[Serializable]
	public sealed class Fog
	{
		// Token: 0x040038D5 RID: 14549
		[Tooltip("Enables the internal deferred fog pass. Actual fog settings should be set in the Lighting panel.")]
		public bool enabled = true;

		// Token: 0x040038D6 RID: 14550
		[Tooltip("Mark true for the fog to ignore the skybox")]
		public bool excludeSkybox = true;

		// Token: 0x06003F87 RID: 16263 RVA: 0x0000441C File Offset: 0x0000261C
		internal DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.Depth;
		}

		// Token: 0x06003F88 RID: 16264 RVA: 0x001769B4 File Offset: 0x00174BB4
		internal bool IsEnabledAndSupported(PostProcessRenderContext context)
		{
			return this.enabled && RenderSettings.fog && !RuntimeUtilities.scriptableRenderPipelineActive && context.resources.shaders.deferredFog && context.resources.shaders.deferredFog.isSupported && context.camera.actualRenderingPath == RenderingPath.DeferredShading;
		}

		// Token: 0x06003F89 RID: 16265 RVA: 0x00176A18 File Offset: 0x00174C18
		internal void Render(PostProcessRenderContext context)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.deferredFog);
			propertySheet.ClearKeywords();
			Color c = RuntimeUtilities.isLinearColorSpace ? RenderSettings.fogColor.linear : RenderSettings.fogColor;
			propertySheet.properties.SetVector(ShaderIDs.FogColor, c);
			propertySheet.properties.SetVector(ShaderIDs.FogParams, new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance));
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, this.excludeSkybox ? 1 : 0, false, null);
		}
	}
}
