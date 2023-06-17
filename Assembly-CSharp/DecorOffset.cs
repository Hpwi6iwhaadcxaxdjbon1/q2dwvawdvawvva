using System;
using UnityEngine;

// Token: 0x0200065B RID: 1627
public class DecorOffset : DecorComponent
{
	// Token: 0x040026B3 RID: 9907
	public Vector3 MinOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x040026B4 RID: 9908
	public Vector3 MaxOffset = new Vector3(0f, 0f, 0f);

	// Token: 0x06002F64 RID: 12132 RVA: 0x0011D860 File Offset: 0x0011BA60
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 1U;
		pos.x += scale.x * SeedRandom.Range(ref num, this.MinOffset.x, this.MaxOffset.x);
		pos.y += scale.y * SeedRandom.Range(ref num, this.MinOffset.y, this.MaxOffset.y);
		pos.z += scale.z * SeedRandom.Range(ref num, this.MinOffset.z, this.MaxOffset.z);
	}
}
