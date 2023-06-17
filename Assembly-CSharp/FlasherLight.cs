using System;
using UnityEngine;

// Token: 0x02000120 RID: 288
public class FlasherLight : IOEntity
{
	// Token: 0x04000EA1 RID: 3745
	public EmissionToggle toggler;

	// Token: 0x04000EA2 RID: 3746
	public Light myLight;

	// Token: 0x04000EA3 RID: 3747
	public float flashSpacing = 0.2f;

	// Token: 0x04000EA4 RID: 3748
	public float flashBurstSpacing = 0.5f;

	// Token: 0x04000EA5 RID: 3749
	public float flashOnTime = 0.1f;

	// Token: 0x04000EA6 RID: 3750
	public int numFlashesPerBurst = 5;

	// Token: 0x0600167C RID: 5756 RVA: 0x00025420 File Offset: 0x00023620
	public override void ResetState()
	{
		base.ResetState();
	}
}
