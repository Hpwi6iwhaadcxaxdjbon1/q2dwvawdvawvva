using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A97 RID: 2711
	public static class HaltonSeq
	{
		// Token: 0x0600409E RID: 16542 RVA: 0x0017D258 File Offset: 0x0017B458
		public static float Get(int index, int radix)
		{
			float num = 0f;
			float num2 = 1f / (float)radix;
			while (index > 0)
			{
				num += (float)(index % radix) * num2;
				index /= radix;
				num2 /= (float)radix;
			}
			return num;
		}
	}
}
