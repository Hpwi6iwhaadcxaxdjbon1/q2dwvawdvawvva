using System;
using UnityEngine;

// Token: 0x0200039A RID: 922
public class DiscoFloorColourLookups : PrefabAttribute, IClientComponent
{
	// Token: 0x0400197C RID: 6524
	public float[] InOutLookup;

	// Token: 0x0400197D RID: 6525
	public float[] RadialLookup;

	// Token: 0x0400197E RID: 6526
	public float[] RippleLookup;

	// Token: 0x0400197F RID: 6527
	public float[] CheckerLookup;

	// Token: 0x04001980 RID: 6528
	public float[] BlockLookup;

	// Token: 0x04001981 RID: 6529
	public Gradient[] ColourGradients;

	// Token: 0x06002077 RID: 8311 RVA: 0x000D6F1E File Offset: 0x000D511E
	protected override Type GetIndexedType()
	{
		return typeof(DiscoFloorColourLookups);
	}
}
