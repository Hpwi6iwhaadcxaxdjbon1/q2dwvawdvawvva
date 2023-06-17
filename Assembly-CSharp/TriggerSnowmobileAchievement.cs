using System;
using UnityEngine;

// Token: 0x02000597 RID: 1431
public class TriggerSnowmobileAchievement : TriggerBase
{
	// Token: 0x06002BA0 RID: 11168 RVA: 0x001082EC File Offset: 0x001064EC
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity != null && baseEntity.isServer && baseEntity.ToPlayer() != null)
		{
			return baseEntity.gameObject;
		}
		return null;
	}
}
