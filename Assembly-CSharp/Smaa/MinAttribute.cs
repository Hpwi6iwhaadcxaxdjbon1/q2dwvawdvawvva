using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009BD RID: 2493
	public sealed class MinAttribute : PropertyAttribute
	{
		// Token: 0x04003629 RID: 13865
		public readonly float min;

		// Token: 0x06003BA7 RID: 15271 RVA: 0x00160DFA File Offset: 0x0015EFFA
		public MinAttribute(float min)
		{
			this.min = min;
		}
	}
}
