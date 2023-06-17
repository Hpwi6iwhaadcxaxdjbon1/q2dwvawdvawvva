using System;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class BaseAnimalNPC : BaseNpc, IAIAttack, IAITirednessAbove, IAISleep, IAIHungerAbove, IAISenses, IThinker
{
	// Token: 0x0400136D RID: 4973
	public string deathStatName = "";

	// Token: 0x0400136E RID: 4974
	protected AnimalBrain brain;

	// Token: 0x06001B6B RID: 7019 RVA: 0x000C21B7 File Offset: 0x000C03B7
	public override void ServerInit()
	{
		base.ServerInit();
		this.brain = base.GetComponent<AnimalBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddAnimal(this);
	}

	// Token: 0x06001B6C RID: 7020 RVA: 0x000C21DA File Offset: 0x000C03DA
	internal override void DoServerDestroy()
	{
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.RemoveAnimal(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001B6D RID: 7021 RVA: 0x000C21F1 File Offset: 0x000C03F1
	public virtual void TryThink()
	{
		if (this.brain.ShouldServerThink())
		{
			this.brain.DoThink();
		}
	}

	// Token: 0x06001B6E RID: 7022 RVA: 0x000C220C File Offset: 0x000C040C
	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (hitInfo != null)
		{
			BasePlayer initiatorPlayer = hitInfo.InitiatorPlayer;
			if (initiatorPlayer != null)
			{
				initiatorPlayer.GiveAchievement("KILL_ANIMAL");
				if (!string.IsNullOrEmpty(this.deathStatName))
				{
					initiatorPlayer.stats.Add(this.deathStatName, 1, (Stats)5);
					initiatorPlayer.stats.Save(false);
				}
				initiatorPlayer.LifeStoryKill(this);
			}
		}
		base.OnKilled(null);
	}

	// Token: 0x06001B6F RID: 7023 RVA: 0x000C2271 File Offset: 0x000C0471
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isServer && info.InitiatorPlayer && !info.damageTypes.IsMeleeType())
		{
			info.InitiatorPlayer.LifeStoryShotHit(info.Weapon);
		}
	}

	// Token: 0x06001B70 RID: 7024 RVA: 0x000C22AD File Offset: 0x000C04AD
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06001B71 RID: 7025 RVA: 0x000C22BC File Offset: 0x000C04BC
	public bool CanAttack(BaseEntity entity)
	{
		if (entity == null)
		{
			return false;
		}
		if (this.NeedsToReload())
		{
			return false;
		}
		if (this.IsOnCooldown())
		{
			return false;
		}
		float num;
		if (!this.IsTargetInRange(entity, out num))
		{
			return false;
		}
		if (!this.CanSeeTarget(entity))
		{
			return false;
		}
		BasePlayer basePlayer = entity as BasePlayer;
		BaseVehicle baseVehicle = (basePlayer != null) ? basePlayer.GetMountedVehicle() : null;
		return !(baseVehicle != null) || !(baseVehicle is BaseModularVehicle);
	}

	// Token: 0x06001B72 RID: 7026 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001B73 RID: 7027 RVA: 0x000C232E File Offset: 0x000C052E
	public float EngagementRange()
	{
		return this.AttackRange * this.brain.AttackRangeMultiplier;
	}

	// Token: 0x06001B74 RID: 7028 RVA: 0x000C2342 File Offset: 0x000C0542
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.AttackPosition);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001B75 RID: 7029 RVA: 0x0006534C File Offset: 0x0006354C
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001B76 RID: 7030 RVA: 0x0002A2F3 File Offset: 0x000284F3
	public bool Reload()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x000C236C File Offset: 0x000C056C
	public bool StartAttacking(BaseEntity target)
	{
		BaseCombatEntity baseCombatEntity = target as BaseCombatEntity;
		if (baseCombatEntity == null)
		{
			return false;
		}
		base.Attack(baseCombatEntity);
		return true;
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x00036489 File Offset: 0x00034689
	public float CooldownDuration()
	{
		return this.AttackRate;
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x000C2393 File Offset: 0x000C0593
	public bool IsOnCooldown()
	{
		return !base.AttackReady();
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x000C239E File Offset: 0x000C059E
	public bool IsTirednessAbove(float value)
	{
		return 1f - this.Sleep > value;
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x000C23AF File Offset: 0x000C05AF
	public void StartSleeping()
	{
		base.SetFact(BaseNpc.Facts.IsSleeping, 1, true, true);
	}

	// Token: 0x06001B7D RID: 7037 RVA: 0x000C23BB File Offset: 0x000C05BB
	public void StopSleeping()
	{
		base.SetFact(BaseNpc.Facts.IsSleeping, 0, true, true);
	}

	// Token: 0x06001B7E RID: 7038 RVA: 0x000C23C7 File Offset: 0x000C05C7
	public bool IsHungerAbove(float value)
	{
		return 1f - this.Energy.Level > value;
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x000C23E0 File Offset: 0x000C05E0
	public bool IsThreat(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		if (baseNpc != null)
		{
			return baseNpc.Stats.Family != this.Stats.Family && base.IsAfraidOf(baseNpc.Stats.Family);
		}
		BasePlayer basePlayer = entity as BasePlayer;
		return basePlayer != null && base.IsAfraidOf(basePlayer.Family);
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000C2448 File Offset: 0x000C0648
	public bool IsTarget(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		return (!(baseNpc != null) || baseNpc.Stats.Family != this.Stats.Family) && !this.IsThreat(entity);
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x000BD950 File Offset: 0x000BBB50
	public bool IsFriendly(BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001B82 RID: 7042 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetAmmoFraction()
	{
		return 1f;
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}
}
