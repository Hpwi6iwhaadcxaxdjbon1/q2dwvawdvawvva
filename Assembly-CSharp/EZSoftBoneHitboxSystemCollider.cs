using System;
using EZhex1991.EZSoftBone;
using UnityEngine;

// Token: 0x020002F8 RID: 760
[RequireComponent(typeof(HitboxSystem))]
public class EZSoftBoneHitboxSystemCollider : EZSoftBoneColliderBase, IClientComponent
{
	// Token: 0x0400177C RID: 6012
	public float radius = 2f;

	// Token: 0x06001E46 RID: 7750 RVA: 0x000063A5 File Offset: 0x000045A5
	public override void Collide(ref Vector3 position, float spacing)
	{
	}
}
