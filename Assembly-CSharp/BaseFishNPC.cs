using System;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class BaseFishNPC : BaseNpc, IAIAttack, IAISenses, IThinker
{
	// Token: 0x0400136F RID: 4975
	protected FishBrain brain;

	// Token: 0x06001B86 RID: 7046 RVA: 0x000C249C File Offset: 0x000C069C
	public override void ServerInit()
	{
		base.ServerInit();
		this.brain = base.GetComponent<FishBrain>();
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.AddAnimal(this);
	}

	// Token: 0x06001B87 RID: 7047 RVA: 0x000C21DA File Offset: 0x000C03DA
	internal override void DoServerDestroy()
	{
		if (base.isClient)
		{
			return;
		}
		AIThinkManager.RemoveAnimal(this);
		base.DoServerDestroy();
	}

	// Token: 0x06001B88 RID: 7048 RVA: 0x000C24BF File Offset: 0x000C06BF
	public virtual void TryThink()
	{
		if (this.brain.ShouldServerThink())
		{
			this.brain.DoThink();
		}
	}

	// Token: 0x06001B89 RID: 7049 RVA: 0x000C24DC File Offset: 0x000C06DC
	public bool CanAttack(BaseEntity entity)
	{
		float num;
		return !this.IsOnCooldown() && this.IsTargetInRange(entity, out num) && this.CanSeeTarget(entity);
	}

	// Token: 0x06001B8A RID: 7050 RVA: 0x00007A3C File Offset: 0x00005C3C
	public bool NeedsToReload()
	{
		return false;
	}

	// Token: 0x06001B8B RID: 7051 RVA: 0x000C250C File Offset: 0x000C070C
	public float EngagementRange()
	{
		return this.AttackRange * this.brain.AttackRangeMultiplier;
	}

	// Token: 0x06001B8C RID: 7052 RVA: 0x000C2520 File Offset: 0x000C0720
	public bool IsTargetInRange(BaseEntity entity, out float dist)
	{
		dist = Vector3.Distance(entity.transform.position, base.AttackPosition);
		return dist <= this.EngagementRange();
	}

	// Token: 0x06001B8D RID: 7053 RVA: 0x0006534C File Offset: 0x0006354C
	public bool CanSeeTarget(BaseEntity entity)
	{
		return !(entity == null) && entity.IsVisible(this.GetEntity().CenterPoint(), entity.CenterPoint(), float.PositiveInfinity);
	}

	// Token: 0x06001B8E RID: 7054 RVA: 0x0000441C File Offset: 0x0000261C
	public bool Reload()
	{
		return true;
	}

	// Token: 0x06001B8F RID: 7055 RVA: 0x000C2548 File Offset: 0x000C0748
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

	// Token: 0x06001B90 RID: 7056 RVA: 0x000063A5 File Offset: 0x000045A5
	public void StopAttacking()
	{
	}

	// Token: 0x06001B91 RID: 7057 RVA: 0x00036489 File Offset: 0x00034689
	public float CooldownDuration()
	{
		return this.AttackRate;
	}

	// Token: 0x06001B92 RID: 7058 RVA: 0x000C2393 File Offset: 0x000C0593
	public bool IsOnCooldown()
	{
		return !base.AttackReady();
	}

	// Token: 0x06001B93 RID: 7059 RVA: 0x000C2570 File Offset: 0x000C0770
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

	// Token: 0x06001B94 RID: 7060 RVA: 0x000C25D8 File Offset: 0x000C07D8
	public bool IsTarget(BaseEntity entity)
	{
		BaseNpc baseNpc = entity as BaseNpc;
		return (!(baseNpc != null) || baseNpc.Stats.Family != this.Stats.Family) && !this.IsThreat(entity);
	}

	// Token: 0x06001B95 RID: 7061 RVA: 0x000BD950 File Offset: 0x000BBB50
	public bool IsFriendly(BaseEntity entity)
	{
		return !(entity == null) && entity.prefabID == this.prefabID;
	}

	// Token: 0x06001B96 RID: 7062 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetAmmoFraction()
	{
		return 1f;
	}

	// Token: 0x06001B97 RID: 7063 RVA: 0x0002CDA7 File Offset: 0x0002AFA7
	public BaseEntity GetBestTarget()
	{
		return null;
	}

	// Token: 0x06001B98 RID: 7064 RVA: 0x000063A5 File Offset: 0x000045A5
	public void AttackTick(float delta, BaseEntity target, bool targetIsLOS)
	{
	}
}
