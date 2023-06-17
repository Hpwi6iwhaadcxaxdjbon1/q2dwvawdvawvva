using System;
using UnityEngine;

// Token: 0x0200043A RID: 1082
public class LargeShredderTrigger : TriggerBase
{
	// Token: 0x04001C75 RID: 7285
	public LargeShredder shredder;

	// Token: 0x06002460 RID: 9312 RVA: 0x000E7834 File Offset: 0x000E5A34
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

	// Token: 0x06002461 RID: 9313 RVA: 0x000E7890 File Offset: 0x000E5A90
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		this.shredder.OnEntityEnteredTrigger(ent);
	}
}
