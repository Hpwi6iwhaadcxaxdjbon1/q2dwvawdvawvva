using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4A RID: 2634
	internal interface IAmbientOcclusionMethod
	{
		// Token: 0x06003F4A RID: 16202
		DepthTextureMode GetCameraFlags();

		// Token: 0x06003F4B RID: 16203
		void RenderAfterOpaque(PostProcessRenderContext context);

		// Token: 0x06003F4C RID: 16204
		void RenderAmbientOnly(PostProcessRenderContext context);

		// Token: 0x06003F4D RID: 16205
		void CompositeAmbientOnly(PostProcessRenderContext context);

		// Token: 0x06003F4E RID: 16206
		void Release();
	}
}
