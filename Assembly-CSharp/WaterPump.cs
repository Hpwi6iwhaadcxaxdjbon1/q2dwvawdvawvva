using System;
using UnityEngine;

// Token: 0x020003DE RID: 990
public class WaterPump : LiquidContainer
{
	// Token: 0x04001A51 RID: 6737
	public Transform WaterResourceLocation;

	// Token: 0x04001A52 RID: 6738
	public float PumpInterval = 20f;

	// Token: 0x04001A53 RID: 6739
	public int AmountPerPump = 30;

	// Token: 0x04001A54 RID: 6740
	public int PowerConsumption = 5;

	// Token: 0x060021FE RID: 8702 RVA: 0x000DC761 File Offset: 0x000DA961
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x000DC76C File Offset: 0x000DA96C
	private void CreateWater()
	{
		if (this.IsFull())
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(this.WaterResourceLocation.position);
		if (atPoint != null)
		{
			base.inventory.AddItem(atPoint, this.AmountPerPump, 0UL, ItemContainer.LimitStack.Existing);
			base.UpdateOnFlag();
		}
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x000DC7B8 File Offset: 0x000DA9B8
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(BaseEntity.Flags.Reserved8);
		if (base.isServer && old.HasFlag(BaseEntity.Flags.Reserved8) != flag)
		{
			if (flag)
			{
				if (!base.IsInvoking(new Action(this.CreateWater)))
				{
					base.InvokeRandomized(new Action(this.CreateWater), this.PumpInterval, this.PumpInterval, this.PumpInterval * 0.1f);
					return;
				}
			}
			else if (base.IsInvoking(new Action(this.CreateWater)))
			{
				base.CancelInvoke(new Action(this.CreateWater));
			}
		}
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x00072400 File Offset: 0x00070600
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Clamp(base.GetLiquidCount(), 0, this.maxOutputFlow);
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x000DC4FC File Offset: 0x000DA6FC
	private bool IsFull()
	{
		return base.inventory.itemList.Count != 0 && base.inventory.itemList[0].amount >= base.inventory.maxStackSize;
	}

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06002203 RID: 8707 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}
}
