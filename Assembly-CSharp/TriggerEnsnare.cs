using System;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class TriggerEnsnare : TriggerBase
{
	// Token: 0x0400230A RID: 8970
	public bool blockHands = true;

	// Token: 0x06002B3B RID: 11067 RVA: 0x001069C8 File Offset: 0x00104BC8
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
		return baseEntity.gameObject;
	}
}
