using System;
using UnityEngine;

// Token: 0x02000583 RID: 1411
public class TriggerForce : TriggerBase, IServerComponent
{
	// Token: 0x0400230B RID: 8971
	public const float GravityMultiplier = 0.1f;

	// Token: 0x0400230C RID: 8972
	public const float VelocityLerp = 10f;

	// Token: 0x0400230D RID: 8973
	public const float AngularDrag = 10f;

	// Token: 0x0400230E RID: 8974
	public Vector3 velocity = Vector3.forward;

	// Token: 0x06002B3D RID: 11069 RVA: 0x00106A10 File Offset: 0x00104C10
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

	// Token: 0x06002B3E RID: 11070 RVA: 0x00106A54 File Offset: 0x00104C54
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		Vector3 vector = base.transform.TransformDirection(this.velocity);
		ent.ApplyInheritedVelocity(vector);
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x00106A81 File Offset: 0x00104C81
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	// Token: 0x06002B40 RID: 11072 RVA: 0x00106A98 File Offset: 0x00104C98
	protected void FixedUpdate()
	{
		if (this.entityContents != null)
		{
			Vector3 vector = base.transform.TransformDirection(this.velocity);
			foreach (BaseEntity baseEntity in this.entityContents)
			{
				if (baseEntity != null)
				{
					baseEntity.ApplyInheritedVelocity(vector);
				}
			}
		}
	}
}
