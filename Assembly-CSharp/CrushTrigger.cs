using System;
using UnityEngine;

// Token: 0x02000477 RID: 1143
public class CrushTrigger : TriggerHurt
{
	// Token: 0x04001E00 RID: 7680
	public bool includeNPCs = true;

	// Token: 0x04001E01 RID: 7681
	public bool requireCentreBelowPosition;

	// Token: 0x060025AE RID: 9646 RVA: 0x000EE888 File Offset: 0x000ECA88
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
		if (baseEntity.isClient)
		{
			return null;
		}
		if (!this.includeNPCs && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x060025AF RID: 9647 RVA: 0x000EE8DD File Offset: 0x000ECADD
	protected override bool CanHurt(BaseCombatEntity ent)
	{
		return (!this.requireCentreBelowPosition || ent.CenterPoint().y <= base.transform.position.y) && base.CanHurt(ent);
	}
}
