using System;

// Token: 0x02000116 RID: 278
public class PressurePad : BaseDetector
{
	// Token: 0x04000E85 RID: 3717
	public float pressPowerTime = 0.5f;

	// Token: 0x04000E86 RID: 3718
	public int pressPowerAmount = 2;

	// Token: 0x04000E87 RID: 3719
	public const BaseEntity.Flags Flag_EmittingPower = BaseEntity.Flags.Reserved3;

	// Token: 0x06001647 RID: 5703 RVA: 0x0000441C File Offset: 0x0000261C
	public override int ConsumptionAmount()
	{
		return 1;
	}

	// Token: 0x06001648 RID: 5704 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x06001649 RID: 5705 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldTrigger()
	{
		return true;
	}

	// Token: 0x0600164A RID: 5706 RVA: 0x000AD509 File Offset: 0x000AB709
	public override void OnDetectorTriggered()
	{
		base.OnDetectorTriggered();
		base.Invoke(new Action(this.UnpowerTime), this.pressPowerTime);
		base.SetFlag(BaseEntity.Flags.Reserved3, true, false, true);
	}

	// Token: 0x0600164B RID: 5707 RVA: 0x000AD537 File Offset: 0x000AB737
	public override void OnDetectorReleased()
	{
		base.OnDetectorReleased();
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
	}

	// Token: 0x0600164C RID: 5708 RVA: 0x000885FE File Offset: 0x000867FE
	public void UnpowerTime()
	{
		base.SetFlag(BaseEntity.Flags.Reserved3, false, false, true);
		this.MarkDirty();
	}

	// Token: 0x0600164D RID: 5709 RVA: 0x000AD54D File Offset: 0x000AB74D
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			if (base.HasFlag(BaseEntity.Flags.Reserved3))
			{
				return this.pressPowerAmount;
			}
			if (this.IsPowered())
			{
				return base.GetPassthroughAmount(0);
			}
		}
		return 0;
	}
}
