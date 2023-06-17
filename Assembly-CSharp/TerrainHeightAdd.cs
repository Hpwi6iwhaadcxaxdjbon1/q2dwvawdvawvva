using System;
using UnityEngine;

// Token: 0x020006EC RID: 1772
public class TerrainHeightAdd : TerrainModifier
{
	// Token: 0x040028D2 RID: 10450
	public float Delta = 1f;

	// Token: 0x06003213 RID: 12819 RVA: 0x00134C69 File Offset: 0x00132E69
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.AddHeight(position, opacity * this.Delta * TerrainMeta.OneOverSize.y, radius, fade);
	}
}
