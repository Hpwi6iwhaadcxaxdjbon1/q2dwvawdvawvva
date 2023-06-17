using System;
using System.Linq;

// Token: 0x020006C2 RID: 1730
public class GenerateRailTexture : ProceduralComponent
{
	// Token: 0x0600319D RID: 12701 RVA: 0x0012DF10 File Offset: 0x0012C110
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rails.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
