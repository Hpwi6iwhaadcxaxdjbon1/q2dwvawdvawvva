using System;
using UnityEngine;

// Token: 0x020006B3 RID: 1715
public class GenerateCliffSplat : ProceduralComponent
{
	// Token: 0x040027E3 RID: 10211
	private const int filter = 8389632;

	// Token: 0x06003161 RID: 12641 RVA: 0x00127548 File Offset: 0x00125748
	public static void Process(int x, int z)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		float normZ = splatMap.Coordinate(z);
		float normX = splatMap.Coordinate(x);
		if ((TerrainMeta.TopologyMap.GetTopology(normX, normZ) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			if (slope > 30f)
			{
				splatMap.SetSplat(x, z, 8, Mathf.InverseLerp(30f, 50f, slope));
			}
		}
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x001275B0 File Offset: 0x001257B0
	public override void Process(uint seed)
	{
		TerrainSplatMap splatMap = TerrainMeta.SplatMap;
		int splatres = splatMap.res;
		Parallel.For(0, splatres, delegate(int z)
		{
			for (int i = 0; i < splatres; i++)
			{
				GenerateCliffSplat.Process(i, z);
			}
		});
	}
}
