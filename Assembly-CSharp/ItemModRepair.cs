using System;
using UnityEngine;

// Token: 0x020005F8 RID: 1528
public class ItemModRepair : ItemMod
{
	// Token: 0x04002517 RID: 9495
	public float conditionLost = 0.05f;

	// Token: 0x04002518 RID: 9496
	public GameObjectRef successEffect;

	// Token: 0x04002519 RID: 9497
	public int workbenchLvlRequired;

	// Token: 0x06002D79 RID: 11641 RVA: 0x00111A0C File Offset: 0x0010FC0C
	public bool HasCraftLevel(BasePlayer player = null)
	{
		return player != null && player.isServer && player.currentCraftLevel >= (float)this.workbenchLvlRequired;
	}

	// Token: 0x06002D7A RID: 11642 RVA: 0x00111A34 File Offset: 0x0010FC34
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "refill")
		{
			if (player.IsSwimming())
			{
				return;
			}
			if (!this.HasCraftLevel(player))
			{
				return;
			}
			if (item.conditionNormalized >= 1f)
			{
				return;
			}
			item.DoRepair(this.conditionLost);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}
}
