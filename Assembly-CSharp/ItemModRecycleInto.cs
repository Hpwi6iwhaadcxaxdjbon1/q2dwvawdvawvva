using System;
using UnityEngine;

// Token: 0x020005F7 RID: 1527
public class ItemModRecycleInto : ItemMod
{
	// Token: 0x04002513 RID: 9491
	public ItemDefinition recycleIntoItem;

	// Token: 0x04002514 RID: 9492
	public int numRecycledItemMin = 1;

	// Token: 0x04002515 RID: 9493
	public int numRecycledItemMax = 1;

	// Token: 0x04002516 RID: 9494
	public GameObjectRef successEffect;

	// Token: 0x06002D77 RID: 11639 RVA: 0x00111944 File Offset: 0x0010FB44
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "recycle_item")
		{
			int num = UnityEngine.Random.Range(this.numRecycledItemMin, this.numRecycledItemMax + 1);
			item.UseItem(1);
			if (num > 0)
			{
				Item item2 = ItemManager.Create(this.recycleIntoItem, num, 0UL);
				if (!item2.MoveToContainer(player.inventory.containerMain, -1, true, false, null, true))
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
}
