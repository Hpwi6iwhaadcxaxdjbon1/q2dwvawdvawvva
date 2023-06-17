using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x0200011F RID: 287
public class Igniter : IOEntity
{
	// Token: 0x04000E9B RID: 3739
	public float IgniteRange = 5f;

	// Token: 0x04000E9C RID: 3740
	public float IgniteFrequency = 1f;

	// Token: 0x04000E9D RID: 3741
	public float IgniteStartDelay;

	// Token: 0x04000E9E RID: 3742
	public Transform LineOfSightEyes;

	// Token: 0x04000E9F RID: 3743
	public float SelfDamagePerIgnite = 0.5f;

	// Token: 0x04000EA0 RID: 3744
	public int PowerConsumption = 2;

	// Token: 0x06001678 RID: 5752 RVA: 0x000ADD6A File Offset: 0x000ABF6A
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x000ADD74 File Offset: 0x000ABF74
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0)
		{
			base.InvokeRepeating(new Action(this.IgniteInRange), this.IgniteStartDelay, this.IgniteFrequency);
			return;
		}
		if (base.IsInvoking(new Action(this.IgniteInRange)))
		{
			base.CancelInvoke(new Action(this.IgniteInRange));
		}
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000ADDD4 File Offset: 0x000ABFD4
	private void IgniteInRange()
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(this.LineOfSightEyes.position, this.IgniteRange, list, 1236495121, QueryTriggerInteraction.Collide);
		int num = 0;
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.HasFlag(BaseEntity.Flags.On) && baseEntity.IsVisible(this.LineOfSightEyes.position, float.PositiveInfinity))
			{
				IIgniteable igniteable;
				if (baseEntity.isServer && baseEntity is BaseOven)
				{
					(baseEntity as BaseOven).StartCooking();
					if (baseEntity.HasFlag(BaseEntity.Flags.On))
					{
						num++;
					}
				}
				else if (baseEntity.isServer && (igniteable = (baseEntity as IIgniteable)) != null && igniteable.CanIgnite())
				{
					igniteable.Ignite(base.transform.position);
					num++;
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		base.Hurt(this.SelfDamagePerIgnite, DamageType.ElectricShock, this, false);
	}
}
