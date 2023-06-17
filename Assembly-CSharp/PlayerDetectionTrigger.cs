using System;
using UnityEngine;

// Token: 0x02000115 RID: 277
public class PlayerDetectionTrigger : TriggerBase
{
	// Token: 0x04000E84 RID: 3716
	public BaseDetector myDetector;

	// Token: 0x06001643 RID: 5699 RVA: 0x000AD498 File Offset: 0x000AB698
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

	// Token: 0x06001644 RID: 5700 RVA: 0x000AD4DB File Offset: 0x000AB6DB
	internal override void OnObjects()
	{
		base.OnObjects();
		this.myDetector.OnObjects();
	}

	// Token: 0x06001645 RID: 5701 RVA: 0x000AD4EE File Offset: 0x000AB6EE
	internal override void OnEmpty()
	{
		base.OnEmpty();
		this.myDetector.OnEmpty();
	}
}
