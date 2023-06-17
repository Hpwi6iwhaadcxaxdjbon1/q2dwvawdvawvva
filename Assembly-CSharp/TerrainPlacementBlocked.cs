using System;
using UnityEngine;

// Token: 0x020006F0 RID: 1776
public class TerrainPlacementBlocked : TerrainModifier
{
	// Token: 0x0600321D RID: 12829 RVA: 0x00134D80 File Offset: 0x00132F80
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.PlacementMap)
		{
			return;
		}
		TerrainMeta.PlacementMap.SetBlocked(position, radius, fade);
	}
}
