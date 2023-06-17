using System;
using UnityEngine;

// Token: 0x020006ED RID: 1773
public class TerrainHeightSet : TerrainModifier
{
	// Token: 0x06003215 RID: 12821 RVA: 0x00134CAC File Offset: 0x00132EAC
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.HeightMap)
		{
			return;
		}
		TerrainMeta.HeightMap.SetHeight(position, opacity, radius, fade);
	}
}
