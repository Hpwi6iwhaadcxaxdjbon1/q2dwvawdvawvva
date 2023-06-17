using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A3F RID: 2623
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class DisplayNameAttribute : Attribute
	{
		// Token: 0x0400384E RID: 14414
		public readonly string displayName;

		// Token: 0x06003F3F RID: 16191 RVA: 0x0017364D File Offset: 0x0017184D
		public DisplayNameAttribute(string displayName)
		{
			this.displayName = displayName;
		}
	}
}
