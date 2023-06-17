using System;
using UnityEngine;

// Token: 0x020005F9 RID: 1529
public class ItemModReveal : ItemMod
{
	// Token: 0x0400251A RID: 9498
	public int numForReveal = 10;

	// Token: 0x0400251B RID: 9499
	public ItemDefinition revealedItemOverride;

	// Token: 0x0400251C RID: 9500
	public int revealedItemAmount = 1;

	// Token: 0x0400251D RID: 9501
	public LootSpawn revealList;

	// Token: 0x0400251E RID: 9502
	public GameObjectRef successEffect;

	// Token: 0x06002D7C RID: 11644 RVA: 0x00111AC4 File Offset: 0x0010FCC4
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "reveal")
		{
			if (item.amount < this.numForReveal)
			{
				return;
			}
			int position = item.position;
			item.UseItem(this.numForReveal);
			Item item2 = null;
			if (this.revealedItemOverride)
			{
				item2 = ItemManager.Create(this.revealedItemOverride, this.revealedItemAmount, 0UL);
			}
			if (item2 != null && !item2.MoveToContainer(player.inventory.containerMain, (item.amount == 0) ? position : -1, true, false, null, true))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}
}
