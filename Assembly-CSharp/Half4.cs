using System;
using UnityEngine;

// Token: 0x0200095C RID: 2396
public struct Half4
{
	// Token: 0x040033B1 RID: 13233
	public ushort x;

	// Token: 0x040033B2 RID: 13234
	public ushort y;

	// Token: 0x040033B3 RID: 13235
	public ushort z;

	// Token: 0x040033B4 RID: 13236
	public ushort w;

	// Token: 0x06003996 RID: 14742 RVA: 0x0015617C File Offset: 0x0015437C
	public Half4(Vector4 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
		this.w = Mathf.FloatToHalf(vec.w);
	}

	// Token: 0x06003997 RID: 14743 RVA: 0x001561CD File Offset: 0x001543CD
	public static explicit operator Vector4(Half4 vec)
	{
		return new Vector4(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z), Mathf.HalfToFloat(vec.w));
	}
}
