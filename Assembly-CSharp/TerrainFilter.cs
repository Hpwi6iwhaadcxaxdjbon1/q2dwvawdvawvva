using System;
using UnityEngine;

// Token: 0x020006AD RID: 1709
public class TerrainFilter : PrefabAttribute
{
	// Token: 0x040027DB RID: 10203
	public SpawnFilter Filter;

	// Token: 0x040027DC RID: 10204
	public bool CheckPlacementMap = true;

	// Token: 0x0600314C RID: 12620 RVA: 0x00126CB8 File Offset: 0x00124EB8
	protected void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(base.transform.position + Vector3.up * 50f * 0.5f, new Vector3(0.5f, 50f, 0.5f));
		Gizmos.DrawSphere(base.transform.position + Vector3.up * 50f, 2f);
	}

	// Token: 0x0600314D RID: 12621 RVA: 0x00126D4E File Offset: 0x00124F4E
	public bool Check(Vector3 pos)
	{
		return this.Filter.GetFactor(pos, this.CheckPlacementMap) > 0f;
	}

	// Token: 0x0600314E RID: 12622 RVA: 0x00126D69 File Offset: 0x00124F69
	protected override Type GetIndexedType()
	{
		return typeof(TerrainFilter);
	}
}
