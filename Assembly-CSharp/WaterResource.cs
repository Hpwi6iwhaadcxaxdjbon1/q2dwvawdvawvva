using System;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class WaterResource
{
	// Token: 0x06002BD6 RID: 11222 RVA: 0x00109655 File Offset: 0x00107855
	public static ItemDefinition GetAtPoint(Vector3 pos)
	{
		return ItemManager.FindItemDefinition(WaterResource.IsFreshWater(pos) ? "water" : "water.salt");
	}

	// Token: 0x06002BD7 RID: 11223 RVA: 0x00109670 File Offset: 0x00107870
	public static bool IsFreshWater(Vector3 pos)
	{
		return !(TerrainMeta.TopologyMap == null) && TerrainMeta.TopologyMap.GetTopology(pos, 245760);
	}

	// Token: 0x06002BD8 RID: 11224 RVA: 0x00109694 File Offset: 0x00107894
	public static ItemDefinition Merge(ItemDefinition first, ItemDefinition second)
	{
		if (first == second)
		{
			return first;
		}
		if (first.shortname == "water.salt" || second.shortname == "water.salt")
		{
			return ItemManager.FindItemDefinition("water.salt");
		}
		return ItemManager.FindItemDefinition("water");
	}
}
