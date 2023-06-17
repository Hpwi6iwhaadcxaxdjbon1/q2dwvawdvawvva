using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x0200027A RID: 634
public class SocketMod_PlantCheck : SocketMod
{
	// Token: 0x0400157C RID: 5500
	public float sphereRadius = 1f;

	// Token: 0x0400157D RID: 5501
	public LayerMask layerMask;

	// Token: 0x0400157E RID: 5502
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x0400157F RID: 5503
	public bool wantsCollide;

	// Token: 0x06001CC8 RID: 7368 RVA: 0x000C78E0 File Offset: 0x000C5AE0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001CC9 RID: 7369 RVA: 0x000C7950 File Offset: 0x000C5B50
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			GrowableEntity component = baseEntity.GetComponent<GrowableEntity>();
			if (component && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (component && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}
}
