using System;
using UnityEngine;

// Token: 0x02000189 RID: 393
public class ItemModWrap : ItemMod
{
	// Token: 0x0400109F RID: 4255
	public GameObjectRef successEffect;

	// Token: 0x040010A0 RID: 4256
	public ItemDefinition wrappedDefinition;

	// Token: 0x040010A1 RID: 4257
	public static Translate.Phrase wrap_gift = new Translate.Phrase("wrap_gift", "Wrap Gift");

	// Token: 0x040010A2 RID: 4258
	public static Translate.Phrase wrap_gift_desc = new Translate.Phrase("wrap_gift_desc", "Wrap this item and turn it in to an openable gift");

	// Token: 0x060017C9 RID: 6089 RVA: 0x000B35E4 File Offset: 0x000B17E4
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "wrap")
		{
			if (item.amount <= 0)
			{
				return;
			}
			Item slot = item.contents.GetSlot(0);
			if (slot == null)
			{
				return;
			}
			int position = item.position;
			ItemContainer rootContainer = item.GetRootContainer();
			item.RemoveFromContainer();
			Item item2 = ItemManager.Create(this.wrappedDefinition, 1, 0UL);
			slot.MoveToContainer(item2.contents, -1, true, false, null, true);
			item2.MoveToContainer(rootContainer, position, true, false, null, true);
			item.Remove(0f);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}
}
