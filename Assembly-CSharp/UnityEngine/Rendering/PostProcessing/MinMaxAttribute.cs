using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A42 RID: 2626
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MinMaxAttribute : Attribute
	{
		// Token: 0x04003851 RID: 14417
		public readonly float min;

		// Token: 0x04003852 RID: 14418
		public readonly float max;

		// Token: 0x06003F42 RID: 16194 RVA: 0x0017367A File Offset: 0x0017187A
		public MinMaxAttribute(float min, float max)
		{
			this.min = min;
			this.max = max;
		}
	}
}
