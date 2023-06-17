using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000585 RID: 1413
public class TriggerHurt : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x0400230F RID: 8975
	public float DamagePerSecond = 1f;

	// Token: 0x04002310 RID: 8976
	public float DamageTickRate = 4f;

	// Token: 0x04002311 RID: 8977
	public DamageType damageType;

	// Token: 0x06002B42 RID: 11074 RVA: 0x00106B24 File Offset: 0x00104D24
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
		return baseEntity.gameObject;
	}

	// Token: 0x06002B43 RID: 11075 RVA: 0x00106B67 File Offset: 0x00104D67
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x00106B8C File Offset: 0x00104D8C
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06002B45 RID: 11077 RVA: 0x00106BA0 File Offset: 0x00104DA0
	private void OnTick()
	{
		BaseEntity attacker = base.gameObject.ToBaseEntity();
		if (this.entityContents == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents.ToArray<BaseEntity>())
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!(baseCombatEntity == null) && this.CanHurt(baseCombatEntity))
				{
					baseCombatEntity.Hurt(this.DamagePerSecond * (1f / this.DamageTickRate), this.damageType, attacker, true);
				}
			}
		}
	}

	// Token: 0x06002B46 RID: 11078 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanHurt(BaseCombatEntity ent)
	{
		return true;
	}
}
