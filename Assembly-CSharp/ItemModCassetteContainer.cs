using System;

// Token: 0x020005D8 RID: 1496
public class ItemModCassetteContainer : ItemModContainer
{
	// Token: 0x040024A7 RID: 9383
	public ItemDefinition[] CassetteItems;

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x06002D16 RID: 11542 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool ForceAcceptItemCheck
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002D17 RID: 11543 RVA: 0x00110158 File Offset: 0x0010E358
	protected override void SetAllowedItems(ItemContainer container)
	{
		container.SetOnlyAllowedItems(this.CassetteItems);
	}
}
