using System;
using UnityEngine;

// Token: 0x02000587 RID: 1415
public class TriggerLadder : TriggerBase
{
	// Token: 0x06002B52 RID: 11090 RVA: 0x00107110 File Offset: 0x00105310
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
		if (baseEntity as BasePlayer == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
