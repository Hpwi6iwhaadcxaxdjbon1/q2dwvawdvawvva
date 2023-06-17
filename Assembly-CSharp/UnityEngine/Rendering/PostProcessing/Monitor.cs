using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A78 RID: 2680
	public abstract class Monitor
	{
		// Token: 0x04003939 RID: 14649
		internal bool requested;

		// Token: 0x17000577 RID: 1399
		// (get) Token: 0x06003FE6 RID: 16358 RVA: 0x0017A29E File Offset: 0x0017849E
		// (set) Token: 0x06003FE7 RID: 16359 RVA: 0x0017A2A6 File Offset: 0x001784A6
		public RenderTexture output { get; protected set; }

		// Token: 0x06003FE8 RID: 16360 RVA: 0x0017A2AF File Offset: 0x001784AF
		public bool IsRequestedAndSupported(PostProcessRenderContext context)
		{
			return this.requested && SystemInfo.supportsComputeShaders && !RuntimeUtilities.isAndroidOpenGL && this.ShaderResourcesAvailable(context);
		}

		// Token: 0x06003FE9 RID: 16361
		internal abstract bool ShaderResourcesAvailable(PostProcessRenderContext context);

		// Token: 0x06003FEA RID: 16362 RVA: 0x00007A3C File Offset: 0x00005C3C
		internal virtual bool NeedsHalfRes()
		{
			return false;
		}

		// Token: 0x06003FEB RID: 16363 RVA: 0x0017A2D0 File Offset: 0x001784D0
		protected void CheckOutput(int width, int height)
		{
			if (this.output == null || !this.output.IsCreated() || this.output.width != width || this.output.height != height)
			{
				RuntimeUtilities.Destroy(this.output);
				this.output = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
				{
					anisoLevel = 0,
					filterMode = FilterMode.Bilinear,
					wrapMode = TextureWrapMode.Clamp,
					useMipMap = false
				};
			}
		}

		// Token: 0x06003FEC RID: 16364 RVA: 0x000063A5 File Offset: 0x000045A5
		internal virtual void OnEnable()
		{
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x0017A34A File Offset: 0x0017854A
		internal virtual void OnDisable()
		{
			RuntimeUtilities.Destroy(this.output);
		}

		// Token: 0x06003FEE RID: 16366
		internal abstract void Render(PostProcessRenderContext context);
	}
}
