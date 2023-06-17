using System;
using UnityEngine;

// Token: 0x02000188 RID: 392
public class ItemModOpenWrapped : ItemMod
{
	// Token: 0x0400109C RID: 4252
	public GameObjectRef successEffect;

	// Token: 0x0400109D RID: 4253
	public static Translate.Phrase open_wrapped_gift = new Translate.Phrase("open_wrapped_gift", "Unwrap");

	// Token: 0x0400109E RID: 4254
	public static Translate.Phrase open_wrapped_gift_desc = new Translate.Phrase("open_wrapped_gift_desc", "Unwrap the gift and reveal its contents");

	// Token: 0x060017C6 RID: 6086 RVA: 0x000B351C File Offset: 0x000B171C
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "open")
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
			slot.MoveToContainer(rootContainer, position, true, false, null, true);
			item.Remove(0f);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}
}
