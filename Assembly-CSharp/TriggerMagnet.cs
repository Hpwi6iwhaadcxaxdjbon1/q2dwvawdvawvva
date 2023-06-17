using System;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class TriggerMagnet : TriggerBase
{
	// Token: 0x06002655 RID: 9813 RVA: 0x000F13F8 File Offset: 0x000EF5F8
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
		if (!baseEntity.syncPosition)
		{
			return null;
		}
		if (!baseEntity.GetComponent<MagnetLiftable>())
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
