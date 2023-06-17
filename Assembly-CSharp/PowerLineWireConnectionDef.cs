using System;
using UnityEngine;

// Token: 0x02000683 RID: 1667
[Serializable]
public class PowerLineWireConnectionDef
{
	// Token: 0x04002732 RID: 10034
	public Vector3 inOffset = Vector3.zero;

	// Token: 0x04002733 RID: 10035
	public Vector3 outOffset = Vector3.zero;

	// Token: 0x04002734 RID: 10036
	public float radius = 0.01f;

	// Token: 0x04002735 RID: 10037
	public bool hidden;

	// Token: 0x06002FBE RID: 12222 RVA: 0x0011F03A File Offset: 0x0011D23A
	public PowerLineWireConnectionDef()
	{
	}

	// Token: 0x06002FBF RID: 12223 RVA: 0x0011F064 File Offset: 0x0011D264
	public PowerLineWireConnectionDef(PowerLineWireConnectionDef src)
	{
		this.inOffset = src.inOffset;
		this.outOffset = src.outOffset;
		this.radius = src.radius;
	}
}
