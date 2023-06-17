using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Rust;
using Rust.Ai;
using UnityEngine;

// Token: 0x02000462 RID: 1122
public class TimedExplosive : BaseEntity, ServerProjectile.IProjectileImpact
{
	// Token: 0x04001D65 RID: 7525
	public float timerAmountMin = 10f;

	// Token: 0x04001D66 RID: 7526
	public float timerAmountMax = 20f;

	// Token: 0x04001D67 RID: 7527
	public float minExplosionRadius;

	// Token: 0x04001D68 RID: 7528
	public float explosionRadius = 10f;

	// Token: 0x04001D69 RID: 7529
	public bool explodeOnContact;

	// Token: 0x04001D6A RID: 7530
	public bool canStick;

	// Token: 0x04001D6B RID: 7531
	public bool onlyDamageParent;

	// Token: 0x04001D6C RID: 7532
	public bool BlindAI;

	// Token: 0x04001D6D RID: 7533
	public float aiBlindDuration = 2.5f;

	// Token: 0x04001D6E RID: 7534
	public float aiBlindRange = 4f;

	// Token: 0x04001D6F RID: 7535
	public GameObjectRef explosionEffect;

	// Token: 0x04001D70 RID: 7536
	[Tooltip("Optional: Will fall back to explosionEffect if not assigned.")]
	public GameObjectRef underwaterExplosionEffect;

	// Token: 0x04001D71 RID: 7537
	public GameObjectRef stickEffect;

	// Token: 0x04001D72 RID: 7538
	public GameObjectRef bounceEffect;

	// Token: 0x04001D73 RID: 7539
	public bool explosionUsesForward;

	// Token: 0x04001D74 RID: 7540
	public bool waterCausesExplosion;

	// Token: 0x04001D75 RID: 7541
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x04001D76 RID: 7542
	[NonSerialized]
	private float lastBounceTime;

	// Token: 0x04001D77 RID: 7543
	private CollisionDetectionMode? initialCollisionDetectionMode;

	// Token: 0x04001D78 RID: 7544
	private static BaseEntity[] queryResults = new BaseEntity[64];

	// Token: 0x06002511 RID: 9489 RVA: 0x000EA5CC File Offset: 0x000E87CC
	public void SetDamageScale(float scale)
	{
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			damageTypeEntry.amount *= scale;
		}
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x000EA624 File Offset: 0x000E8824
	public override void ServerInit()
	{
		this.lastBounceTime = Time.time;
		base.ServerInit();
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion || this.AlwaysRunWaterCheck)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	// Token: 0x17000313 RID: 787
	// (get) Token: 0x06002514 RID: 9492 RVA: 0x00007A3C File Offset: 0x00005C3C
	protected virtual bool AlwaysRunWaterCheck
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x000EA682 File Offset: 0x000E8882
	public virtual void WaterCheck()
	{
		if (this.waterCausesExplosion && this.WaterFactor() >= 0.5f)
		{
			this.Explode();
		}
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x000EA69F File Offset: 0x000E889F
	public virtual void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			base.Invoke(new Action(this.Explode), fuseLength);
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		}
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x000EA6CB File Offset: 0x000E88CB
	public virtual float GetRandomTimerTime()
	{
		return UnityEngine.Random.Range(this.timerAmountMin, this.timerAmountMax);
	}

