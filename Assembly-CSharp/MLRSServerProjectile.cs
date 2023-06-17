using System;

// Token: 0x02000489 RID: 1161
public class MLRSServerProjectile : ServerProjectile
{
	// Token: 0x17000320 RID: 800
	// (get) Token: 0x0600263B RID: 9787 RVA: 0x00007A3C File Offset: 0x00005C3C
	public override bool HasRangeLimit
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000321 RID: 801
	// (get) Token: 0x0600263C RID: 9788 RVA: 0x000F0FDF File Offset: 0x000EF1DF
	protected override int mask
	{
		get
		{
			return 1235430161;
		}
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x000F0FE6 File Offset: 0x000EF1E6
	protected override bool IsAValidHit(BaseEntity hitEnt)
	{
		return base.IsAValidHit(hitEnt) && (!hitEnt.IsValid() || !(hitEnt is MLRS));
	}
}
