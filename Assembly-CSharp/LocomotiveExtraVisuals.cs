using System;
using UnityEngine;

// Token: 0x020004AD RID: 1197
public class LocomotiveExtraVisuals : MonoBehaviour
{
	// Token: 0x04001F8E RID: 8078
	[Header("Gauges")]
	[SerializeField]
	private TrainEngine trainEngine;

	// Token: 0x04001F8F RID: 8079
	[SerializeField]
	private Transform needleA;

	// Token: 0x04001F90 RID: 8080
	[SerializeField]
	private Transform needleB;

	// Token: 0x04001F91 RID: 8081
	[SerializeField]
	private Transform needleC;

	// Token: 0x04001F92 RID: 8082
	[SerializeField]
	private float maxAngle = 240f;

	// Token: 0x04001F93 RID: 8083
	[SerializeField]
	private float speedoMoveSpeed = 75f;

	// Token: 0x04001F94 RID: 8084
	[SerializeField]
	private float pressureMoveSpeed = 25f;

	// Token: 0x04001F95 RID: 8085
	[SerializeField]
	private float fanAcceleration = 50f;

	// Token: 0x04001F96 RID: 8086
	[SerializeField]
	private float fanMaxSpeed = 1000f;

	// Token: 0x04001F97 RID: 8087
	[SerializeField]
	private float speedoMax = 80f;

	// Token: 0x04001F98 RID: 8088
	[Header("Fans")]
	[SerializeField]
	private Transform[] engineFans;
}
