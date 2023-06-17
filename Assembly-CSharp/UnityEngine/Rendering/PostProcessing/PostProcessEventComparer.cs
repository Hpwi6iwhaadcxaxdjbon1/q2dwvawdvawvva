using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A90 RID: 2704
	internal struct PostProcessEventComparer : IEqualityComparer<PostProcessEvent>
	{
		// Token: 0x0600405B RID: 16475 RVA: 0x0017BABE File Offset: 0x00179CBE
		public bool Equals(PostProcessEvent x, PostProcessEvent y)
		{
			return x == y;
		}

		// Token: 0x0600405C RID: 16476 RVA: 0x00036DC0 File Offset: 0x00034FC0
		public int GetHashCode(PostProcessEvent obj)
		{
			return (int)obj;
		}
	}
}
