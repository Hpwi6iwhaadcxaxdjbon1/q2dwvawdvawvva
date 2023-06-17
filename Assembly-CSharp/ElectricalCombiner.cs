using System;
using UnityEngine;

// Token: 0x0200011A RID: 282
public class ElectricalCombiner : IOEntity
{
	// Token: 0x04000E8E RID: 3726
	public int input1Amount;

	// Token: 0x04000E8F RID: 3727
	public int input2Amount;

	// Token: 0x04000E90 RID: 3728
	public int input3Amount;

	// Token: 0x0600165E RID: 5726 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x170001F3 RID: 499
	// (get) Token: 0x0600165F RID: 5727 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool BlockFluidDraining
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x000AD840 File Offset: 0x000ABA40
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = this.input1Amount + this.input2Amount + this.input3Amount;
		Mathf.Clamp(num - 1, 0, num);
		return num;
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x000AD86E File Offset: 0x000ABA6E
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x000AD894 File Offset: 0x000ABA94
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (inputAmount > 0 && base.IsConnectedTo(this, slot, IOEntity.backtracking * 2, true))
		{
			inputAmount = 0;
			base.SetFlag(BaseEntity.Flags.Reserved7, true, false, true);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved7, false, false, true);
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		else if (slot == 2)
		{
			this.input3Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount + this.input3Amount;
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0 || this.input3Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(num, slot);
	}
}
