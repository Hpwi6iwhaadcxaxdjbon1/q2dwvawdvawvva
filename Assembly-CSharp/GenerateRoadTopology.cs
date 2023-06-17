using System;
using System.Linq;

// Token: 0x020006CE RID: 1742
public class GenerateRoadTopology : ProceduralComponent
{
	// Token: 0x060031BA RID: 12730 RVA: 0x00130108 File Offset: 0x0012E308
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRoadside();
		TerrainMeta.PlacementMap.Reset();
	}

	// Token: 0x060031BB RID: 12731 RVA: 0x00130170 File Offset: 0x0012E370
	private void MarkRoadside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 6144, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 4096;
			}
			float normX = topomap.Coordinate(x);
			float normZ = topomap.Coordinate(y);
			if (heightmap.GetSlope(normX, normZ) > 40f)
			{
				map[x * res + y] |= 2;
			}
		});
	}
}
