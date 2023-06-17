using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A9F RID: 2719
	internal class TargetPool
	{
		// Token: 0x04003A46 RID: 14918
		private readonly List<int> m_Pool;

		// Token: 0x04003A47 RID: 14919
		private int m_Current;

		// Token: 0x060040F2 RID: 16626 RVA: 0x0017EEEA File Offset: 0x0017D0EA
		internal TargetPool()
		{
			this.m_Pool = new List<int>();
			this.Get();
		}

		// Token: 0x060040F3 RID: 16627 RVA: 0x0017EF04 File Offset: 0x0017D104
		internal int Get()
		{
			int result = this.Get(this.m_Current);
			this.m_Current++;
			return result;
		}

		// Token: 0x060040F4 RID: 16628 RVA: 0x0017EF20 File Offset: 0x0017D120
		private int Get(int i)
		{
			int result;
			if (this.m_Pool.Count > i)
			{
				result = this.m_Pool[i];
			}
			else
			{
				while (this.m_Pool.Count <= i)
				{
					this.m_Pool.Add(Shader.PropertyToID("_TargetPool" + i));
				}
				result = this.m_Pool[i];
			}
			return result;
		}

		// Token: 0x060040F5 RID: 16629 RVA: 0x0017EF86 File Offset: 0x0017D186
		internal void Reset()
		{
			this.m_Current = 0;
		}
	}
}
