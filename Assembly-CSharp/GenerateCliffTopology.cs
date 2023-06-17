using System;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
public class GenerateCliffTopology : ProceduralComponent
{
	// Token: 0x040027E4 RID: 10212
	public bool KeepExisting = true;

	// Token: 0x040027E5 RID: 10213
	private const int filter = 8389632;

	// Token: 0x06003164 RID: 12644 RVA: 0x001275F0 File Offset: 0x001257F0
	public static void Process(int x, int z)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float normZ = topologyMap.Coordinate(z);
		float normX = topologyMap.Coordinate(x);
		if ((topologyMap.GetTopology(x, z) & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			topologyMap.RemoveTopology(x, z, 2);
		}
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x00127660 File Offset: 0x00125860
	private static void Process(int x, int z, bool keepExisting)
	{
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		float normZ = topologyMap.Coordinate(z);
		float normX = topologyMap.Coordinate(x);
		int topology = topologyMap.GetTopology(x, z);
		if (!World.Procedural || (topology & 8389632) == 0)
		{
			float slope = TerrainMeta.HeightMap.GetSlope(normX, normZ);
			float splat = TerrainMeta.SplatMap.GetSplat(normX, normZ, 8);
			if (slope > 40f || splat > 0.4f)
			{
				topologyMap.AddTopology(x, z, 2);
				return;
			}
			if (!keepExisting)
			{
				topologyMap.RemoveTopology(x, z, 2);
			}
		}
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x001276E0 File Offset: 0x001258E0
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		Parallel.For(0, res, delegate(int z)
		{
			for (int i = 0; i < res; i++)
			{
				GenerateCliffTopology.Process(i, z, this.KeepExisting);
			}
		});
		ImageProcessing.Dilate2D(map, res, res, 4194306, 1, delegate(int x, int y)
		{
			if ((map[x * res + y] & 2) == 0)
			{
				map[x * res + y] |= 4194304;
			}
		});
	}
}
