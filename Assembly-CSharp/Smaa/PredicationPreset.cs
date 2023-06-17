using System;
using UnityEngine;

namespace Smaa
{
	// Token: 0x020009BE RID: 2494
	[Serializable]
	public class PredicationPreset
	{
		// Token: 0x0400362A RID: 13866
		[Min(0.0001f)]
		public float Threshold = 0.01f;

		// Token: 0x0400362B RID: 13867
		[Range(1f, 5f)]
		public float Scale = 2f;

		// Token: 0x0400362C RID: 13868
		[Range(0f, 1f)]
		public float Strength = 0.4f;
	}
}
