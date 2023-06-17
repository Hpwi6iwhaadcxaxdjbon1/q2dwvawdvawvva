using System;
using UnityEngine;

// Token: 0x02000182 RID: 386
public class ItemModUnwrap : ItemMod
{
	// Token: 0x04001094 RID: 4244
	public LootSpawn revealList;

	// Token: 0x04001095 RID: 4245
	public GameObjectRef successEffect;

	// Token: 0x04001096 RID: 4246
	public int minTries = 1;

	// Token: 0x04001097 RID: 4247
	public int maxTries = 1;

	// Token: 0x060017BC RID: 6076 RVA: 0x000B33B8 File Offset: 0x000B15B8
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "unwrap")
		{
			if (item.amount <= 0)
			{
				return;
			}
			item.UseItem(1);
			int num = UnityEngine.Random.Range(this.minTries, this.maxTries + 1);
			for (int i = 0; i < num; i++)
			{
				this.revealList.SpawnIntoContainer(player.inventory.containerMain);
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}
}
