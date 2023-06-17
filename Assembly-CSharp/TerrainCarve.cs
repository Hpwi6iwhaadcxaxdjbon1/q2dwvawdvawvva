using System;
using UnityEngine;

// Token: 0x020006EB RID: 1771
public class TerrainCarve : TerrainModifier
{
	// Token: 0x06003211 RID: 12817 RVA: 0x00134C3E File Offset: 0x00132E3E
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.AlphaMap)
		{
			return;
		}
		TerrainMeta.AlphaMap.SetAlpha(position, 0f, opacity, radius, fade);
	}
}
