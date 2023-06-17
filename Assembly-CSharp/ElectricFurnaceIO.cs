using System;

// Token: 0x020004C7 RID: 1223
public class ElectricFurnaceIO : IOEntity, IIndustrialStorage
{
	// Token: 0x0400203A RID: 8250
	public int PowerConsumption = 3;

	// Token: 0x060027DD RID: 10205 RVA: 0x000F811A File Offset: 0x000F631A
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x060027DE RID: 10206 RVA: 0x000F8122 File Offset: 0x000F6322
	public override int DesiredPower()
	{
		if (base.GetParentEntity() == null)
		{
			return 0;
		}
		if (!base.GetParentEntity().IsOn())
		{
			return 0;
		}
		return this.PowerConsumption;
	}

	// Token: 0x060027DF RID: 10207 RVA: 0x000F814C File Offset: 0x000F634C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			ElectricOven parentOven = this.GetParentOven();
			if (parentOven != null)
			{
				parentOven.OnIOEntityFlagsChanged(old, next);
			}
		}
	}

	// Token: 0x060027E0 RID: 10208 RVA: 0x000F8184 File Offset: 0x000F6384
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
		if (inputSlot == 1 && inputAmount > 0)
		{
			ElectricOven parentOven = this.GetParentOven();
			if (parentOven != null)
			{
				parentOven.StartCooking();
			}
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			ElectricOven parentOven2 = this.GetParentOven();
			if (parentOven2 != null)
			{
				parentOven2.StopCooking();
			}
		}
	}

	// Token: 0x060027E1 RID: 10209 RVA: 0x000F81D8 File Offset: 0x000F63D8
	private ElectricOven GetParentOven()
	{
		return base.GetParentEntity() as ElectricOven;
	}

	// Token: 0x17000365 RID: 869
	// (get) Token: 0x060027E2 RID: 10210 RVA: 0x000F81E5 File Offset: 0x000F63E5
	public ItemContainer Container
	{
		get
		{
			return this.GetParentOven().inventory;
		}
	}

	// Token: 0x060027E3 RID: 10211 RVA: 0x000F81F2 File Offset: 0x000F63F2
	public Vector2i InputSlotRange(int slotIndex)
	{
		return new Vector2i(1, 2);
	}

	// Token: 0x060027E4 RID: 10212 RVA: 0x000F81FB File Offset: 0x000F63FB
	public Vector2i OutputSlotRange(int slotIndex)
	{
		return new Vector2i(3, 5);
	}

	// Token: 0x060027E5 RID: 10213 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x060027E6 RID: 10214 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x17000366 RID: 870
	// (get) Token: 0x060027E7 RID: 10215 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}
}
