using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8C RID: 2700
	public abstract class PostProcessEffectRenderer
	{
		// Token: 0x04003975 RID: 14709
		protected bool m_ResetHistory = true;

		// Token: 0x06004049 RID: 16457 RVA: 0x000063A5 File Offset: 0x000045A5
		public virtual void Init()
		{
		}

		// Token: 0x0600404A RID: 16458 RVA: 0x00007A3C File Offset: 0x00005C3C
		public virtual DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.None;
		}

		// Token: 0x0600404B RID: 16459 RVA: 0x0017B868 File Offset: 0x00179A68
		public virtual void ResetHistory()
		{
			this.m_ResetHistory = true;
		}

		// Token: 0x0600404C RID: 16460 RVA: 0x0017B871 File Offset: 0x00179A71
		public virtual void Release()
		{
			this.ResetHistory();
		}

		// Token: 0x0600404D RID: 16461
		public abstract void Render(PostProcessRenderContext context);

		// Token: 0x0600404E RID: 16462
		internal abstract void SetSettings(PostProcessEffectSettings settings);
	}
}
