using System;

// Token: 0x020006E2 RID: 1762
public class ResetPlacementMap : ProceduralComponent
{
	// Token: 0x060031F3 RID: 12787 RVA: 0x001340BC File Offset: 0x001322BC
	public override void Process(uint seed)
	{
		TerrainMeta.PlacementMap.Reset();
	}
}
