using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class FireBall : BaseEntity, ISplashable
{
	// Token: 0x04001A80 RID: 6784
	public float lifeTimeMin = 20f;

	// Token: 0x04001A81 RID: 6785
	public float lifeTimeMax = 40f;

	// Token: 0x04001A82 RID: 6786
	public ParticleSystem[] movementSystems;

	// Token: 0x04001A83 RID: 6787
	public ParticleSystem[] restingSystems;

	// Token: 0x04001A84 RID: 6788
	[NonSerialized]
	public float generation;

	// Token: 0x04001A85 RID: 6789
	public GameObjectRef spreadSubEntity;

	// Token: 0x04001A86 RID: 6790
	public float tickRate = 0.5f;

	// Token: 0x04001A87 RID: 6791
	public float damagePerSecond = 2f;

	// Token: 0x04001A88 RID: 6792
	public float radius = 0.5f;

	// Token: 0x04001A89 RID: 6793
	public int waterToExtinguish = 200;

	// Token: 0x04001A8A RID: 6794
	public bool canMerge;

	// Token: 0x04001A8B RID: 6795
	public LayerMask AttackLayers = 1219701521;

	// Token: 0x04001A8C RID: 6796
	public bool ignoreNPC;

	// Token: 0x04001A8D RID: 6797
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x04001A8E RID: 6798
	private float deathTime;

	// Token: 0x04001A8F RID: 6799
	private int wetness;

	// Token: 0x04001A90 RID: 6800
	private float spawnTime;

	// Token: 0x04001A91 RID: 6801
	private Vector3 delayedVelocity;

	// Token: 0x0600227C RID: 8828 RVA: 0x000DE105 File Offset: 0x000DC305
	public void SetDelayedVelocity(Vector3 delayed)
	{
		if (this.delayedVelocity != Vector3.zero)
		{
			return;
		}
		this.delayedVelocity = delayed;
		base.Invoke(new Action(this.ApplyDelayedVelocity), 0.1f);
	}

	// Token: 0x0600227D RID: 8829 RVA: 0x000DE138 File Offset: 0x000DC338
	private void ApplyDelayedVelocity()
	{
		this.SetVelocity(this.delayedVelocity);
		this.delayedVelocity = Vector3.zero;
	}

	// Token: 0x0600227E RID: 8830 RVA: 0x000DE154 File Offset: 0x000DC354
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), this.tickRate);
		float num = UnityEngine.Random.Range(this.lifeTimeMin, this.lifeTimeMax);
		float num2 = num * UnityEngine.Random.Range(0.9f, 1.1f);
		base.Invoke(new Action(this.Extinguish), num2);
		base.Invoke(new Action(this.TryToSpread), num * UnityEngine.Random.Range(0.3f, 0.5f));
		this.deathTime = Time.realtimeSinceStartup + num2;
		this.spawnTime = Time.realtimeSinceStartup;
	}

	// Token: 0x0600227F RID: 8831 RVA: 0x000DE200 File Offset: 0x000DC400
	public float GetDeathTime()
	{
		return this.deathTime;
	}

	// Token: 0x06002280 RID: 8832 RVA: 0x000DE208 File Offset: 0x000DC408
	public void AddLife(float amountToAdd)
	{
		float time = Mathf.Clamp(this.GetDeathTime() + amountToAdd, 0f, this.MaxLifeTime());
		base.Invoke(new Action(this.Extinguish), time);
		this.deathTime = time;
	}

	// Token: 0x06002281 RID: 8833 RVA: 0x000DE248 File Offset: 0x000DC448
	public float MaxLifeTime()
	{
		return this.lifeTimeMax * 2.5f;
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x000DE258 File Offset: 0x000DC458
	public float TimeLeft()
	{
		float num = this.deathTime - Time.realtimeSinceStartup;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x000DE284 File Offset: 0x000DC484
	public void TryToSpread()
	{
		float num = 0.9f - this.generation * 0.1f;
		if (UnityEngine.Random.Range(0f, 1f) >= num)
		{
			return;
		}
		if (this.spreadSubEntity.isValid)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.spreadSubEntity.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.transform.position = base.transform.position + Vector3.up * 0.25f;
				baseEntity.Spawn();
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.up, true);
				baseEntity.creatorEntity = ((this.creatorEntity == null) ? baseEntity : this.creatorEntity);
				baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(5f, 8f));
				baseEntity.SendMessage("SetGeneration", this.generation + 1f);
			}
		}
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x000DE394 File Offset: 0x000DC594
	public void SetGeneration(int gen)
	{
		this.generation = (float)gen;
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x000DE3A0 File Offset: 0x000DC5A0
	public void Think()
	{
		if (!base.isServer)
		{
			return;
		}
		this.SetResting(Vector3.Distance(this.lastPos, base.transform.localPosition) < 0.25f);
		this.lastPos = base.transform.localPosition;
		if (this.IsResting())
		{
			this.DoRadialDamage();
		}
		if (this.WaterFactor() > 0.5f)
		{
			this.Extinguish();
		}
		if (this.wetness > this.waterToExtinguish)
		{
			this.Extinguish();
		}
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x000DE420 File Offset: 0x000DC620
	public void DoRadialDamage()
	{
		List<Collider> list = Pool.GetList<Collider>();
		Vector3 position = base.transform.position + new Vector3(0f, this.radius * 0.75f, 0f);
		Vis.Colliders<Collider>(position, this.radius, list, this.AttackLayers, QueryTriggerInteraction.Collide);
		HitInfo hitInfo = new HitInfo();
		hitInfo.DoHitEffects = true;
		hitInfo.DidHit = true;
		hitInfo.HitBone = 0U;
		hitInfo.Initiator = ((this.creatorEntity == null) ? base.gameObject.ToBaseEntity() : this.creatorEntity);
		hitInfo.PointStart = base.transform.position;
		foreach (Collider collider in list)
		{
			if (!collider.isTrigger || (collider.gameObject.layer != 29 && collider.gameObject.layer != 18))
			{
				BaseCombatEntity baseCombatEntity = collider.gameObject.ToBaseEntity() as BaseCombatEntity;
				if (!(baseCombatEntity == null) && baseCombatEntity.isServer && baseCombatEntity.IsAlive() && (!this.ignoreNPC || !baseCombatEntity.IsNpc) && baseCombatEntity.IsVisible(position, float.PositiveInfinity))
				{
					if (baseCombatEntity is BasePlayer)
					{
						Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseCombatEntity, 0U, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
					}
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					hitInfo.HitPositionWorld = baseCombatEntity.transform.position;
					hitInfo.damageTypes.Set(DamageType.Heat, this.damagePerSecond * this.tickRate);
					baseCombatEntity.OnAttacked(hitInfo);
				}
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x000DE620 File Offset: 0x000DC820
	public bool CanMerge()
	{
		return this.canMerge && this.TimeLeft() < this.MaxLifeTime() * 0.8f;
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x000DE640 File Offset: 0x000DC840
	public float TimeAlive()
	{
		return Time.realtimeSinceStartup - this.spawnTime;
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x000DE650 File Offset: 0x000DC850
	public void SetResting(bool isResting)
	{
		if (isResting != this.IsResting() && isResting && this.TimeAlive() > 1f && this.CanMerge())
		{
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(base.transform.position, 0.5f, list, 512, QueryTriggerInteraction.Collide);
			foreach (Collider collider in list)
			{
				BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					FireBall fireBall = baseEntity.ToServer<FireBall>();
					if (fireBall && fireBall.CanMerge() && fireBall != this)
					{
						fireBall.Invoke(new Action(this.Extinguish), 1f);
						fireBall.canMerge = false;
						this.AddLife(fireBall.TimeLeft() * 0.25f);
					}
				}
			}
			Pool.FreeList<Collider>(ref list);
		}
		base.SetFlag(BaseEntity.Flags.OnFire, isResting, false, true);
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x000DE75C File Offset: 0x000DC95C
	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x000DE77F File Offset: 0x000DC97F
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed;
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x000DE78A File Offset: 0x000DC98A
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.wetness += amount;
		return amount;
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x0000326F File Offset: 0x0000146F
	public bool IsResting()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x000DE79B File Offset: 0x000DC99B
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}
}
