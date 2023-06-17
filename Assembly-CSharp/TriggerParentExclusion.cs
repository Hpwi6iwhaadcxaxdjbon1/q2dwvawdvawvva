using System;
using UnityEngine;

// Token: 0x02000591 RID: 1425
public class TriggerParentExclusion : TriggerBase, IServerComponent
{
	// Token: 0x06002B7C RID: 11132 RVA: 0x00107AC8 File Offset: 0x00105CC8
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
