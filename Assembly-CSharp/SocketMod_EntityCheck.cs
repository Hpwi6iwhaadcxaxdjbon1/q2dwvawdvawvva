using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

// Token: 0x02000275 RID: 629
public class SocketMod_EntityCheck : SocketMod
{
	// Token: 0x0400156C RID: 5484
	public float sphereRadius = 1f;

	// Token: 0x0400156D RID: 5485
	public LayerMask layerMask;

	// Token: 0x0400156E RID: 5486
	public QueryTriggerInteraction queryTriggers;

	// Token: 0x0400156F RID: 5487
	public BaseEntity[] entityTypes;

	// Token: 0x04001570 RID: 5488
	public bool wantsCollide;

	// Token: 0x06001CB9 RID: 7353 RVA: 0x000C73F4 File Offset: 0x000C55F4
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = (this.wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, this.sphereRadius);
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x000C7464 File Offset: 0x000C5664
	public override bool DoCheck(Construction.Placement place)
	{
		Vector3 position = place.position + place.rotation * this.worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(position, this.sphereRadius, list, this.layerMask.value, this.queryTriggers);
		using (List<BaseEntity>.Enumerator enumerator = list.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				BaseEntity ent = enumerator.Current;
				bool flag = this.entityTypes.Any((BaseEntity x) => x.prefabID == ent.prefabID);
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
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !this.wantsCollide;
	}
}
