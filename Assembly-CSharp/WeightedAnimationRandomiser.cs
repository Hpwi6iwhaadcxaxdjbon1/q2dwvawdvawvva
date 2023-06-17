using System;
using UnityEngine;

// Token: 0x0200021D RID: 541
public class WeightedAnimationRandomiser : StateMachineBehaviour
{
	// Token: 0x0400139A RID: 5018
	public int LoopRangeMin = 3;

	// Token: 0x0400139B RID: 5019
	public int LoopRangeMax = 5;

	// Token: 0x0400139C RID: 5020
	public float NormalizedTransitionDuration;

	// Token: 0x0400139D RID: 5021
	public WeightedAnimationRandomiser.IdleChance[] IdleTransitions = new WeightedAnimationRandomiser.IdleChance[0];

	// Token: 0x0400139E RID: 5022
	public bool AllowRepeats;

	// Token: 0x02000C69 RID: 3177
	[Serializable]
	public struct IdleChance
	{
		// Token: 0x04004314 RID: 17172
		public string StateName;

		// Token: 0x04004315 RID: 17173
		[Range(0f, 100f)]
		public int Chance;
	}
}
