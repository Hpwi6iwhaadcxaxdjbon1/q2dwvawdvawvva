using System;
using UnityEngine;

// Token: 0x020006F2 RID: 1778
public class TerrainTopologyAdd : TerrainModifier
{
	// Token: 0x040028D7 RID: 10455
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;

	// Token: 0x06003221 RID: 12833 RVA: 0x00134DC1 File Offset: 0x00132FC1
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.AddTopology(position, (int)this.TopologyType, radius, fade);
	}
}
