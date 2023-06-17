using System;

// Token: 0x02000119 RID: 281
public class ElectricalBlocker : IOEntity
{
	// Token: 0x04000E8C RID: 3724
	protected int input1Amount;

	// Token: 0x04000E8D RID: 3725
	protected int input2Amount;

	// Token: 0x06001657 RID: 5719 RVA: 0x000AD77B File Offset: 0x000AB97B
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	// Token: 0x06001658 RID: 5720 RVA: 0x00050870 File Offset: 0x0004EA70
	public override bool WantsPower()
	{
		return !base.IsOn();
	}

	// Token: 0x06001659 RID: 5721 RVA: 0x000AD791 File Offset: 0x000AB991
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x0600165A RID: 5722 RVA: 0x00057F25 File Offset: 0x00056125
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x000AD7C0 File Offset: 0x000AB9C0
	public virtual void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, base.IsOn(), false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x000AD81A File Offset: 0x000ABA1A
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.input1Amount = inputAmount;
			this.UpdateBlocked();
			return;
		}
		if (inputSlot == 0)
		{
			this.input2Amount = inputAmount;
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}
}
