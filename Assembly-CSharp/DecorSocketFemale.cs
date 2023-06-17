using System;
using UnityEngine;

// Token: 0x0200065E RID: 1630
public class DecorSocketFemale : PrefabAttribute
{
	// Token: 0x06002F6A RID: 12138 RVA: 0x0011DAEE File Offset: 0x0011BCEE
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketFemale);
	}

	// Token: 0x06002F6B RID: 12139 RVA: 0x0011DAFA File Offset: 0x0011BCFA
	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.5f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
