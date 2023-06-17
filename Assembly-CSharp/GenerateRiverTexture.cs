using System;
using System.Linq;

// Token: 0x020006C7 RID: 1735
public class GenerateRiverTexture : ProceduralComponent
{
	// Token: 0x060031A9 RID: 12713 RVA: 0x0012E8A0 File Offset: 0x0012CAA0
	public override void Process(uint seed)
	{
		foreach (PathList pathList in TerrainMeta.Path.Rivers.AsEnumerable<PathList>().Reverse<PathList>())
		{
			pathList.AdjustTerrainTexture();
		}
	}
}
