using System;
using UnityEngine;

// Token: 0x0200068C RID: 1676
public class TerrainCheck : PrefabAttribute
{
	// Token: 0x04002750 RID: 10064
	public bool Rotate = true;

	// Token: 0x04002751 RID: 10065
	public float Extents = 1f;

	// Token: 0x06002FCD RID: 12237 RVA: 0x0011F558 File Offset: 0x0011D758
	public bool Check(Vector3 pos)
	{
		float extents = this.Extents;
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float num = pos.y - extents;
		float num2 = pos.y + extents;
		return num <= height && num2 >= height;
	}

	// Token: 0x06002FCE RID: 12238 RVA: 0x0011F595 File Offset: 0x0011D795
	protected override Type GetIndexedType()
	{
		return typeof(TerrainCheck);
	}
}
