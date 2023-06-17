using System;
using UnityEngine;

// Token: 0x02000931 RID: 2353
public class FixedRateStepped
{
	// Token: 0x0400333F RID: 13119
	public float rate = 0.1f;

	// Token: 0x04003340 RID: 13120
	public int maxSteps = 3;

	// Token: 0x04003341 RID: 13121
	internal float nextCall;

	// Token: 0x06003883 RID: 14467 RVA: 0x001512C4 File Offset: 0x0014F4C4
	public bool ShouldStep()
	{
		if (this.nextCall > Time.time)
		{
			return false;
		}
		if (this.nextCall == 0f)
		{
			this.nextCall = Time.time;
		}
		if (this.nextCall + this.rate * (float)this.maxSteps < Time.time)
		{
			this.nextCall = Time.time - this.rate * (float)this.maxSteps;
		}
		this.nextCall += this.rate;
		return true;
	}
}
