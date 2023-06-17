using System;
using UnityEngine;

// Token: 0x020005FF RID: 1535
public class ItemModUpgrade : ItemMod
{
	// Token: 0x0400252D RID: 9517
	public int numForUpgrade = 10;

	// Token: 0x0400252E RID: 9518
	public float upgradeSuccessChance = 1f;

	// Token: 0x0400252F RID: 9519
	public int numToLoseOnFail = 2;

	// Token: 0x04002530 RID: 9520
	public ItemDefinition upgradedItem;

	// Token: 0x04002531 RID: 9521
	public int numUpgradedItem = 1;

	// Token: 0x04002532 RID: 9522
	public GameObjectRef successEffect;

	// Token: 0x04002533 RID: 9523
	public GameObjectRef failEffect;

	// Token: 0x06002D88 RID: 11656 RVA: 0x001120C0 File Offset: 0x001102C0
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "upgrade_item")
		{
			if (item.amount < this.numForUpgrade)
			{
				return;
			}
			if (UnityEngine.Random.Range(0f, 1f) <= this.upgradeSuccessChance)
			{
				item.UseItem(this.numForUpgrade);
				Item item2 = ItemManager.Create(this.upgradedItem, this.numUpgradedItem, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
				{
					item2.Drop(player.GetDropPosition(), player.GetDropVelocity(), default(Quaternion));
				}
				if (this.successEffect.isValid)
				{
					Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
					return;
				}
			}
			else
			{
				item.UseItem(this.numToLoseOnFail);
				if (this.failEffect.isValid)
				{
					Effect.server.Run(this.failEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
				}
			}
		}
	}
}
