using System;

// Token: 0x020006D0 RID: 1744
public class GenerateTerrainMesh : ProceduralComponent
{
	// Token: 0x060031C0 RID: 12736 RVA: 0x0013027F File Offset: 0x0012E47F
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("terrain", TerrainMeta.HeightMap.ToByteArray());
		}
		TerrainMeta.HeightMap.ApplyToTerrain();
	}

	// Token: 0x1700040F RID: 1039
	// (get) Token: 0x060031C1 RID: 12737 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
