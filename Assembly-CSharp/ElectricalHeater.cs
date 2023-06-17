using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
public class ElectricalHeater : IOEntity
{
	// Token: 0x04000E88 RID: 3720
	public float fadeDuration = 1f;

	// Token: 0x04000E89 RID: 3721
	public Light sourceLight;

	// Token: 0x04000E8A RID: 3722
	public GrowableHeatSource growableHeatSource;

	// Token: 0x0600164F RID: 5711 RVA: 0x000037BE File Offset: 0x000019BE
	public override int ConsumptionAmount()
	{
		return 3;
	}

	// Token: 0x06001650 RID: 5712 RVA: 0x00025420 File Offset: 0x00023620
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06001651 RID: 5713 RVA: 0x000AD59C File Offset: 0x000AB79C
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(BaseEntity.Flags.Reserved8);
		if (old.HasFlag(BaseEntity.Flags.Reserved8) != flag && this.growableHeatSource != null)
		{
			this.growableHeatSource.ForceUpdateGrowablesInRange();
		}
	}

	// Token: 0x06001652 RID: 5714 RVA: 0x000AD5F8 File Offset: 0x000AB7F8
	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		if (this.growableHeatSource != null)
		{
			this.growableHeatSource.ForceUpdateGrowablesInRange();
		}
	}
}
