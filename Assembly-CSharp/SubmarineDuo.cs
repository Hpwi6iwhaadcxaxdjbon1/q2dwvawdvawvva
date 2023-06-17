using System;
using Sonar;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
public class SubmarineDuo : BaseSubmarine
{
	// Token: 0x04001F5B RID: 8027
	[Header("Duo Sub Seating & Controls")]
	[SerializeField]
	private Transform steeringWheel;

	// Token: 0x04001F5C RID: 8028
	[SerializeField]
	private Transform steeringWheelLeftGrip;

	// Token: 0x04001F5D RID: 8029
	[SerializeField]
	private Transform steeringWheelRightGrip;

	// Token: 0x04001F5E RID: 8030
	[SerializeField]
	private Transform leftPedal;

	// Token: 0x04001F5F RID: 8031
	[SerializeField]
	private Transform rightPedal;

	// Token: 0x04001F60 RID: 8032
	[SerializeField]
	private Transform driverLeftFoot;

	// Token: 0x04001F61 RID: 8033
	[SerializeField]
	private Transform driverRightFoot;

	// Token: 0x04001F62 RID: 8034
	[SerializeField]
	private Transform mphNeedle;

	// Token: 0x04001F63 RID: 8035
	[SerializeField]
	private Transform fuelNeedle;

	// Token: 0x04001F64 RID: 8036
	[SerializeField]
	private Transform waterDepthNeedle;

	// Token: 0x04001F65 RID: 8037
	[SerializeField]
	private Transform ammoFlag;

	// Token: 0x04001F66 RID: 8038
	[SerializeField]
	private SubmarineSonar sonar;

	// Token: 0x04001F67 RID: 8039
	[SerializeField]
	private Transform torpedoTubeHatch;
}
