using System;
using UnityEngine;

// Token: 0x020006E4 RID: 1764
public class AddToAlphaMap : ProceduralObject
{
	// Token: 0x040028BE RID: 10430
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	// Token: 0x060031F8 RID: 12792 RVA: 0x00134100 File Offset: 0x00132300
	public override void Process()
	{
		OBB obb = new OBB(base.transform, this.bounds);
		Vector3 point = obb.GetPoint(-1f, 0f, -1f);
		Vector3 point2 = obb.GetPoint(1f, 0f, -1f);
		Vector3 point3 = obb.GetPoint(-1f, 0f, 1f);
		Vector3 point4 = obb.GetPoint(1f, 0f, 1f);
		TerrainMeta.AlphaMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
		{
			TerrainMeta.AlphaMap.SetAlpha(x, z, 0f);
		});
		GameManager.Destroy(this, 0f);
	}
}
