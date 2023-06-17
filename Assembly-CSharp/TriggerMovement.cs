using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class TriggerMovement : TriggerBase, IClientComponent
{
	// Token: 0x0400175E RID: 5982
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x0400175F RID: 5983
	public BaseEntity.MovementModify movementModify;
}
