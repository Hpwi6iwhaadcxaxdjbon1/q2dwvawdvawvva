using System;

// Token: 0x020001BB RID: 443
public class ItemModRFListener : ItemModAssociatedEntity<BaseEntity>
{
	// Token: 0x040011A0 RID: 4512
	public GameObjectRef frequencyPanelPrefab;

	// Token: 0x060018F9 RID: 6393 RVA: 0x000B886C File Offset: 0x000B6A6C
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
	}
}
