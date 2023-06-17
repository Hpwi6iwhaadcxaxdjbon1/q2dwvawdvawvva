using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class DropBox : Mailbox
{
	// Token: 0x04000E4F RID: 3663
	public Transform EyePoint;

	// Token: 0x060015D0 RID: 5584 RVA: 0x000ABAB9 File Offset: 0x000A9CB9
	public override bool PlayerIsOwner(BasePlayer player)
	{
		return this.PlayerBehind(player);
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x000ABAC4 File Offset: 0x000A9CC4
	public bool PlayerBehind(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) <= -0.3f && GamePhysics.LineOfSight(player.eyes.position, this.EyePoint.position, 2162688, null);
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x000ABB30 File Offset: 0x000A9D30
	public bool PlayerInfront(BasePlayer player)
	{
		return Vector3.Dot(base.transform.forward, (player.transform.position - base.transform.position).normalized) >= 0.7f;
	}

	// Token: 0x060015D3 RID: 5587 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}
}
