using System;
using UnityEngine;

// Token: 0x020004CC RID: 1228
public class ORSwitch : IOEntity
{
	// Token: 0x04002041 RID: 8257
	private int input1Amount;

	// Token: 0x04002042 RID: 8258
	private int input2Amount;

	// Token: 0x06002803 RID: 10243 RVA: 0x00007641 File Offset: 0x00005841
	public override bool WantsPassthroughPower()
	{
		return base.IsOn();
	}

	// Token: 0x06002804 RID: 10244 RVA: 0x000F85A0 File Offset: 0x000F67A0
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = Mathf.Max(this.input1Amount, this.input2Amount);
		return Mathf.Max(0, num - this.ConsumptionAmount());
	}

	// Token: 0x06002805 RID: 10245 RVA: 0x000F85CD File Offset: 0x000F67CD
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06002806 RID: 10246 RVA: 0x00057F25 File Offset: 0x00056125
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06002807 RID: 10247 RVA: 0x000F85F4 File Offset: 0x000F67F4
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (base.IsConnectedTo(this, slot, IOEntity.backtracking, false))
		{
			inputAmount = 0;
		}
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = this.input1Amount + this.input2Amount;
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}
}
