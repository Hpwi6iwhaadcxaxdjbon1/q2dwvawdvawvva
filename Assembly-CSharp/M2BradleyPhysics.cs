using System;
using UnityEngine;

// Token: 0x02000416 RID: 1046
public class M2BradleyPhysics : MonoBehaviour
{
	// Token: 0x04001B5D RID: 7005
	private m2bradleyAnimator m2Animator;

	// Token: 0x04001B5E RID: 7006
	public WheelCollider[] Wheels;

	// Token: 0x04001B5F RID: 7007
	public WheelCollider[] TurningWheels;

	// Token: 0x04001B60 RID: 7008
	public Rigidbody mainRigidbody;

	// Token: 0x04001B61 RID: 7009
	public Transform[] waypoints;

	// Token: 0x04001B62 RID: 7010
	private Vector3 currentWaypoint;

	// Token: 0x04001B63 RID: 7011
	private Vector3 nextWaypoint;
}
