using System;
using UnityEngine;

// Token: 0x02000598 RID: 1432
public class TriggerSubmarineMoonpool : TriggerBase, IServerComponent
{
	// Token: 0x06002BA2 RID: 11170 RVA: 0x00108328 File Offset: 0x00106528
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		BaseSubmarine baseSubmarine;
		if (baseEntity.isServer && (baseSubmarine = (baseEntity as BaseSubmarine)) != null)
		{
			return baseSubmarine.gameObject;
		}
		return null;
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x00108378 File Offset: 0x00106578
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		BaseSubmarine baseSubmarine;
		if ((baseSubmarine = (ent as BaseSubmarine)) != null)
		{
			baseSubmarine.OnSurfacedInMoonpool();
		}
	}
}
