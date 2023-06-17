using System;

// Token: 0x020005D7 RID: 1495
public class ItemModBurnable : ItemMod
{
	// Token: 0x040024A3 RID: 9379
	public float fuelAmount = 10f;

	// Token: 0x040024A4 RID: 9380
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition byproductItem;

	// Token: 0x040024A5 RID: 9381
	public int byproductAmount = 1;

	// Token: 0x040024A6 RID: 9382
	public float byproductChance = 0.5f;

	// Token: 0x06002D14 RID: 11540 RVA: 0x00110125 File Offset: 0x0010E325
	public override void OnItemCreated(Item item)
	{
		item.fuel = this.fuelAmount;
	}
}
