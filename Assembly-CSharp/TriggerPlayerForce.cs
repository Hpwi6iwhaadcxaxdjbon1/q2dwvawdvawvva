using System;
using UnityEngine;

// Token: 0x02000592 RID: 1426
public class TriggerPlayerForce : TriggerBase, IServerComponent
{
	// Token: 0x0400233D RID: 9021
	public BoxCollider triggerCollider;

	// Token: 0x0400233E RID: 9022
	public float pushVelocity = 5f;

	// Token: 0x0400233F RID: 9023
	public bool requireUpAxis;

	// Token: 0x04002340 RID: 9024
	private const float HACK_DISABLE_TIME = 4f;

	// Token: 0x06002B7E RID: 11134 RVA: 0x00107B0C File Offset: 0x00105D0C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity != null)
		{
			return baseEntity.gameObject;
		}
		return null;
	}

	// Token: 0x06002B7F RID: 11135 RVA: 0x00107B45 File Offset: 0x00105D45
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.HackDisableTick), 0f, 3.75f);
	}

	// Token: 0x06002B80 RID: 11136 RVA: 0x00107B63 File Offset: 0x00105D63
	internal override void OnEmpty()
	{
		base.OnEmpty();
		base.CancelInvoke(new Action(this.HackDisableTick));
	}

	// Token: 0x06002B81 RID: 11137 RVA: 0x00107B7D File Offset: 0x00105D7D
	protected override void OnDisable()
	{
		base.CancelInvoke(new Action(this.HackDisableTick));
		base.OnDisable();
	}

	// Token: 0x06002B82 RID: 11138 RVA: 0x00106A81 File Offset: 0x00104C81
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	// Token: 0x06002B83 RID: 11139 RVA: 0x00107B98 File Offset: 0x00105D98
	private void HackDisableTick()
	{
		if (this.entityContents == null || !base.enabled)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents)
		{
			if (this.IsInterested(baseEntity))
			{
				BasePlayer basePlayer = baseEntity.ToPlayer();
				if (basePlayer != null && !basePlayer.IsNpc)
				{
					basePlayer.PauseVehicleNoClipDetection(4f);
					basePlayer.PauseSpeedHackDetection(4f);
				}
			}
		}
	}

	// Token: 0x06002B84 RID: 11140 RVA: 0x00107C2C File Offset: 0x00105E2C
	protected void FixedUpdate()
	{
		if (this.entityContents != null)
		{
			foreach (BaseEntity baseEntity in this.entityContents)
			{
				if ((!this.requireUpAxis || Vector3.Dot(baseEntity.transform.up, base.transform.up) >= 0f) && this.IsInterested(baseEntity))
				{
					Vector3 velocity = this.GetPushVelocity(baseEntity.gameObject);
					baseEntity.ApplyInheritedVelocity(velocity);
				}
			}
		}
	}

	// Token: 0x06002B85 RID: 11141 RVA: 0x00107CC8 File Offset: 0x00105EC8
	private Vector3 GetPushVelocity(GameObject obj)
	{
		Vector3 a = -(this.triggerCollider.bounds.center - obj.transform.position);
		a.Normalize();
		a.y = 0.2f;
		a.Normalize();
		return a * this.pushVelocity;
	}

	// Token: 0x06002B86 RID: 11142 RVA: 0x00107D24 File Offset: 0x00105F24
	private bool IsInterested(BaseEntity entity)
	{
		if (entity == null || entity.isClient)
		{
			return false;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		return !(basePlayer != null) || (((!basePlayer.IsAdmin && !basePlayer.IsDeveloper) || !basePlayer.IsFlying) && (basePlayer != null && basePlayer.IsAlive()) && !basePlayer.isMounted);
	}
}
