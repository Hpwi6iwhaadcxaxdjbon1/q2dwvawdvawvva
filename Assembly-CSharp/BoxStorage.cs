using System;
using UnityEngine;

// Token: 0x020003D3 RID: 979
public class BoxStorage : StorageContainer
{
	// Token: 0x060021BC RID: 8636 RVA: 0x000DBB94 File Offset: 0x000D9D94
	public override Vector3 GetDropPosition()
	{
		return base.ClosestPoint(base.GetDropPosition() + base.LastAttackedDir * 10f);
	}

	// Token: 0x060021BD RID: 8637 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x060021BE RID: 8638 RVA: 0x000A77FF File Offset: 0x000A59FF
	public override bool CanPickup(BasePlayer player)
	{
		return this.children.Count == 0 && base.CanPickup(player);
	}
}
