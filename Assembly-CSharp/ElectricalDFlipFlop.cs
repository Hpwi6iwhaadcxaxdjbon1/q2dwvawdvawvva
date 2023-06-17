using System;
using UnityEngine;

// Token: 0x0200011B RID: 283
public class ElectricalDFlipFlop : IOEntity
{
	// Token: 0x04000E91 RID: 3729
	[NonSerialized]
	private int setAmount;

	// Token: 0x04000E92 RID: 3730
	[NonSerialized]
	private int resetAmount;

	// Token: 0x04000E93 RID: 3731
	[NonSerialized]
	private int toggleAmount;

	// Token: 0x06001664 RID: 5732 RVA: 0x00087805 File Offset: 0x00085A05
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
	}

	// Token: 0x06001665 RID: 5733 RVA: 0x000AD990 File Offset: 0x000ABB90
	public bool GetDesiredState()
	{
		if (this.setAmount > 0 && this.resetAmount == 0)
		{
			return true;
		}
		if (this.setAmount > 0 && this.resetAmount > 0)
		{
			return true;
		}
		if (this.setAmount == 0 && this.resetAmount > 0)
		{
			return false;
		}
		if (this.toggleAmount > 0)
		{
			return !base.IsOn();
		}
		return this.setAmount == 0 && this.resetAmount == 0 && base.IsOn();
	}

	// Token: 0x06001666 RID: 5734 RVA: 0x000ADA04 File Offset: 0x000ABC04
	public void UpdateState()
	{
		if (this.IsPowered())
		{
			bool flag = base.IsOn();
			bool desiredState = this.GetDesiredState();
			base.SetFlag(BaseEntity.Flags.On, desiredState, false, true);
			if (flag != base.IsOn())
			{
				base.MarkDirtyForceUpdateOutputs();
			}
		}
	}

	// Token: 0x06001667 RID: 5735 RVA: 0x000ADA40 File Offset: 0x000ABC40
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1)
		{
			this.setAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 2)
		{
			this.resetAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 3)
		{
			this.toggleAmount = inputAmount;
			this.UpdateState();
			return;
		}
		if (inputSlot == 0)
		{
			base.UpdateFromInput(inputAmount, inputSlot);
			this.UpdateState();
		}
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x000ADA94 File Offset: 0x000ABC94
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x000ADAA0 File Offset: 0x000ABCA0
	public override void UpdateOutputs()
	{
		if (!base.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			int num = Mathf.Max(0, this.currentEnergy - 1);
			if (this.outputs[0].connectedTo.Get(true) != null)
			{
				this.outputs[0].connectedTo.Get(true).UpdateFromInput(base.IsOn() ? num : 0, this.outputs[0].connectedToSlot);
			}
			if (this.outputs[1].connectedTo.Get(true) != null)
			{
				this.outputs[1].connectedTo.Get(true).UpdateFromInput(base.IsOn() ? 0 : num, this.outputs[1].connectedToSlot);
			}
		}
	}
}
