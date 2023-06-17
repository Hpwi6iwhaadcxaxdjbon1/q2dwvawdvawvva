using System;
using UnityEngine;

// Token: 0x0200011C RID: 284
public class RANDSwitch : ElectricalBlocker
{
	// Token: 0x04000E94 RID: 3732
	private bool rand;

	// Token: 0x0600166B RID: 5739 RVA: 0x000ADB68 File Offset: 0x000ABD68
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return base.GetPassthroughAmount(outputSlot) * (base.IsOn() ? 0 : 1);
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x000ADB80 File Offset: 0x000ABD80
	public override void UpdateBlocked()
	{
		bool flag = base.IsOn();
		base.SetFlag(BaseEntity.Flags.On, this.rand, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved8, this.rand, false, false);
		this.UpdateHasPower(this.input1Amount + this.input2Amount, 1);
		if (flag != base.IsOn())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x0600166D RID: 5741 RVA: 0x000ADBD7 File Offset: 0x000ABDD7
	public bool RandomRoll()
	{
		return UnityEngine.Random.Range(0, 2) == 1;
	}

	// Token: 0x0600166E RID: 5742 RVA: 0x000ADBE4 File Offset: 0x000ABDE4
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && inputAmount > 0)
		{
			this.input1Amount = inputAmount;
			this.rand = this.RandomRoll();
			this.UpdateBlocked();
		}
		if (inputSlot == 2)
		{
			if (inputAmount > 0)
			{
				this.rand = false;
				this.UpdateBlocked();
				return;
			}
		}
		else
		{
			base.UpdateFromInput(inputAmount, inputSlot);
		}
	}
}
