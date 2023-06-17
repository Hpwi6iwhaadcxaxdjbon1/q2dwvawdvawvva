using System;
using UnityEngine;

// Token: 0x0200095B RID: 2395
public struct Half3
{
	// Token: 0x040033AE RID: 13230
	public ushort x;

	// Token: 0x040033AF RID: 13231
	public ushort y;

	// Token: 0x040033B0 RID: 13232
	public ushort z;

	// Token: 0x06003994 RID: 14740 RVA: 0x0015611C File Offset: 0x0015431C
	public Half3(Vector3 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x00156151 File Offset: 0x00154351
	public static explicit operator Vector3(Half3 vec)
	{
		return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
	}
}
