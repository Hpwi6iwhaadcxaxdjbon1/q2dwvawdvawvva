using System;
using UnityEngine;

// Token: 0x02000959 RID: 2393
public struct FixedSByteNorm3
{
	// Token: 0x040033A1 RID: 13217
	private const int FracBits = 7;

	// Token: 0x040033A2 RID: 13218
	private const float MaxFrac = 128f;

	// Token: 0x040033A3 RID: 13219
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x040033A4 RID: 13220
	public sbyte x;

	// Token: 0x040033A5 RID: 13221
	public sbyte y;

	// Token: 0x040033A6 RID: 13222
	public sbyte z;

	// Token: 0x06003990 RID: 14736 RVA: 0x0015601F File Offset: 0x0015421F
	public FixedSByteNorm3(Vector3 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
	}

	// Token: 0x06003991 RID: 14737 RVA: 0x0015605A File Offset: 0x0015425A
	public static explicit operator Vector3(FixedSByteNorm3 vec)
	{
		return new Vector3((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f);
	}
}
