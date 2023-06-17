using System;

// Token: 0x020003A0 RID: 928
public class MicrophoneStandIOEntity : IOEntity, IAudioConnectionSource
{
	// Token: 0x04001998 RID: 6552
	public int PowerCost = 5;

	// Token: 0x04001999 RID: 6553
	public TriggerBase InstrumentTrigger;

	// Token: 0x0400199A RID: 6554
	public bool IsStatic;

	// Token: 0x06002084 RID: 8324 RVA: 0x000D701C File Offset: 0x000D521C
	public override int DesiredPower()
	{
		return this.PowerCost;
	}

	// Token: 0x06002085 RID: 8325 RVA: 0x000D7024 File Offset: 0x000D5224
	public override int MaximalPowerOutput()
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.MaximalPowerOutput();
	}

	// Token: 0x06002086 RID: 8326 RVA: 0x000D7037 File Offset: 0x000D5237
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.CalculateCurrentEnergy(inputAmount, inputSlot);
	}

	// Token: 0x06002087 RID: 8327 RVA: 0x000D704C File Offset: 0x000D524C
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (this.IsStatic)
		{
			return 100;
		}
		return base.GetPassthroughAmount(outputSlot);
	}

	// Token: 0x06002088 RID: 8328 RVA: 0x000D7060 File Offset: 0x000D5260
	public override bool IsRootEntity()
	{
		return this.IsStatic || base.IsRootEntity();
	}

	// Token: 0x06002089 RID: 8329 RVA: 0x000037E7 File Offset: 0x000019E7
	public IOEntity ToEntity()
	{
		return this;
	}
}
