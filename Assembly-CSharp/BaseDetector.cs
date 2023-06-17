using System;

// Token: 0x02000113 RID: 275
public class BaseDetector : IOEntity
{
	// Token: 0x04000E82 RID: 3714
	public PlayerDetectionTrigger myTrigger;

	// Token: 0x04000E83 RID: 3715
	public const BaseEntity.Flags Flag_HasContents = BaseEntity.Flags.Reserved1;

	// Token: 0x06001639 RID: 5689 RVA: 0x000AD39C File Offset: 0x000AB59C
	public override int ConsumptionAmount()
	{
		return base.ConsumptionAmount();
	}

	// Token: 0x0600163A RID: 5690 RVA: 0x000AD3A4 File Offset: 0x000AB5A4
	public virtual bool ShouldTrigger()
	{
		return this.IsPowered();
	}

	// Token: 0x0600163B RID: 5691 RVA: 0x000AD3AC File Offset: 0x000AB5AC
	public virtual void OnObjects()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		if (this.ShouldTrigger())
		{
			this.OnDetectorTriggered();
			this.MarkDirty();
		}
	}

	// Token: 0x0600163C RID: 5692 RVA: 0x000AD3D0 File Offset: 0x000AB5D0
	public virtual void OnEmpty()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
		if (this.ShouldTrigger())
		{
			this.OnDetectorReleased();
			this.MarkDirty();
		}
	}

	// Token: 0x0600163D RID: 5693 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDetectorTriggered()
	{
	}

	// Token: 0x0600163E RID: 5694 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnDetectorReleased()
	{
	}

	// Token: 0x0600163F RID: 5695 RVA: 0x000AD3F4 File Offset: 0x000AB5F4
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!base.HasFlag(BaseEntity.Flags.Reserved1))
		{
			return 0;
		}
		return base.GetPassthroughAmount(0);
	}
}
