using System;
using UnityEngine;

// Token: 0x020004CA RID: 1226
public class FluidSwitch : ElectricSwitch
{
	// Token: 0x0400203D RID: 8253
	private BaseEntity.Flags Flag_PumpPowered = BaseEntity.Flags.Reserved6;

	// Token: 0x0400203E RID: 8254
	public Animator PumpAnimator;

	// Token: 0x0400203F RID: 8255
	private bool pumpEnabled;

	// Token: 0x04002040 RID: 8256
	private int lastToggleInput;

	// Token: 0x060027F7 RID: 10231 RVA: 0x00025420 File Offset: 0x00023620
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x060027F8 RID: 10232 RVA: 0x000F849C File Offset: 0x000F669C
	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		if (inputSlot == 1 && this.lastToggleInput != inputAmount)
		{
			this.lastToggleInput = inputAmount;
			this.SetSwitch(inputAmount > 0);
		}
		if (inputSlot == 2)
		{
			bool flag = this.pumpEnabled;
			this.pumpEnabled = (inputAmount > 0);
			if (flag != this.pumpEnabled)
			{
				this.lastPassthroughEnergy = -1;
				base.SetFlag(this.Flag_PumpPowered, this.pumpEnabled, false, true);
				this.SendChangedToRoot(true);
			}
		}
	}

	// Token: 0x060027F9 RID: 10233 RVA: 0x000F8505 File Offset: 0x000F6705
	public override void SetSwitch(bool wantsOn)
	{
		base.SetSwitch(wantsOn);
		base.Invoke(new Action(this.DelayedSendChanged), IOEntity.responsetime * 2f);
	}

	// Token: 0x060027FA RID: 10234 RVA: 0x000F852B File Offset: 0x000F672B
	private void DelayedSendChanged()
	{
		this.SendChangedToRoot(true);
	}

	// Token: 0x060027FB RID: 10235 RVA: 0x000F8534 File Offset: 0x000F6734
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		if (!base.IsOn())
		{
			return 0;
		}
		return this.GetCurrentEnergy();
	}

	// Token: 0x060027FC RID: 10236 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x060027FD RID: 10237 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x060027FE RID: 10238 RVA: 0x000F854B File Offset: 0x000F674B
	protected override bool DisregardGravityRestrictionsOnLiquid
	{
		get
		{
			return base.HasFlag(this.Flag_PumpPowered);
		}
	}

	// Token: 0x060027FF RID: 10239 RVA: 0x000F8559 File Offset: 0x000F6759
	public override bool AllowLiquidPassthrough(IOEntity fromSource, Vector3 sourceWorldPosition, bool forPlacement = false)
	{
		return (forPlacement || base.IsOn()) && base.AllowLiquidPassthrough(fromSource, sourceWorldPosition, false);
	}
}
