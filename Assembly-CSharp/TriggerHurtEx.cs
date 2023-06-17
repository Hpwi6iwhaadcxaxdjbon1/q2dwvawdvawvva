using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000746 RID: 1862
public class TriggerHurtEx : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x04002A2D RID: 10797
	public float repeatRate = 0.1f;

	// Token: 0x04002A2E RID: 10798
	[Header("On Enter")]
	public List<DamageTypeEntry> damageOnEnter;

	// Token: 0x04002A2F RID: 10799
	public GameObjectRef effectOnEnter;

	// Token: 0x04002A30 RID: 10800
	public TriggerHurtEx.HurtType hurtTypeOnEnter;

	// Token: 0x04002A31 RID: 10801
	[Header("On Timer (damage per second)")]
	public List<DamageTypeEntry> damageOnTimer;

	// Token: 0x04002A32 RID: 10802
	public GameObjectRef effectOnTimer;

	// Token: 0x04002A33 RID: 10803
	public TriggerHurtEx.HurtType hurtTypeOnTimer;

	// Token: 0x04002A34 RID: 10804
	[Header("On Move (damage per meter)")]
	public List<DamageTypeEntry> damageOnMove;

	// Token: 0x04002A35 RID: 10805
	public GameObjectRef effectOnMove;

	// Token: 0x04002A36 RID: 10806
	public TriggerHurtEx.HurtType hurtTypeOnMove;

	// Token: 0x04002A37 RID: 10807
	[Header("On Leave")]
	public List<DamageTypeEntry> damageOnLeave;

	// Token: 0x04002A38 RID: 10808
	public GameObjectRef effectOnLeave;

	// Token: 0x04002A39 RID: 10809
	public TriggerHurtEx.HurtType hurtTypeOnLeave;

	// Token: 0x04002A3A RID: 10810
	public bool damageEnabled = true;

	// Token: 0x04002A3B RID: 10811
	internal Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo> entityInfo;

	// Token: 0x04002A3C RID: 10812
	internal List<BaseEntity> entityAddList;

	// Token: 0x04002A3D RID: 10813
	internal List<BaseEntity> entityLeaveList;

	// Token: 0x0600341F RID: 13343 RVA: 0x00142DB8 File Offset: 0x00140FB8
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x00142E08 File Offset: 0x00141008
	internal void DoDamage(BaseEntity ent, TriggerHurtEx.HurtType type, List<DamageTypeEntry> damage, GameObjectRef effect, float multiply = 1f)
	{
		if (!this.damageEnabled)
		{
			return;
		}
		using (TimeWarning.New("TriggerHurtEx.DoDamage", 0))
		{
			if (damage != null && damage.Count > 0)
			{
				BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
				if (baseCombatEntity)
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damage);
					hitInfo.damageTypes.ScaleAll(multiply);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.Initiator = base.gameObject.ToBaseEntity();
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					if (type == TriggerHurtEx.HurtType.Simple)
					{
						baseCombatEntity.Hurt(hitInfo);
					}
					else
					{
						baseCombatEntity.OnAttacked(hitInfo);
					}
				}
			}
			if (effect.isValid)
			{
				Effect.server.Run(effect.resourcePath, ent, StringPool.closest, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x00142F08 File Offset: 0x00141108
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityAddList == null)
		{
			this.entityAddList = new List<BaseEntity>();
		}
		this.entityAddList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x00142F5C File Offset: 0x0014115C
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityLeaveList == null)
		{
			this.entityLeaveList = new List<BaseEntity>();
		}
		this.entityLeaveList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x00142FB0 File Offset: 0x001411B0
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x00142FD0 File Offset: 0x001411D0
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06003425 RID: 13349 RVA: 0x00142FE4 File Offset: 0x001411E4
	private void OnTick()
	{
		this.ProcessQueues();
		if (this.entityInfo != null)
		{
			foreach (KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo> keyValuePair in this.entityInfo.ToArray<KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo>>())
			{
				if (keyValuePair.Key.IsValid())
				{
					Vector3 position = keyValuePair.Key.transform.position;
					float magnitude = (position - keyValuePair.Value.lastPosition).magnitude;
					if (magnitude > 0.01f)
					{
						keyValuePair.Value.lastPosition = position;
						this.DoDamage(keyValuePair.Key, this.hurtTypeOnMove, this.damageOnMove, this.effectOnMove, magnitude);
					}
					this.DoDamage(keyValuePair.Key, this.hurtTypeOnTimer, this.damageOnTimer, this.effectOnTimer, this.repeatRate);
				}
			}
		}
	}

	// Token: 0x06003426 RID: 13350 RVA: 0x001430C8 File Offset: 0x001412C8
	private void ProcessQueues()
	{
		if (this.entityAddList != null)
		{
			foreach (BaseEntity baseEntity in this.entityAddList)
			{
				if (baseEntity.IsValid())
				{
					this.DoDamage(baseEntity, this.hurtTypeOnEnter, this.damageOnEnter, this.effectOnEnter, 1f);
					if (this.entityInfo == null)
					{
						this.entityInfo = new Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo>();
					}
					if (!this.entityInfo.ContainsKey(baseEntity))
					{
						this.entityInfo.Add(baseEntity, new TriggerHurtEx.EntityTriggerInfo
						{
							lastPosition = baseEntity.transform.position
						});
					}
				}
			}
			this.entityAddList = null;
		}
		if (this.entityLeaveList != null)
		{
			foreach (BaseEntity baseEntity2 in this.entityLeaveList)
			{
				if (baseEntity2.IsValid())
				{
					this.DoDamage(baseEntity2, this.hurtTypeOnLeave, this.damageOnLeave, this.effectOnLeave, 1f);
					if (this.entityInfo != null)
					{
						this.entityInfo.Remove(baseEntity2);
						if (this.entityInfo.Count == 0)
						{
							this.entityInfo = null;
						}
					}
				}
			}
			this.entityLeaveList.Clear();
		}
	}

	// Token: 0x02000E53 RID: 3667
	public enum HurtType
	{
		// Token: 0x04004B15 RID: 19221
		Simple,
		// Token: 0x04004B16 RID: 19222
		IncludeBleedingAndScreenShake
	}

	// Token: 0x02000E54 RID: 3668
	public class EntityTriggerInfo
	{
		// Token: 0x04004B17 RID: 19223
		public Vector3 lastPosition;
	}
}
