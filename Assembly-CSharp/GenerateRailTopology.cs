using System;
using System.Linq;

// Token: 0x020006C3 RID: 1731
public class GenerateRailTopology : ProceduralComponent
{
	// Token: 0x0600319F RID: 12703 RVA: 0x0012DF68 File Offset: 0x0012C168
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRailside();
		TerrainMeta.PlacementMap.Reset();
	}

	// Token: 0x060031A0 RID: 12704 RVA: 0x0012DFD0 File Offset: 0x0012C1D0
	private void MarkRailside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 1572864, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 1048576;
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
