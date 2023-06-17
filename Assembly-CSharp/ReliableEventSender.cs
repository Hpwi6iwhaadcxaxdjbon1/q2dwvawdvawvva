using System;
using UnityEngine;

// Token: 0x0200028E RID: 654
public class ReliableEventSender : StateMachineBehaviour
{
	// Token: 0x040015D8 RID: 5592
	[Header("State Enter")]
	public string StateEnter;

	// Token: 0x040015D9 RID: 5593
	[Header("Mid State")]
	public string MidStateEvent;

	// Token: 0x040015DA RID: 5594
	[Range(0f, 1f)]
	public float TargetEventTime;
}
