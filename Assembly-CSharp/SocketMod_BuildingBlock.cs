using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class SocketMod_BuildingBlock : SocketMod
{
	// Token: 0x04001568 RID: 5480
	public float sphereRadius = 1f;

	// Token: 0x04001569 RID: 5481
	public LayerMask layerMask;

	// Token: 0x0400156A RID: 5482
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x0400156B RID: 5483
	public bool wantsCollide;

	// Token: 0x06001CB6 RID: 7350 RVA: 0x000C72E0 File Offset: 0x000C54E0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x000C7350 File Offset: 0x000C5550
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities<BuildingBlock>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		bool flag = list.Count > 0;
		if (flag && this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return true;
		}
		if (flag && !this.wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return false;
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return !this.wantsCollide;
	}
}
