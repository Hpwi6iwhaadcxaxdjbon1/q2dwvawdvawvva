using System;

// Token: 0x02000124 RID: 292
public class CableTunnel : IOEntity
{
	// Token: 0x04000EBD RID: 3773
	private const int numChannels = 4;

	// Token: 0x04000EBE RID: 3774
	private int[] inputAmounts = new int[4];

	// Token: 0x06001690 RID: 5776 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool WantsPower()
	{
		return true;
	}

	// Token: 0x06001691 RID: 5777 RVA: 0x000AE564 File Offset: 0x000AC764
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		int num = this.inputAmounts[inputSlot];
		this.inputAmounts[inputSlot] = inputAmount;
		if (inputAmount != num)
		{
			this.ensureOutputsUpdated = true;
		}
		base.IOStateChanged(inputAmount, inputSlot);
	}

	// Token: 0x06001692 RID: 5778 RVA: 0x000AE598 File Offset: 0x000AC798
	public override void UpdateOutputs()
	{
		if (!base.ShouldUpdateOutputs())
		{
			return;
		}
		if (this.ensureOutputsUpdated)
		{
			for (int i = 0; i < 4; i++)
			{
				IOEntity.IOSlot ioslot = this.outputs[i];
				if (ioslot.connectedTo.Get(true) != null)
				{
					ioslot.connectedTo.Get(true).UpdateFromInput(this.inputAmounts[i], ioslot.connectedToSlot);
				}
			}
		}
	}
}
