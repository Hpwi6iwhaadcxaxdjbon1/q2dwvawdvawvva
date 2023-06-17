using System;
using UnityEngine;

// Token: 0x02000451 RID: 1105
public class PlayerWalkMovement : BaseMovement
{
	// Token: 0x04001D13 RID: 7443
	public const float WaterLevelHead = 0.75f;

	// Token: 0x04001D14 RID: 7444
	public const float WaterLevelNeck = 0.65f;

	// Token: 0x04001D15 RID: 7445
	public PhysicMaterial zeroFrictionMaterial;

	// Token: 0x04001D16 RID: 7446
	public PhysicMaterial highFrictionMaterial;

	// Token: 0x04001D17 RID: 7447
	public float capsuleHeight = 1f;

	// Token: 0x04001D18 RID: 7448
	public float capsuleCenter = 1f;

	// Token: 0x04001D19 RID: 7449
	public float capsuleHeightDucked = 1f;

	// Token: 0x04001D1A RID: 7450
	public float capsuleCenterDucked = 1f;

	// Token: 0x04001D1B RID: 7451
	public float capsuleHeightCrawling = 0.5f;

	// Token: 0x04001D1C RID: 7452
	public float capsuleCenterCrawling = 0.5f;

	// Token: 0x04001D1D RID: 7453
	public float gravityTestRadius = 0.2f;

	// Token: 0x04001D1E RID: 7454
	public float gravityMultiplier = 2.5f;

	// Token: 0x04001D1F RID: 7455
	public float gravityMultiplierSwimming = 0.1f;

	// Token: 0x04001D20 RID: 7456
	public float maxAngleWalking = 50f;

	// Token: 0x04001D21 RID: 7457
	public float maxAngleClimbing = 60f;

	// Token: 0x04001D22 RID: 7458
	public float maxAngleSliding = 90f;

	// Token: 0x04001D23 RID: 7459
	public float maxStepHeight = 0.25f;
}
