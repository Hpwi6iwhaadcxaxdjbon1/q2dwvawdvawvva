using System;
using UnityEngine;

// Token: 0x020002ED RID: 749
public class TriggerVehicleDrag : TriggerBase, IServerComponent
{
	// Token: 0x04001760 RID: 5984
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;

	// Token: 0x04001761 RID: 5985
	public float vehicleDrag;

	// Token: 0x06001E07 RID: 7687 RVA: 0x000CD174 File Offset: 0x000CB374
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
		if (this.losEyes != null)
		{
			if (this.entityContents != null && this.entityContents.Contains(baseEntity))
			{
				return baseEntity.gameObject;
			}
			if (!baseEntity.IsVisible(this.losEyes.transform.position, baseEntity.CenterPoint(), float.PositiveInfinity))
			{
				return null;
			}
		}
		return baseEntity.gameObject;
	}
}
