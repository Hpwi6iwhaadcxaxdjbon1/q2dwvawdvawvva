using System;
using UnityEngine;

// Token: 0x02000958 RID: 2392
public struct FixedShort3
{
	// Token: 0x0400339B RID: 13211
	private const int FracBits = 10;

	// Token: 0x0400339C RID: 13212
	private const float MaxFrac = 1024f;

	// Token: 0x0400339D RID: 13213
	private const float RcpMaxFrac = 0.0009765625f;

	// Token: 0x0400339E RID: 13214
	public short x;

	// Token: 0x0400339F RID: 13215
	public short y;

	// Token: 0x040033A0 RID: 13216
	public short z;

	// Token: 0x0600398E RID: 14734 RVA: 0x00155FB6 File Offset: 0x001541B6
	public FixedShort3(Vector3 vec)
	{
		this.x = (short)(vec.x * 1024f);
		this.y = (short)(vec.y * 1024f);
		this.z = (short)(vec.z * 1024f);
	}

	// Token: 0x0600398F RID: 14735 RVA: 0x00155FF1 File Offset: 0x001541F1
	public static explicit operator Vector3(FixedShort3 vec)
	{
		return new Vector3((float)vec.x * 0.0009765625f, (float)vec.y * 0.0009765625f, (float)vec.z * 0.0009765625f);
	}
}
