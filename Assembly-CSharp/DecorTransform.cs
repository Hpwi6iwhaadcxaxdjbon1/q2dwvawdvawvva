using System;
using UnityEngine;

// Token: 0x02000661 RID: 1633
public class DecorTransform : DecorComponent
{
	// Token: 0x040026B9 RID: 9913
	public Vector3 Position = new Vector3(0f, 0f, 0f);

	// Token: 0x040026BA RID: 9914
	public Vector3 Rotation = new Vector3(0f, 0f, 0f);

	// Token: 0x040026BB RID: 9915
	public Vector3 Scale = new Vector3(1f, 1f, 1f);

	// Token: 0x06002F72 RID: 12146 RVA: 0x0011DB90 File Offset: 0x0011BD90
	public override void Apply(ref Vector3 pos, ref Quaternion rot, ref Vector3 scale)
	{
		pos += rot * Vector3.Scale(scale, this.Position);
		rot = Quaternion.Euler(this.Rotation) * rot;
		scale = Vector3.Scale(scale, this.Scale);
	}
}
