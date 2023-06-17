using System;

namespace UnityEngine.UI
{
	// Token: 0x02000A2C RID: 2604
	public class ScrollRectSettable : ScrollRect
	{
		// Token: 0x06003E15 RID: 15893 RVA: 0x0016C05C File Offset: 0x0016A25C
		public void SetHorizNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 0);
		}

		// Token: 0x06003E16 RID: 15894 RVA: 0x0016C066 File Offset: 0x0016A266
		public void SetVertNormalizedPosition(float value)
		{
			this.SetNormalizedPosition(value, 1);
		}
	}
}
