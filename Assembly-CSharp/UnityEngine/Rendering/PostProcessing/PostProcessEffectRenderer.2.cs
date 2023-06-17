using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A8D RID: 2701
	public abstract class PostProcessEffectRenderer<T> : PostProcessEffectRenderer where T : PostProcessEffectSettings
	{
		// Token: 0x1700057E RID: 1406
		// (get) Token: 0x06004050 RID: 16464 RVA: 0x0017B888 File Offset: 0x00179A88
		// (set) Token: 0x06004051 RID: 16465 RVA: 0x0017B890 File Offset: 0x00179A90
		public T settings { get; internal set; }

		// Token: 0x06004052 RID: 16466 RVA: 0x0017B899 File Offset: 0x00179A99
		internal override void SetSettings(PostProcessEffectSettings settings)
		{
			this.settings = (T)((object)settings);
		}
	}
}
