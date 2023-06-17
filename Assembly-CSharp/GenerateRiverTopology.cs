using System;
using System.Linq;

// Token: 0x020006C8 RID: 1736
public class GenerateRiverTopology : ProceduralComponent
{
	// Token: 0x060031AB RID: 12715 RVA: 0x0012E8F8 File Offset: 0x0012CAF8
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTopology();
		}
		this.MarkRiverside();
	}

	// Token: 0x060031AC RID: 12716 RVA: 0x0012E958 File Offset: 0x0012CB58
	public void MarkRiverside()
	{
		TerrainHeightMap heightmap = TerrainMeta.HeightMap;
		TerrainTopologyMap topomap = TerrainMeta.TopologyMap;
		int[] map = topomap.dst;
		int res = topomap.res;
		ImageProcessing.Dilate2D(map, res, res, 49152, 6, delegate(int x, int y)
		{
			if ((map[x * res + y] & 49) != 0)
			{
				map[x * res + y] |= 32768;
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
