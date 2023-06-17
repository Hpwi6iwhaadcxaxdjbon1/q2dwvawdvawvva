using System;
using UnityEngine;

// Token: 0x02000709 RID: 1801
public class WaterCheck : PrefabAttribute
{
	// Token: 0x04002951 RID: 10577
	public bool Rotate = true;

	// Token: 0x060032EB RID: 13035 RVA: 0x001399E1 File Offset: 0x00137BE1
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0f, 0f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}

	// Token: 0x060032EC RID: 13036 RVA: 0x00139A16 File Offset: 0x00137C16
	public bool Check(Vector3 pos)
	{
		return pos.y <= TerrainMeta.WaterMap.GetHeight(pos);
	}

	// Token: 0x060032ED RID: 13037 RVA: 0x00139A2E File Offset: 0x00137C2E
	protected override Type GetIndexedType()
	{
		return typeof(WaterCheck);
	}
}
