using System;
using UnityEngine;

// Token: 0x02000652 RID: 1618
[Serializable]
public class SpawnFilter
{
	// Token: 0x0400267A RID: 9850
	[InspectorFlags]
	public TerrainSplat.Enum SplatType = (TerrainSplat.Enum)(-1);

	// Token: 0x0400267B RID: 9851
	[InspectorFlags]
	public TerrainBiome.Enum BiomeType = (TerrainBiome.Enum)(-1);

	// Token: 0x0400267C RID: 9852
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAny = (TerrainTopology.Enum)(-1);

	// Token: 0x0400267D RID: 9853
	[InspectorFlags]
	public TerrainTopology.Enum TopologyAll;

	// Token: 0x0400267E RID: 9854
	[InspectorFlags]
	public TerrainTopology.Enum TopologyNot;

	// Token: 0x06002EE9 RID: 12009 RVA: 0x0011B777 File Offset: 0x00119977
	public bool Test(Vector3 worldPos)
	{
		return this.GetFactor(worldPos, true) > 0.5f;
	}

	// Token: 0x06002EEA RID: 12010 RVA: 0x0011B788 File Offset: 0x00119988
	public bool Test(float normX, float normZ)
	{
		return this.GetFactor(normX, normZ, true) > 0.5f;
	}

	// Token: 0x06002EEB RID: 12011 RVA: 0x0011B79C File Offset: 0x0011999C
	public float GetFactor(Vector3 worldPos, bool checkPlacementMap = true)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetFactor(normX, normZ, checkPlacementMap);
	}

	// Token: 0x06002EEC RID: 12012 RVA: 0x0011B7CC File Offset: 0x001199CC
	public float GetFactor(float normX, float normZ, bool checkPlacementMap = true)
	{
		if (TerrainMeta.TopologyMap == null)
		{
			return 0f;
		}
		if (checkPlacementMap && TerrainMeta.PlacementMap != null && TerrainMeta.PlacementMap.GetBlocked(normX, normZ))
		{
			return 0f;
		}
		int splatType = (int)this.SplatType;
		int biomeType = (int)this.BiomeType;
		int topologyAny = (int)this.TopologyAny;
		int topologyAll = (int)this.TopologyAll;
		int topologyNot = (int)this.TopologyNot;
		if (topologyAny == 0)
		{
			Debug.LogError("Empty topology filter is invalid.");
		}
		else if (topologyAny != -1 || topologyAll != 0 || topologyNot != 0)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
			if (topologyAny != -1 && (topology & topologyAny) == 0)
			{
				return 0f;
			}
			if (topologyNot != 0 && (topology & topologyNot) != 0)
			{
				return 0f;
			}
			if (topologyAll != 0 && (topology & topologyAll) != topologyAll)
			{
				return 0f;
			}
		}
		if (biomeType == 0)
		{
			Debug.LogError("Empty biome filter is invalid.");
		}
		else if (biomeType != -1 && (TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ, -1) & biomeType) == 0)
		{
			return 0f;
		}
		if (splatType == 0)
		{
			Debug.LogError("Empty splat filter is invalid.");
		}
		else if (splatType != -1)
		{
			return TerrainMeta.SplatMap.GetSplat(normX, normZ, splatType);
		}
		return 1f;
	}
}
