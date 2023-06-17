using System;
using UnityEngine;

// Token: 0x020005D6 RID: 1494
public class ItemModBlueprintCraft : ItemMod
{
	// Token: 0x040024A2 RID: 9378
	public GameObjectRef successEffect;

	// Token: 0x06002D12 RID: 11538 RVA: 0x0010FFD0 File Offset: 0x0010E1D0
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (item.GetOwnerPlayer() != player)
		{
			return;
		}
		if (command == "craft")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, 1, false))
			{
				return;
			}
			Item fromTempBlueprint = item;
			if (item.amount > 1)
			{
				fromTempBlueprint = item.SplitItem(1);
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, 1, 0, fromTempBlueprint, false);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
		if (command == "craft_all")
		{
			if (!item.IsBlueprint())
			{
				return;
			}
			if (!player.inventory.crafting.CanCraft(item.blueprintTargetDef.Blueprint, item.amount, false))
			{
				return;
			}
			player.inventory.crafting.CraftItem(item.blueprintTargetDef.Blueprint, player, null, item.amount, 0, item, false);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}
}
