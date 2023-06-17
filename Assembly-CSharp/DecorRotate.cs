using System;
using UnityEngine;

// Token: 0x0200065C RID: 1628
public class DecorRotate : DecorComponent
{
	// Token: 0x040026B5 RID: 9909
	public Vector3 MinRotation = new Vector3(0f, -180f, 0f);

	// Token: 0x040026B6 RID: 9910
	public Vector3 MaxRotation = new Vector3(0f, 180f, 0f);

	// Token: 0x06002F66 RID: 12134 RVA: 0x0011D948 File Offset: 0x0011BB48
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 2U;
		float x = SeedRandom.Range(ref num, this.MinRotation.x, this.MaxRotation.x);
		float y = SeedRandom.Range(ref num, this.MinRotation.y, this.MaxRotation.y);
		float z = SeedRandom.Range(ref num, this.MinRotation.z, this.MaxRotation.z);
		rot = Quaternion.Euler(x, y, z) * rot;
	}
}
