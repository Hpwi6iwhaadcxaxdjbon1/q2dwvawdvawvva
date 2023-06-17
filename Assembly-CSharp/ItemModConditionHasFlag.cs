using System;

// Token: 0x020005DE RID: 1502
public class ItemModConditionHasFlag : ItemMod
{
	// Token: 0x040024B2 RID: 9394
	public Item.Flag flag;

	// Token: 0x040024B3 RID: 9395
	public bool requiredState;

	// Token: 0x06002D22 RID: 11554 RVA: 0x001102E9 File Offset: 0x0010E4E9
	public override bool Passes(Item item)
	{
		return item.HasFlag(this.flag) == this.requiredState;
	}
}
