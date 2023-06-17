using System;
using UnityEngine;

// Token: 0x0200021C RID: 540
public class HorseIdleMultiConditionCrossfade : StateMachineBehaviour
{
	// Token: 0x04001398 RID: 5016
	public string TargetState = "breathe";

	// Token: 0x04001399 RID: 5017
	public float NormalizedTransitionDuration = 0.1f;

	// Token: 0x02000C68 RID: 3176
	[Serializable]
	public struct Condition
	{
		// Token: 0x04004311 RID: 17169
		public int FloatParameter;

		// Token: 0x04004312 RID: 17170
		public HorseIdleMultiConditionCrossfade.Condition.CondtionOperator Operator;

		// Token: 0x04004313 RID: 17171
		public float Value;

		// Token: 0x02000FCA RID: 4042
		public enum CondtionOperator
		{
			// Token: 0x040050D7 RID: 20695
			GreaterThan,
			// Token: 0x040050D8 RID: 20696
			LessThan
		}
	}
}
