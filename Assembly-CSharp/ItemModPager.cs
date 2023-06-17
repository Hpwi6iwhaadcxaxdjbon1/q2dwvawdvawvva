using System;

// Token: 0x020001BA RID: 442
public class ItemModPager : ItemModRFListener
{
	// Token: 0x060018F7 RID: 6391 RVA: 0x000B87FC File Offset: 0x000B69FC
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		PagerEntity component = ItemModAssociatedEntity<BaseEntity>.GetAssociatedEntity(item, true).GetComponent<PagerEntity>();
		if (component)
		{
			if (command == "stop")
			{
				component.SetOff();
				return;
			}
			if (command == "silenton")
			{
				component.SetSilentMode(true);
				return;
			}
			if (command == "silentoff")
			{
				component.SetSilentMode(false);
			}
		}
	}
}
