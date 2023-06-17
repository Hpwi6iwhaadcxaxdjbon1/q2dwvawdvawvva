using System;
using UnityEngine;

// Token: 0x02000684 RID: 1668
[Serializable]
public class PowerLineWireConnection
{
	// Token: 0x04002736 RID: 10038
	public Vector3 inOffset = Vector3.zero;

	// Token: 0x04002737 RID: 10039
	public Vector3 outOffset = Vector3.zero;

	// Token: 0x04002738 RID: 10040
	public float radius = 0.01f;

	// Token: 0x04002739 RID: 10041
	public Transform start;

	// Token: 0x0400273A RID: 10042
	public Transform end;
}
