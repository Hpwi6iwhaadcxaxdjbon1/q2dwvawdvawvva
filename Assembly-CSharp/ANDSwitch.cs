using System;
using UnityEngine;

// Token: 0x020004C2 RID: 1218
public class ANDSwitch : IOEntity
{
	// Token: 0x0400202E RID: 8238
	private int input1Amount;

	// Token: 0x0400202F RID: 8239
	private int input2Amount;

	// Token: 0x060027B8 RID: 10168 RVA: 0x000F78A1 File Offset: 0x000F5AA1
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount <= 0 || this.input2Amount <= 0)
		{
			return 0;
		}
		return Mathf.Max(this.input1Amount, this.input2Amount);
	}

	// Token: 0x060027B9 RID: 10169 RVA: 0x000F78C8 File Offset: 0x000F5AC8
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x060027BA RID: 10170 RVA: 0x000F78EC File Offset: 0x000F5AEC
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (slot == 0)
		{
			this.input1Amount = inputAmount;
		}
		else if (slot == 1)
		{
			this.input2Amount = inputAmount;
		}
		int num = (this.input1Amount > 0 && this.input2Amount > 0) ? (this.input1Amount + this.input2Amount) : 0;
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 && this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
		base.UpdateFromInput(inputAmount, slot);
	}
}
