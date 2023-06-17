using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A4B RID: 2635
	[Preserve]
	internal sealed class AmbientOcclusionRenderer : PostProcessEffectRenderer<AmbientOcclusion>
	{
		// Token: 0x0400386D RID: 14445
		private IAmbientOcclusionMethod[] m_Methods;

		// Token: 0x06003F4F RID: 16207 RVA: 0x00173951 File Offset: 0x00171B51
		public override void Init()
		{
			if (this.m_Methods == null)
			{
				this.m_Methods = new IAmbientOcclusionMethod[]
				{
					new ScalableAO(base.settings),
					new MultiScaleVO(base.settings)
				};
			}
		}

		// Token: 0x06003F50 RID: 16208 RVA: 0x00173984 File Offset: 0x00171B84
		public bool IsAmbientOnly(PostProcessRenderContext context)
		{
			Camera camera = context.camera;
			return base.settings.ambientOnly.value && camera.actualRenderingPath == RenderingPath.DeferredShading && camera.allowHDR;
		}

		// Token: 0x06003F51 RID: 16209 RVA: 0x001739BB File Offset: 0x00171BBB
		public IAmbientOcclusionMethod Get()
		{
			return this.m_Methods[(int)base.settings.mode.value];
		}

		// Token: 0x06003F52 RID: 16210 RVA: 0x001739D4 File Offset: 0x00171BD4
		public override DepthTextureMode GetCameraFlags()
		{
			return this.Get().GetCameraFlags();
		}

		// Token: 0x06003F53 RID: 16211 RVA: 0x001739E4 File Offset: 0x00171BE4
		public override void Release()
		{
			IAmbientOcclusionMethod[] methods = this.m_Methods;
			for (int i = 0; i < methods.Length; i++)
			{
				methods[i].Release();
			}
		}

		// Token: 0x06003F54 RID: 16212 RVA: 0x00173A0E File Offset: 0x00171C0E
		public ScalableAO GetScalableAO()
		{
			return (ScalableAO)this.m_Methods[0];
		}

		// Token: 0x06003F55 RID: 16213 RVA: 0x00173A1D File Offset: 0x00171C1D
		public MultiScaleVO GetMultiScaleVO()
		{
			return (MultiScaleVO)this.m_Methods[1];
		}

		// Token: 0x06003F56 RID: 16214 RVA: 0x000063A5 File Offset: 0x000045A5
		public override void Render(PostProcessRenderContext context)
		{
		}
	}
}
