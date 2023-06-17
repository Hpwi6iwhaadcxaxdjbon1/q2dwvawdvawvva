using System;
using UnityEngine;

// Token: 0x02000464 RID: 1124
public class BaseTrapTrigger : TriggerBase
{
	// Token: 0x04001D79 RID: 7545
	public BaseTrap _trap;

	// Token: 0x0600252F RID: 9519 RVA: 0x000EB1A4 File Offset: 0x000E93A4
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

	// Token: 0x06002530 RID: 9520 RVA: 0x000EB1E7 File Offset: 0x000E93E7
	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		base.OnObjectAdded(obj, col);
		this._trap.ObjectEntered(obj);
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x000EB1FD File Offset: 0x000E93FD
	internal override void OnEmpty()
	{
		base.OnEmpty();
		this._trap.OnEmpty();
	}
}
