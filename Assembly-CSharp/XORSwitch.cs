using System;
using UnityEngine;

// Token: 0x020004D0 RID: 1232
public class XORSwitch : IOEntity
{
	// Token: 0x04002043 RID: 8259
	private int input1Amount;

	// Token: 0x04002044 RID: 8260
	private int input2Amount;

	// Token: 0x04002045 RID: 8261
	private bool firstRun = true;

	// Token: 0x06002811 RID: 10257 RVA: 0x000F86D1 File Offset: 0x000F68D1
	public override void ResetState()
	{
		base.ResetState();
		this.firstRun = true;
	}

	// Token: 0x06002812 RID: 10258 RVA: 0x000F86E0 File Offset: 0x000F68E0
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.input1Amount > 0 && this.input2Amount > 0)
		{
			return 0;
		}
		int num = Mathf.Max(this.input1Amount, this.input2Amount);
		return Mathf.Max(0, num - this.ConsumptionAmount());
	}

	// Token: 0x06002813 RID: 10259 RVA: 0x000F8721 File Offset: 0x000F6921
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, this.input1Amount > 0 || this.input2Amount > 0, false, false);
	}

	// Token: 0x06002814 RID: 10260 RVA: 0x00057F25 File Offset: 0x00056125
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06002815 RID: 10261 RVA: 0x000F8748 File Offset: 0x000F6948
	public override void UpdateFromInput(int inputAmount, int slot)
	{
		if (inputAmount > 0 && base.IsConnectedTo(this, slot, IOEntity.backtracking, false))
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
		if (this.firstRun)
		{
			if (!base.IsInvoking(new Action(this.UpdateFlags)))
			{
				base.Invoke(new Action(this.UpdateFlags), 0.1f);
			}
		}
		else
		{
			this.UpdateFlags();
		}
		this.firstRun = false;
		base.UpdateFromInput(inputAmount, slot);
	}

	// Token: 0x06002816 RID: 10262 RVA: 0x000F87EC File Offset: 0x000F69EC
	private void UpdateFlags()
	{
		int num = (this.input1Amount > 0 && this.input2Amount > 0) ? 0 : Mathf.Max(this.input1Amount, this.input2Amount);
		bool b = num > 0;
		base.SetFlag(BaseEntity.Flags.Reserved1, this.input1Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved3, b, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved4, this.input1Amount > 0 || this.input2Amount > 0, false, false);
		base.SetFlag(BaseEntity.Flags.On, num > 0, false, true);
	}
}
