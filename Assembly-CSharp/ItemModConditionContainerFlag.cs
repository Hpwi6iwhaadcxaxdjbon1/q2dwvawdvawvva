using System;

// Token: 0x020005DB RID: 1499
public class ItemModConditionContainerFlag : ItemMod
{
	// Token: 0x040024AB RID: 9387
	public ItemContainer.Flag flag;

	// Token: 0x040024AC RID: 9388
	public bool requiredState;

	// Token: 0x06002D1B RID: 11547 RVA: 0x00110184 File Offset: 0x0010E384
	public override bool Passes(Item item)
	{
		if (item.parent == null)
		{
			return !this.requiredState;
		}
		if (!item.parent.HasFlag(this.flag))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}
}
