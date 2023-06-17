using System;
using System.Linq;

// Token: 0x020006CD RID: 1741
public class GenerateRoadTexture : ProceduralComponent
{
	// Token: 0x060031B8 RID: 12728 RVA: 0x001300B0 File Offset: 0x0012E2B0
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Roads.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
