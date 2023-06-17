using System;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A87 RID: 2695
	public sealed class PostProcessBundle
	{
		// Token: 0x04003950 RID: 14672
		private PostProcessEffectRenderer m_Renderer;

		// Token: 0x17000578 RID: 1400
		// (get) Token: 0x06004024 RID: 16420 RVA: 0x0017AE26 File Offset: 0x00179026
		// (set) Token: 0x06004025 RID: 16421 RVA: 0x0017AE2E File Offset: 0x0017902E
		public PostProcessAttribute attribute { get; private set; }

		// Token: 0x17000579 RID: 1401
		// (get) Token: 0x06004026 RID: 16422 RVA: 0x0017AE37 File Offset: 0x00179037
		// (set) Token: 0x06004027 RID: 16423 RVA: 0x0017AE3F File Offset: 0x0017903F
		public PostProcessEffectSettings settings { get; private set; }

		// Token: 0x1700057A RID: 1402
		// (get) Token: 0x06004028 RID: 16424 RVA: 0x0017AE48 File Offset: 0x00179048
		internal PostProcessEffectRenderer renderer
		{
			get
			{
				if (this.m_Renderer == null)
				{
					Assert.IsNotNull<Type>(this.attribute.renderer);
					Type renderer = this.attribute.renderer;
					this.m_Renderer = (PostProcessEffectRenderer)Activator.CreateInstance(renderer);
					this.m_Renderer.SetSettings(this.settings);
					this.m_Renderer.Init();
				}
				return this.m_Renderer;
			}
		}

		// Token: 0x06004029 RID: 16425 RVA: 0x0017AEAC File Offset: 0x001790AC
		internal PostProcessBundle(PostProcessEffectSettings settings)
		{
			Assert.IsNotNull<PostProcessEffectSettings>(settings);
			this.settings = settings;
			this.attribute = settings.GetType().GetAttribute<PostProcessAttribute>();
		}

		// Token: 0x0600402A RID: 16426 RVA: 0x0017AED2 File Offset: 0x001790D2
		internal void Release()
		{
			if (this.m_Renderer != null)
			{
				this.m_Renderer.Release();
			}
			RuntimeUtilities.Destroy(this.settings);
		}

		// Token: 0x0600402B RID: 16427 RVA: 0x0017AEF2 File Offset: 0x001790F2
		internal void ResetHistory()
		{
			if (this.m_Renderer != null)
			{
				this.m_Renderer.ResetHistory();
			}
		}

		// Token: 0x0600402C RID: 16428 RVA: 0x0017AF07 File Offset: 0x00179107
		internal T CastSettings<T>() where T : PostProcessEffectSettings
		{
			return (T)((object)this.settings);
		}

		// Token: 0x0600402D RID: 16429 RVA: 0x0017AF14 File Offset: 0x00179114
		internal T CastRenderer<T>() where T : PostProcessEffectRenderer
		{
			return (T)((object)this.renderer);
		}
	}
}
