using System;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class DecorScale : DecorComponent
{
	// Token: 0x040026B7 RID: 9911
	public Vector3 MinScale = new Vector3(1f, 1f, 1f);

	// Token: 0x040026B8 RID: 9912
	public Vector3 MaxScale = new Vector3(2f, 2f, 2f);

	// Token: 0x06002F68 RID: 12136 RVA: 0x0011DA18 File Offset: 0x0011BC18
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 3U;
		float t = SeedRandom.Value(ref num);
		scale.x *= Mathf.Lerp(this.MinScale.x, this.MaxScale.x, t);
		scale.y *= Mathf.Lerp(this.MinScale.y, this.MaxScale.y, t);
		scale.z *= Mathf.Lerp(this.MinScale.z, this.MaxScale.z, t);
	}
}
