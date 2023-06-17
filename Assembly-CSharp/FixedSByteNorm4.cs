using System;
using UnityEngine;

// Token: 0x0200095A RID: 2394
public struct FixedSByteNorm4
{
	// Token: 0x040033A7 RID: 13223
	private const int FracBits = 7;

	// Token: 0x040033A8 RID: 13224
	private const float MaxFrac = 128f;

	// Token: 0x040033A9 RID: 13225
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x040033AA RID: 13226
	public sbyte x;

	// Token: 0x040033AB RID: 13227
	public sbyte y;

	// Token: 0x040033AC RID: 13228
	public sbyte z;

	// Token: 0x040033AD RID: 13229
	public sbyte w;

	// Token: 0x06003992 RID: 14738 RVA: 0x00156088 File Offset: 0x00154288
	public FixedSByteNorm4(Vector4 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
		this.w = (sbyte)(vec.w * 128f);
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x001560E1 File Offset: 0x001542E1
	public static explicit operator Vector4(FixedSByteNorm4 vec)
	{
		return new Vector4((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f, (float)vec.w * 0.0078125f);
	}
}
