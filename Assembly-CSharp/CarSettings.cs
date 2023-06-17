using System;
using UnityEngine;

// Token: 0x02000474 RID: 1140
[Serializable]
public class CarSettings
{
	// Token: 0x04001DCC RID: 7628
	[Header("Vehicle Setup")]
	[Range(0f, 1f)]
	public float rollingResistance = 0.05f;

	// Token: 0x04001DCD RID: 7629
	[Range(0f, 1f)]
	public float antiRoll;

	// Token: 0x04001DCE RID: 7630
	public bool canSleep = true;

	// Token: 0x04001DCF RID: 7631
	[Header("Wheels")]
	public bool tankSteering;

	// Token: 0x04001DD0 RID: 7632
	[Range(0f, 50f)]
	public float maxSteerAngle = 35f;

	// Token: 0x04001DD1 RID: 7633
	public bool steeringAssist = true;

	// Token: 0x04001DD2 RID: 7634
	[Range(0f, 1f)]
	public float steeringAssistRatio = 0.5f;

	// Token: 0x04001DD3 RID: 7635
	public bool steeringLimit;

	// Token: 0x04001DD4 RID: 7636
	[Range(0f, 50f)]
	public float minSteerLimitAngle = 6f;

	// Token: 0x04001DD5 RID: 7637
	[Range(10f, 50f)]
	public float minSteerLimitSpeed = 30f;

	// Token: 0x04001DD6 RID: 7638
	[Range(0f, 1f)]
	public float rearWheelSteer = 1f;

	// Token: 0x04001DD7 RID: 7639
	public float steerMinLerpSpeed = 75f;

	// Token: 0x04001DD8 RID: 7640
	public float steerMaxLerpSpeed = 150f;

	// Token: 0x04001DD9 RID: 7641
	public float steerReturnLerpSpeed = 200f;

	// Token: 0x04001DDA RID: 7642
	[Header("Motor")]
	public float maxDriveSlip = 4f;

	// Token: 0x04001DDB RID: 7643
	public float driveForceToMaxSlip = 1000f;

	// Token: 0x04001DDC RID: 7644
	public float reversePercentSpeed = 0.3f;

	// Token: 0x04001DDD RID: 7645
	[Header("Brakes")]
	public float brakeForceMultiplier = 1000f;

	// Token: 0x04001DDE RID: 7646
	[Header("Front/Rear Vehicle Balance")]
	[Range(0f, 1f)]
	public float handlingBias = 0.5f;
}
