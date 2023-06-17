using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000276 RID: 630
public class SocketMod_EntityType : SocketMod
{
	// Token: 0x04001571 RID: 5489
	public float sphereRadius = 1f;

	// Token: 0x04001572 RID: 5490
	public LayerMask layerMask;

	// Token: 0x04001573 RID: 5491
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x04001574 RID: 5492
	public BaseEntity searchType;

	// Token: 0x04001575 RID: 5493
	public bool wantsCollide;

	// Token: 0x06001CBC RID: 7356 RVA: 0x000C7564 File Offset: 0x000C5764
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x000C75D4 File Offset: 0x000C57D4
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		foreach (BaseEntity baseEntity in list)
		{
			bool flag = baseEntity.GetType().IsAssignableFrom(this.searchType.GetType());
			if (flag && this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (flag && !this.wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}
}