	// Token: 0x06002518 RID: 9496 RVA: 0x000EA6DE File Offset: 0x000E88DE
	public virtual void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.Explode();
	}

	// Token: 0x06002519 RID: 9497 RVA: 0x000EA6E6 File Offset: 0x000E88E6
	public virtual void Explode()
	{
		this.Explode(base.PivotPoint());
	}

	// Token: 0x0600251A RID: 9498 RVA: 0x000EA6F4 File Offset: 0x000E88F4
	public virtual void Explode(Vector3 explosionFxPos)
	{
		Analytics.Azure.OnExplosion(this);
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		bool flag = false;
		if (this.underwaterExplosionEffect.isValid)
		{
			flag = (WaterLevel.GetWaterDepth(base.transform.position, true, null) > 1f);
		}
		if (flag)
		{
			Effect.server.Run(this.underwaterExplosionEffect.resourcePath, explosionFxPos, this.explosionUsesForward ? base.transform.forward : Vector3.up, null, true);
		}
		else if (this.explosionEffect.isValid)
		{
			Effect.server.Run(this.explosionEffect.resourcePath, explosionFxPos, this.explosionUsesForward ? base.transform.forward : Vector3.up, null, true);
		}
		if (this.damageTypes.Count > 0)
		{
			if (this.onlyDamageParent)
			{
				Vector3 vector = base.CenterPoint();
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), vector, this.minExplosionRadius, this.explosionRadius, this.damageTypes, 166144, true);
				BaseEntity parentEntity = base.GetParentEntity();
				BaseCombatEntity baseCombatEntity = parentEntity as BaseCombatEntity;
				while (baseCombatEntity == null && parentEntity != null && parentEntity.HasParent())
				{
					parentEntity = parentEntity.GetParentEntity();
					baseCombatEntity = (parentEntity as BaseCombatEntity);
				}
				if (parentEntity == null || !parentEntity.gameObject.IsOnLayer(Layer.Construction))
				{
					List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
					Vis.Entities<BuildingBlock>(vector, this.explosionRadius, list, 2097152, QueryTriggerInteraction.Ignore);
					BuildingBlock buildingBlock = null;
					float num = float.PositiveInfinity;
					foreach (BuildingBlock buildingBlock2 in list)
					{
						if (!buildingBlock2.isClient && !buildingBlock2.IsDestroyed && buildingBlock2.healthFraction > 0f)
						{
							float num2 = Vector3.Distance(buildingBlock2.ClosestPoint(vector), vector);
							if (num2 < num && buildingBlock2.IsVisible(vector, this.explosionRadius))
							{
								buildingBlock = buildingBlock2;
								num = num2;
							}
						}
					}
					if (buildingBlock)
					{
						HitInfo hitInfo = new HitInfo();
						hitInfo.Initiator = this.creatorEntity;
						hitInfo.WeaponPrefab = base.LookupPrefab();
						hitInfo.damageTypes.Add(this.damageTypes);
						hitInfo.PointStart = vector;
						hitInfo.PointEnd = buildingBlock.transform.position;
						float amount = 1f - Mathf.Clamp01((num - this.minExplosionRadius) / (this.explosionRadius - this.minExplosionRadius));
						hitInfo.damageTypes.ScaleAll(amount);
						buildingBlock.Hurt(hitInfo);
					}
					Pool.FreeList<BuildingBlock>(ref list);
				}
				if (baseCombatEntity)
				{
					HitInfo hitInfo2 = new HitInfo();
					hitInfo2.Initiator = this.creatorEntity;
					hitInfo2.WeaponPrefab = base.LookupPrefab();
					hitInfo2.damageTypes.Add(this.damageTypes);
					baseCombatEntity.Hurt(hitInfo2);
				}
				else if (parentEntity != null)
				{
					HitInfo hitInfo3 = new HitInfo();
					hitInfo3.Initiator = this.creatorEntity;
					hitInfo3.WeaponPrefab = base.LookupPrefab();
					hitInfo3.damageTypes.Add(this.damageTypes);
					hitInfo3.PointStart = vector;
					hitInfo3.PointEnd = parentEntity.transform.position;
					parentEntity.OnAttacked(hitInfo3);
				}
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float num3 = 0f;
					foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
					{
						num3 += damageTypeEntry.amount;
					}
					Sense.Stimulate(new Sensation
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = num3,
						InitiatorPlayer = (this.creatorEntity as BasePlayer),
						Initiator = this.creatorEntity
					});
				}
			}
			else
			{
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 1076005121, true);
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float num4 = 0f;
					foreach (DamageTypeEntry damageTypeEntry2 in this.damageTypes)
					{
						num4 += damageTypeEntry2.amount;
					}
					Sense.Stimulate(new Sensation
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = num4,
						InitiatorPlayer = (this.creatorEntity as BasePlayer),
						Initiator = this.creatorEntity
					});
				}
			}
			this.BlindAnyAI();
		}
		if (base.IsDestroyed || base.HasFlag(BaseEntity.Flags.Broken))
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x0600251B RID: 9499 RVA: 0x000EAC48 File Offset: 0x000E8E48
	private void BlindAnyAI()
	{
		if (!this.BlindAI)
		{
			return;
		}
		int brainsInSphere = BaseEntity.Query.Server.GetBrainsInSphere(base.transform.position, 10f, TimedExplosive.queryResults, null);
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity baseEntity = TimedExplosive.queryResults[i];
			if (Vector3.Distance(base.transform.position, baseEntity.transform.position) <= this.aiBlindRange)
			{
				BaseAIBrain component = baseEntity.GetComponent<BaseAIBrain>();
				if (!(component == null))
				{
					BaseEntity brainBaseEntity = component.GetBrainBaseEntity();
					if (!(brainBaseEntity == null) && brainBaseEntity.IsVisible(base.CenterPoint(), float.PositiveInfinity))
					{
						float blinded = this.aiBlindDuration * component.BlindDurationMultiplier * UnityEngine.Random.Range(0.6f, 1.4f);
						component.SetBlinded(blinded);
						TimedExplosive.queryResults[i] = null;
					}
				}
			}
		}
	}

	// Token: 0x0600251C RID: 9500 RVA: 0x000EAD24 File Offset: 0x000E8F24
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.canStick && !this.IsStuck())
		{
			bool flag = true;
			if (hitEntity)
			{
				flag = this.CanStickTo(hitEntity);
				if (!flag)
				{
					Collider component = base.GetComponent<Collider>();
					if (collision.collider != null && component != null)
					{
						Physics.IgnoreCollision(collision.collider, component);
					}
				}
			}
			if (flag)
			{
				this.DoCollisionStick(collision, hitEntity);
			}
		}
		if (this.explodeOnContact && !base.IsBusy())
		{
			this.SetMotionEnabled(false);
			base.SetFlag(BaseEntity.Flags.Busy, true, false, false);
			base.Invoke(new Action(this.Explode), 0.015f);
			return;
		}
		this.DoBounceEffect();
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000EADD0 File Offset: 0x000E8FD0
	public virtual bool CanStickTo(BaseEntity entity)
	{
		DecorDeployable decorDeployable;
		return !entity.TryGetComponent<DecorDeployable>(out decorDeployable) && !(entity is Drone) && !(entity is BaseVehicle);
	}

	// Token: 0x0600251E RID: 9502 RVA: 0x000EAE00 File Offset: 0x000E9000
	private void DoBounceEffect()
	{
		if (!this.bounceEffect.isValid)
		{
			return;
		}
		if (Time.time - this.lastBounceTime < 0.2f)
		{
			return;
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && component.velocity.magnitude < 1f)
		{
			return;
		}
		if (this.bounceEffect.isValid)
		{
			Effect.server.Run(this.bounceEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		this.lastBounceTime = Time.time;
	}

	// Token: 0x0600251F RID: 9503 RVA: 0x000EAE90 File Offset: 0x000E9090
	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		ContactPoint contact = collision.GetContact(0);
		this.DoStick(contact.point, contact.normal, ent, collision.collider);
	}

	// Token: 0x06002520 RID: 9504 RVA: 0x000EAEC0 File Offset: 0x000E90C0
	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component)
		{
			if (this.initialCollisionDetectionMode == null)
			{
				this.initialCollisionDetectionMode = new CollisionDetectionMode?(component.collisionDetectionMode);
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = CollisionDetectionMode.Discrete;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = this.initialCollisionDetectionMode.Value;
			}
		}
	}

	// Token: 0x06002521 RID: 9505 RVA: 0x000EAF2C File Offset: 0x000E912C
	public bool IsStuck()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && !component.isKinematic)
		{
			return false;
		}
		Collider component2 = base.GetComponent<Collider>();
		return (!component2 || !component2.enabled) && this.parentEntity.IsValid(true);
	}

	// Token: 0x06002522 RID: 9506 RVA: 0x000EAF78 File Offset: 0x000E9178
	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider collider)
	{
		if (ent == null)
		{
			return;
		}
		if (ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		this.SetCollisionEnabled(false);
		if (base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		if (collider != null)
		{
			base.SetParent(ent, ent.FindBoneID(collider.transform), true, false);
		}
		else
		{
			base.SetParent(ent, StringPool.closest, true, false);
		}
		if (this.stickEffect.isValid)
		{
			Effect.server.Run(this.stickEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		base.ReceiveCollisionMessages(false);
	}

	// Token: 0x06002523 RID: 9507 RVA: 0x000EB063 File Offset: 0x000E9263
	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		this.SetCollisionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x06002524 RID: 9508 RVA: 0x000EB091 File Offset: 0x000E9291
	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	// Token: 0x06002525 RID: 9509 RVA: 0x000EB099 File Offset: 0x000E9299
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
	}

	// Token: 0x06002526 RID: 9510 RVA: 0x000EB0A2 File Offset: 0x000E92A2
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.parentEntity.IsValid(true))
		{
			this.DoStick(base.transform.position, base.transform.forward, this.parentEntity.Get(true), null);
		}
	}

	// Token: 0x06002527 RID: 9511 RVA: 0x000EB0E1 File Offset: 0x000E92E1
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.explosive != null)
		{
			this.parentEntity.uid = info.msg.explosive.parentid;
		}
	}

	// Token: 0x06002528 RID: 9512 RVA: 0x000EB114 File Offset: 0x000E9314
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}
}
