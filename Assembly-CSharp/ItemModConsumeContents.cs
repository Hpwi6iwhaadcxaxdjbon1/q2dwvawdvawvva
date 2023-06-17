using System;

// Token: 0x020005E4 RID: 1508
public class ItemModConsumeContents : ItemMod
{
	// Token: 0x040024C2 RID: 9410
	public GameObjectRef consumeEffect;

	// Token: 0x06002D33 RID: 11571 RVA: 0x0011075C File Offset: 0x0010E95C
	public override void DoAction(Item item, BasePlayer player)
	{
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				component.DoAction(item2, player);
				break;
			}
		}
	}

	// Token: 0x06002D34 RID: 11572 RVA: 0x001107D8 File Offset: 0x0010E9D8
	public override bool CanDoAction(Item item, BasePlayer player)
	{
		if (!player.metabolism.CanConsume())
		{
			return false;
		}
		if (item.contents == null)
		{
			return false;
		}
		foreach (Item item2 in item.contents.itemList)
		{
			ItemModConsume component = item2.info.GetComponent<ItemModConsume>();
			if (!(component == null) && component.CanDoAction(item2, player))
			{
				return true;
			}
		}
		return false;
	}
}
