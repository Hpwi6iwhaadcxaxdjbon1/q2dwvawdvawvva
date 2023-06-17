using System;
using UnityEngine;

// Token: 0x02000202 RID: 514
public class EnvironmentFish : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x0400131A RID: 4890
	public Animator animator;

	// Token: 0x0400131B RID: 4891
	public float minSpeed;

	// Token: 0x0400131C RID: 4892
	public float maxSpeed;

	// Token: 0x0400131D RID: 4893
	public float idealDepth;

	// Token: 0x0400131E RID: 4894
	public float minTurnSpeed = 0.5f;

	// Token: 0x0400131F RID: 4895
	public float maxTurnSpeed = 180f;

	// Token: 0x04001320 RID: 4896
	public Vector3 destination;

	// Token: 0x04001321 RID: 4897
	public Vector3 spawnPos;

	// Token: 0x04001322 RID: 4898
	public Vector3 idealLocalScale = Vector3.one;
}
