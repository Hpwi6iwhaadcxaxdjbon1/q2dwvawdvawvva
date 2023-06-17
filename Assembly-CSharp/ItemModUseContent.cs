using System;

// Token: 0x02000600 RID: 1536
public class ItemModUseContent : ItemMod
{
	// Token: 0x04002534 RID: 9524
	public int amountToConsume = 1;

	// Token: 0x06002D8A RID: 11658 RVA: 0x001121F6 File Offset: 0x001103F6
	public override void DoAction(Item item, BasePlayer player)
	{
		if (item.contents == null)
		{
			return;
		}
		if (item.contents.itemList.Count == 0)
		{
			return;
		}
		item.contents.itemList[0].UseItem(this.amountToConsume);
	}
}
