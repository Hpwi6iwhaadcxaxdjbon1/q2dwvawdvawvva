using System;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public class CapturePointTrigger : TriggerBase
{
	// Token: 0x06002958 RID: 10584 RVA: 0x000FD808 File Offset: 0x000FBA08
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
		if (baseEntity as BasePlayer == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
