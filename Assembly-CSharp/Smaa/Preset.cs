using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009BF RID: 2495
	[Serializable]
	public class Preset
	{
		// Token: 0x0400362D RID: 13869
		public bool DiagDetection = true;

		// Token: 0x0400362E RID: 13870
		public bool CornerDetection = true;

		// Token: 0x0400362F RID: 13871
		[Range(0f, 0.5f)]
		public float Threshold = 0.1f;

		// Token: 0x04003630 RID: 13872
		[Min(0.0001f)]
		public float DepthThreshold = 0.01f;

		// Token: 0x04003631 RID: 13873
		[Range(0f, 112f)]
		public int MaxSearchSteps = 16;

		// Token: 0x04003632 RID: 13874
		[Range(0f, 20f)]
		public int MaxSearchStepsDiag = 8;

		// Token: 0x04003633 RID: 13875
		[Range(0f, 100f)]
		public int CornerRounding = 25;

		// Token: 0x04003634 RID: 13876
		[Min(0f)]
		public float LocalContrastAdaptationFactor = 2f;
	}
}
