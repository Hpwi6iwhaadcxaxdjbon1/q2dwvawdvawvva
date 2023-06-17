using System;
using UnityEngine;

// Token: 0x0200065A RID: 1626
public class DecorFlip : DecorComponent
{
	// Token: 0x040026B2 RID: 9906
	public DecorFlip.AxisType FlipAxis = DecorFlip.AxisType.Y;

	// Token: 0x06002F62 RID: 12130 RVA: 0x0011D7B0 File Offset: 0x0011B9B0
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		uint num = pos.Seed(World.Seed) + 4U;
		if (SeedRandom.Value(ref num) > 0.5f)
		{
			return;
		}
		switch (this.FlipAxis)
		{
		case DecorFlip.AxisType.X:
		case DecorFlip.AxisType.Z:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.up) * rot;
			return;
		case DecorFlip.AxisType.Y:
			rot = Quaternion.AngleAxis(180f, rot * Vector3.forward) * rot;
			return;
		default:
			return;
		}
	}

	// Token: 0x02000DB6 RID: 3510
	public enum AxisType
	{
		// Token: 0x040048BF RID: 18623
		X,
		// Token: 0x040048C0 RID: 18624
		Y,
		// Token: 0x040048C1 RID: 18625
		Z
	}
}
