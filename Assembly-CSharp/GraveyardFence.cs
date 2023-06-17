using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class GraveyardFence : SimpleBuildingBlock
{
	// Token: 0x04000FFE RID: 4094
	public BoxCollider[] pillars;

	// Token: 0x06001743 RID: 5955 RVA: 0x000B1099 File Offset: 0x000AF299
	public override void ServerInit()
	{
		base.ServerInit();
		this.UpdatePillars();
	}

	// Token: 0x06001744 RID: 5956 RVA: 0x000B10A8 File Offset: 0x000AF2A8
	public override void DestroyShared()
	{
		base.DestroyShared();
		List<GraveyardFence> list = Pool.GetList<GraveyardFence>();
		Vis.Entities<GraveyardFence>(base.transform.position, 5f, list, 2097152, QueryTriggerInteraction.Collide);
		foreach (GraveyardFence graveyardFence in list)
		{
			graveyardFence.UpdatePillars();
		}
		Pool.FreeList<GraveyardFence>(ref list);
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x000B1124 File Offset: 0x000AF324
	public virtual void UpdatePillars()
	{
		foreach (BoxCollider boxCollider in this.pillars)
		{
			boxCollider.gameObject.SetActive(true);
			foreach (Collider collider in Physics.OverlapBox(boxCollider.transform.TransformPoint(boxCollider.center), boxCollider.size * 0.5f, boxCollider.transform.rotation, 2097152))
			{
				if (collider.CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
					if (!(baseEntity == null) && !base.EqualNetID(baseEntity) && collider != boxCollider)
					{
						boxCollider.gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
