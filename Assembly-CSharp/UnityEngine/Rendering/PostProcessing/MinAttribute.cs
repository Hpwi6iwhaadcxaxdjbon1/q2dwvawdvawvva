using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A41 RID: 2625
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MinAttribute : Attribute
	{
		// Token: 0x04003850 RID: 14416
		public readonly float min;

		// Token: 0x06003F41 RID: 16193 RVA: 0x0017366B File Offset: 0x0017186B
		public MinAttribute(float min)
		{
			this.min = min;
		}
	}
}
