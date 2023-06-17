using System;

namespace UnityEngine.Rendering.PostProcessing
{
	// Token: 0x02000A44 RID: 2628
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	public sealed class TrackballAttribute : Attribute
	{
		// Token: 0x04003858 RID: 14424
		public readonly TrackballAttribute.Mode mode;

		// Token: 0x06003F45 RID: 16197 RVA: 0x001736E0 File Offset: 0x001718E0
		public TrackballAttribute(TrackballAttribute.Mode mode)
		{
			this.mode = mode;
		}

		// Token: 0x02000F1E RID: 3870
		public enum Mode
		{
			// Token: 0x04004E66 RID: 20070
			None,
			// Token: 0x04004E67 RID: 20071
			Lift,
			// Token: 0x04004E68 RID: 20072
			Gamma,
			// Token: 0x04004E69 RID: 20073
			Gain
		}
	}
}
