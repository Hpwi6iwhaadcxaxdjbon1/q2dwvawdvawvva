using System;
using UnityEngine;

// Token: 0x0200065F RID: 1631
public class DecorSocketMale : PrefabAttribute
{
	// Token: 0x06002F6D RID: 12141 RVA: 0x0011DB2F File Offset: 0x0011BD2F
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketMale);
	}

	// Token: 0x06002F6E RID: 12142 RVA: 0x0011DB3B File Offset: 0x0011BD3B
	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 1f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
