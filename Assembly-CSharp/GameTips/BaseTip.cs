using System;

namespace GameTips
{
	// Token: 0x020009DC RID: 2524
	public abstract class BaseTip
	{
		// Token: 0x06003C4E RID: 15438
		public abstract Translate.Phrase GetPhrase();

		// Token: 0x170004E6 RID: 1254
		// (get) Token: 0x06003C4F RID: 15439
		public abstract bool ShouldShow { get; }

		// Token: 0x170004E7 RID: 1255
		// (get) Token: 0x06003C50 RID: 15440 RVA: 0x001631E0 File Offset: 0x001613E0
		public string Type
		{
			get
			{
				return base.GetType().Name;
			}
		}
	}
}
