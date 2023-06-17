using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020005F5 RID: 1525
public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
	// Token: 0x04002509 RID: 9481
	public float radius = 0.5f;

	// Token: 0x0400250A RID: 9482
	public DamageTypeEntry damage;

	// Token: 0x0400250B RID: 9483
	public GameObjectRef effect;

	// Token: 0x0400250C RID: 9484
	public bool ignoreHitObject = true;

	// Token: 0x06002D73 RID: 11635 RVA: 0x001115DC File Offset: 0x0010F7DC
	public override void ServerProjectileHit(HitInfo info)
	{
		if (this.effect.isValid)
		{
			Effect.server.Run(this.effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld, null, false);
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		List<BaseCombatEntity> list2 = Pool.GetList<BaseCombatEntity>();
		Vis.Entities<BaseCombatEntity>(info.HitPositionWorld, this.radius, list2, 1236478737, QueryTriggerInteraction.Collide);
		foreach (BaseCombatEntity baseCombatEntity in list2)
		{
			if (baseCombatEntity.isServer && !list.Contains(baseCombatEntity) && (!(baseCombatEntity == info.HitEntity) || !this.ignoreHitObject))
			{
				baseCombatEntity.CenterPoint();
				Vector3 a = baseCombatEntity.ClosestPoint(info.HitPositionWorld);
				float num = Vector3.Distance(a, info.HitPositionWorld) / this.radius;
				if (num <= 1f)
				{
					float num2 = 1f - num;
					if (baseCombatEntity.IsVisibleAndCanSee(info.HitPositionWorld - info.ProjectileVelocity.normalized * 0.1f, float.PositiveInfinity) && baseCombatEntity.IsVisibleAndCanSee(info.HitPositionWorld - (a - info.HitPositionWorld).normalized * 0.1f, float.PositiveInfinity))
					{
						list.Add(baseCombatEntity);
						baseCombatEntity.OnAttacked(new HitInfo(info.Initiator, baseCombatEntity, this.damage.type, this.damage.amount * num2));
					}
				}
			}
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		Pool.FreeList<BaseCombatEntity>(ref list2);
	}
}
