using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class NpcWalkAnimation : MonoBehaviour, IClientComponent
{
	// Token: 0x040015C6 RID: 5574
	public Vector3 HipFudge = new Vector3(-90f, 0f, 90f);

	// Token: 0x040015C7 RID: 5575
	public BaseNpc Npc;

	// Token: 0x040015C8 RID: 5576
	public Animator Animator;

	// Token: 0x040015C9 RID: 5577
	public Transform HipBone;

	// Token: 0x040015CA RID: 5578
	public Transform LookBone;

	// Token: 0x040015CB RID: 5579
	public bool UpdateWalkSpeed = true;

	// Token: 0x040015CC RID: 5580
	public bool UpdateFacingDirection = true;

	// Token: 0x040015CD RID: 5581
	public bool UpdateGroundNormal = true;

	// Token: 0x040015CE RID: 5582
	public Transform alignmentRoot;

	// Token: 0x040015CF RID: 5583
	public bool LaggyAss = true;

	// Token: 0x040015D0 RID: 5584
	public bool LookAtTarget;

	// Token: 0x040015D1 RID: 5585
	public float MaxLaggyAssRotation = 70f;

	// Token: 0x040015D2 RID: 5586
	public float MaxWalkAnimSpeed = 25f;

	// Token: 0x040015D3 RID: 5587
	public bool UseDirectionBlending;

	// Token: 0x040015D4 RID: 5588
	public bool useTurnPosing;

	// Token: 0x040015D5 RID: 5589
	public float turnPoseScale = 0.5f;

	// Token: 0x040015D6 RID: 5590
	public float laggyAssLerpScale = 15f;

	// Token: 0x040015D7 RID: 5591
	public bool skeletonChainInverted;
}
