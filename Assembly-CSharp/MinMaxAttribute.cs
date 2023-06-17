using System;
using UnityEngine;

// Token: 0x020008F0 RID: 2288
public class MinMaxAttribute : PropertyAttribute
{
	// Token: 0x040032B1 RID: 12977
	public float min;

	// Token: 0x040032B2 RID: 12978
	public float max;

	// Token: 0x060037BD RID: 14269 RVA: 0x0014DFA5 File Offset: 0x0014C1A5
	public MinMaxAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}
}
