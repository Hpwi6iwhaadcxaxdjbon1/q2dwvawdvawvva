using System;

// Token: 0x020005D5 RID: 1493
public class ItemModBaitContainer : ItemModContainer
{
	// Token: 0x170003BD RID: 957
	// (get) Token: 0x06002D0E RID: 11534 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool ForceAcceptItemCheck
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x0010FF84 File Offset: 0x0010E184
	protected override bool CanAcceptItem(Item item, int count)
	{
		ItemModCompostable component = item.info.GetComponent<ItemModCompostable>();
		return component != null && component.BaitValue > 0f;
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x0010FFB5 File Offset: 0x0010E1B5
	protected override void SetAllowedItems(ItemContainer container)
	{
		FishLookup.LoadFish();
		container.SetOnlyAllowedItems(FishLookup.BaitItems);
	}
}
