using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000586 RID: 1414
public class TriggerHurtNotChild : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x04002312 RID: 8978
	public float DamagePerSecond = 1f;

	// Token: 0x04002313 RID: 8979
	public float DamageTickRate = 4f;

	// Token: 0x04002314 RID: 8980
	public float DamageDelay;

	// Token: 0x04002315 RID: 8981
	public DamageType damageType;

	// Token: 0x04002316 RID: 8982
	public bool ignoreNPC = true;

	// Token: 0x04002317 RID: 8983
	public float npcMultiplier = 1f;

	// Token: 0x04002318 RID: 8984
	public float resourceMultiplier = 1f;

	// Token: 0x04002319 RID: 8985
	public bool triggerHitImpacts = true;

	// Token: 0x0400231A RID: 8986
	public bool RequireUpAxis;

	// Token: 0x0400231B RID: 8987
	public BaseEntity SourceEntity;

	// Token: 0x0400231C RID: 8988
	public bool UseSourceEntityDamageMultiplier = true;

	// Token: 0x0400231D RID: 8989
	public bool ignoreAllVehicleMounted;

	// Token: 0x0400231E RID: 8990
	public float activationDelay;

	// Token: 0x0400231F RID: 8991
	private Dictionary<BaseEntity, float> entryTimes;

	// Token: 0x04002320 RID: 8992
	private TimeSince timeSinceAcivation;

	// Token: 0x04002321 RID: 8993
	private TriggerHurtNotChild.IHurtTriggerUser hurtTiggerUser;

	// Token: 0x06002B48 RID: 11080 RVA: 0x00106C44 File Offset: 0x00104E44
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
		if (this.ignoreNPC && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x00106C99 File Offset: 0x00104E99
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x00106CC0 File Offset: 0x00104EC0
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent != null && this.DamageDelay > 0f)
		{
			if (this.entryTimes == null)
			{
				this.entryTimes = new Dictionary<BaseEntity, float>();
			}
			this.entryTimes.Add(ent, Time.time);
		}
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x00106D0E File Offset: 0x00104F0E
	internal override void OnEntityLeave(BaseEntity ent)
	{
		if (ent != null && this.entryTimes != null)
		{
			this.entryTimes.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	// Token: 0x06002B4C RID: 11084 RVA: 0x00106D35 File Offset: 0x00104F35
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06002B4D RID: 11085 RVA: 0x00106D49 File Offset: 0x00104F49
	protected void OnEnable()
	{
		this.timeSinceAcivation = 0f;
		this.hurtTiggerUser = (this.SourceEntity as TriggerHurtNotChild.IHurtTriggerUser);
	}

	// Token: 0x06002B4E RID: 11086 RVA: 0x00106D6C File Offset: 0x00104F6C
	public new void OnDisable()
	{
		base.CancelInvoke(new Action(this.OnTick));
		base.OnDisable();
	}

	// Token: 0x06002B4F RID: 11087 RVA: 0x00106D88 File Offset: 0x00104F88
	private bool IsInterested(BaseEntity ent)
	{
		if (this.timeSinceAcivation < this.activationDelay)
		{
			return false;
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null)
		{
			if (basePlayer.isMounted)
			{
				BaseVehicle mountedVehicle = basePlayer.GetMountedVehicle();
				if (this.SourceEntity != null && mountedVehicle == this.SourceEntity)
				{
					return false;
				}
				if (this.ignoreAllVehicleMounted && mountedVehicle != null)
				{
					return false;
				}
			}
			if (this.SourceEntity != null && basePlayer.HasEntityInParents(this.SourceEntity))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002B50 RID: 11088 RVA: 0x00106E1C File Offset: 0x0010501C
	private void OnTick()
	{
		if (this.entityContents.IsNullOrEmpty<BaseEntity>())
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		list.AddRange(this.entityContents);
		foreach (BaseEntity baseEntity in list)
		{
			float num;
			if (baseEntity.IsValid() && this.IsInterested(baseEntity) && (this.DamageDelay <= 0f || this.entryTimes == null || !this.entryTimes.TryGetValue(baseEntity, out num) || num + this.DamageDelay <= Time.time) && (!this.RequireUpAxis || Vector3.Dot(baseEntity.transform.up, base.transform.up) >= 0f))
			{
				float num2 = this.DamagePerSecond * 1f / this.DamageTickRate;
				if (this.UseSourceEntityDamageMultiplier && this.hurtTiggerUser != null)
				{
					num2 *= this.hurtTiggerUser.GetDamageMultiplier(baseEntity);
				}
				if (baseEntity.IsNpc)
				{
					num2 *= this.npcMultiplier;
				}
				if (baseEntity is ResourceEntity)
				{
					num2 *= this.resourceMultiplier;
				}
				Vector3 vector = baseEntity.transform.position + Vector3.up * 1f;
				bool flag = baseEntity is BasePlayer || baseEntity is BaseNpc;
				BaseEntity baseEntity2 = null;
				BaseEntity weaponPrefab = null;
				if (this.hurtTiggerUser != null)
				{
					baseEntity2 = this.hurtTiggerUser.GetPlayerDamageInitiator();
					weaponPrefab = this.SourceEntity.LookupPrefab();
				}
				if (baseEntity2 == null)
				{
					if (this.SourceEntity != null)
					{
						baseEntity2 = this.SourceEntity;
					}
					else
					{
						baseEntity2 = base.gameObject.ToBaseEntity();
					}
				}
				HitInfo hitInfo = new HitInfo
				{
					DoHitEffects = true,
					HitEntity = baseEntity,
					HitPositionWorld = vector,
					HitPositionLocal = baseEntity.transform.InverseTransformPoint(vector),
					HitNormalWorld = Vector3.up,
					HitMaterial = (flag ? StringPool.Get("Flesh") : 0U),
					WeaponPrefab = weaponPrefab,
					Initiator = baseEntity2
				};
				hitInfo.damageTypes = new DamageTypeList();
				hitInfo.damageTypes.Set(this.damageType, num2);
				baseEntity.OnAttacked(hitInfo);
				if (this.hurtTiggerUser != null)
				{
					this.hurtTiggerUser.OnHurtTriggerOccupant(baseEntity, this.damageType, num2);
				}
				if (this.triggerHitImpacts)
				{
					Effect.server.ImpactEffect(hitInfo);
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		base.RemoveInvalidEntities();
	}

	// Token: 0x02000D5C RID: 3420
	public interface IHurtTriggerUser
	{
		// Token: 0x060050CB RID: 20683
		BasePlayer GetPlayerDamageInitiator();

		// Token: 0x060050CC RID: 20684
		float GetDamageMultiplier(BaseEntity ent);

		// Token: 0x060050CD RID: 20685
		void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal);
	}
}
