using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A40 RID: 2624
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class MaxAttribute : Attribute
	{
		// Token: 0x0400384F RID: 14415
		public readonly float max;

		// Token: 0x06003F40 RID: 16192 RVA: 0x0017365C File Offset: 0x0017185C
		public MaxAttribute(float max)
		{
			this.max = max;
		}
	}
}
