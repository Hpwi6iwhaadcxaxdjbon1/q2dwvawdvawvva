using System;
using UnityEngine;

// Token: 0x0200059C RID: 1436
public class TriggerWorkbench : TriggerBase
{
	// Token: 0x0400235C RID: 9052
	public Workbench parentBench;

	// Token: 0x06002BB5 RID: 11189 RVA: 0x00108844 File Offset: 0x00106A44
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

	// Token: 0x06002BB6 RID: 11190 RVA: 0x00108887 File Offset: 0x00106A87
	public float WorkbenchLevel()
	{
		return (float)this.parentBench.Workbenchlevel;
	}
}
