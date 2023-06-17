using System;
using UnityEngine;

// Token: 0x020006F1 RID: 1777
public class TerrainSplatSet : TerrainModifier
{
	// Token: 0x040028D6 RID: 10454
	public TerrainSplat.Enum SplatType;

	// Token: 0x0600321F RID: 12831 RVA: 0x00134D9D File Offset: 0x00132F9D
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.SplatMap)
		{
			return;
		}
		TerrainMeta.SplatMap.SetSplat(position, (int)this.SplatType, opacity, radius, fade);
	}
}
