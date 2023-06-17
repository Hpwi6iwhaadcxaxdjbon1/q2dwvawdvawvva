using System;
using UnityEngine;

// Token: 0x02000694 RID: 1684
public class TerrainColors : TerrainExtension
{
	// Token: 0x04002777 RID: 10103
	private TerrainSplatMap splatMap;

	// Token: 0x04002778 RID: 10104
	private TerrainBiomeMap biomeMap;

	// Token: 0x06002FE5 RID: 12261 RVA: 0x0011FC4C File Offset: 0x0011DE4C
	public override void Setup()
	{
		this.splatMap = this.terrain.GetComponent<TerrainSplatMap>();
		this.biomeMap = this.terrain.GetComponent<TerrainBiomeMap>();
	}

	// Token: 0x06002FE6 RID: 12262 RVA: 0x0011FC70 File Offset: 0x0011DE70
	public Color GetColor(Vector3 worldPos, int mask = -1)
	{
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return this.GetColor(normX, normZ, mask);
	}

	// Token: 0x06002FE7 RID: 12263 RVA: 0x0011FCA0 File Offset: 0x0011DEA0
	public Color GetColor(float normX, float normZ, int mask = -1)
	{
		float biome = this.biomeMap.GetBiome(normX, normZ, 1);
		float biome2 = this.biomeMap.GetBiome(normX, normZ, 2);
		float biome3 = this.biomeMap.GetBiome(normX, normZ, 4);
		float biome4 = this.biomeMap.GetBiome(normX, normZ, 8);
		int num = TerrainSplat.TypeToIndex(this.splatMap.GetSplatMaxType(normX, normZ, mask));
		TerrainConfig.SplatType splatType = this.config.Splats[num];
		return biome * splatType.AridColor + biome2 * splatType.TemperateColor + biome3 * splatType.TundraColor + biome4 * splatType.ArcticColor;
	}
}
