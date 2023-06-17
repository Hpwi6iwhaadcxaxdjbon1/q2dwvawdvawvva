using System;
using UnityEngine;

// Token: 0x0200072C RID: 1836
public struct MaterialPropertyDesc
{
	// Token: 0x040029B6 RID: 10678
	public int nameID;

	// Token: 0x040029B7 RID: 10679
	public Type type;

	// Token: 0x0600334F RID: 13135 RVA: 0x0013AF01 File Offset: 0x00139101
	public MaterialPropertyDesc(string name, Type type)
	{
		this.nameID = Shader.PropertyToID(name);
		this.type = type;
	}
}
