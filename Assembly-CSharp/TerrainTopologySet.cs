using System;
using UnityEngine;

// Token: 0x020006F3 RID: 1779
public class TerrainTopologySet : TerrainModifier
{
	// Token: 0x040028D8 RID: 10456
	[InspectorFlags]
	public TerrainTopology.Enum TopologyType = TerrainTopology.Enum.Decor;

	// Token: 0x06003223 RID: 12835 RVA: 0x00134DF7 File Offset: 0x00132FF7
	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		if (!TerrainMeta.TopologyMap)
		{
			return;
		}
		TerrainMeta.TopologyMap.SetTopology(position, (int)this.TopologyType, radius, fade);
	}
}
