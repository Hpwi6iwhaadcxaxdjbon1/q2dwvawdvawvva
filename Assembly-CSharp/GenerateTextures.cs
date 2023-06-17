using System;

// Token: 0x020006D1 RID: 1745
public class GenerateTextures : ProceduralComponent
{
	// Token: 0x060031C3 RID: 12739 RVA: 0x001302A8 File Offset: 0x0012E4A8
	public override void Process(uint seed)
	{
		if (!World.Cached)
		{
			World.AddMap("height", TerrainMeta.HeightMap.ToByteArray());
			World.AddMap("splat", TerrainMeta.SplatMap.ToByteArray());
			World.AddMap("biome", TerrainMeta.BiomeMap.ToByteArray());
			World.AddMap("topology", TerrainMeta.TopologyMap.ToByteArray());
			World.AddMap("alpha", TerrainMeta.AlphaMap.ToByteArray());
			World.AddMap("water", TerrainMeta.WaterMap.ToByteArray());
			return;
		}
		TerrainMeta.HeightMap.FromByteArray(World.GetMap("height"));
	}

	// Token: 0x17000410 RID: 1040
	// (get) Token: 0x060031C4 RID: 12740 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
