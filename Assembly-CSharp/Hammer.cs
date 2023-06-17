using System;

// Token: 0x020003B1 RID: 945
public class Hammer : BaseMelee
{
	// Token: 0x06002110 RID: 8464 RVA: 0x000D92EA File Offset: 0x000D74EA
	public override bool CanHit(HitTest info)
	{
		return !(info.HitEntity == null) && !(info.HitEntity is BasePlayer) && info.HitEntity is BaseCombatEntity;
	}

	// Token: 0x06002111 RID: 8465 RVA: 0x000D931C File Offset: 0x000D751C
	public override void DoAttackShared(HitInfo info)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		BaseCombatEntity baseCombatEntity = info.HitEntity as BaseCombatEntity;
		if (baseCombatEntity != null && ownerPlayer != null && base.isServer)
		{
			using (TimeWarning.New("DoRepair", 50))
			{
				baseCombatEntity.DoRepair(ownerPlayer);
			}
		}
		info.DoDecals = false;
		if (base.isServer)
		{
			Effect.server.ImpactEffect(info);
			return;
		}
		Effect.client.ImpactEffect(info);
	}
}
