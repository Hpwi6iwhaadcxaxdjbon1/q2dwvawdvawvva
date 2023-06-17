using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000303 RID: 771
public class DirectionalDamageTrigger : TriggerBase
{
	// Token: 0x04001792 RID: 6034
	public float repeatRate = 1f;

	// Token: 0x04001793 RID: 6035
	public List<DamageTypeEntry> damageType;

	// Token: 0x04001794 RID: 6036
	public GameObjectRef attackEffect;

	// Token: 0x06001E83 RID: 7811 RVA: 0x000D03A8 File Offset: 0x000CE5A8
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

	// Token: 0x06001E84 RID: 7812 RVA: 0x000D03F5 File Offset: 0x000CE5F5
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	// Token: 0x06001E85 RID: 7813 RVA: 0x000D0415 File Offset: 0x000CE615
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06001E86 RID: 7814 RVA: 0x000D042C File Offset: 0x000CE62C
	private void OnTick()
	{
		if (this.attackEffect.isValid)
		{
			Effect.server.Run(this.attackEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.entityContents == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents.ToArray<BaseEntity>())
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!(baseCombatEntity == null))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(this.damageType);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					baseCombatEntity.Hurt(hitInfo);
				}
			}
		}
	}
}
