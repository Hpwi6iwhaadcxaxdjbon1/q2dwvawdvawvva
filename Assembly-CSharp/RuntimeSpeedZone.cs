using System;

// Token: 0x020001AF RID: 431
public class RuntimeSpeedZone : IAIPathSpeedZone
{
	// Token: 0x0400116B RID: 4459
	public OBB worldOBBBounds;

	// Token: 0x0400116C RID: 4460
	public float maxVelocityPerSec = 5f;

	// Token: 0x060018D0 RID: 6352 RVA: 0x000B7CB1 File Offset: 0x000B5EB1
	public float GetMaxSpeed()
	{
		return this.maxVelocityPerSec;
	}

	// Token: 0x060018D1 RID: 6353 RVA: 0x000B7CB9 File Offset: 0x000B5EB9
	public OBB WorldSpaceBounds()
	{
		return this.worldOBBBounds;
	}
}
