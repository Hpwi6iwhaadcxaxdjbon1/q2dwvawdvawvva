using System;
using UnityEngine;

// Token: 0x02000407 RID: 1031
public class TargetTrigger : TriggerBase
{
	// Token: 0x04001B0F RID: 6927
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x06002313 RID: 8979 RVA: 0x000E095C File Offset: 0x000DEB5C
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
		if (this.losEyes != null && !baseEntity.IsVisible(this.losEyes.transform.position, baseEntity.CenterPoint(), float.PositiveInfinity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
