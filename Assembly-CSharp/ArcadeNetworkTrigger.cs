using System;
using UnityEngine;

// Token: 0x02000147 RID: 327
public class ArcadeNetworkTrigger : TriggerBase
{
	// Token: 0x06001705 RID: 5893 RVA: 0x000AFF38 File Offset: 0x000AE138
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
		return baseEntity.gameObject;
	}
}
