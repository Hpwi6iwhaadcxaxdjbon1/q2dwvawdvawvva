using System;

// Token: 0x020006B5 RID: 1717
public class GenerateClutterTopology : ProceduralComponent
{
	// Token: 0x06003168 RID: 12648 RVA: 0x0012776C File Offset: 0x0012596C
	public override void Process(uint seed)
	{
		int[] map = TerrainMeta.TopologyMap.dst;
		int res = TerrainMeta.TopologyMap.res;
		ImageProcessing.Dilate2D(map, res, res, 16777728, 3, delegate(int x, int y)
		{
			if ((map[x * res + y] & 512) == 0)
			{
				map[x * res + y] |= 16777216;
			}
		});
	}
}
