using System;

// Token: 0x020006D3 RID: 1747
public class GenerateWireMeshes : ProceduralComponent
{
	// Token: 0x060031C9 RID: 12745 RVA: 0x001303CB File Offset: 0x0012E5CB
	public override void Process(uint seed)
	{
		TerrainMeta.Path.CreateWires();
	}

	// Token: 0x17000411 RID: 1041
	// (get) Token: 0x060031CA RID: 12746 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool RunOnCache
	{
		get
		{
			return true;
		}
	}
}
