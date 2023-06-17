using System;
using UnityEngine;

// Token: 0x020005F0 RID: 1520
public class ItemModMenuOption : ItemMod
{
	// Token: 0x040024F3 RID: 9459
	public string commandName;

	// Token: 0x040024F4 RID: 9460
	public ItemMod actionTarget;

	// Token: 0x040024F5 RID: 9461
	public BaseEntity.Menu.Option option;

	// Token: 0x040024F6 RID: 9462
	[Tooltip("If true, this is the command that will run when an item is 'selected' on the toolbar")]
	public bool isPrimaryOption = true;

	// Token: 0x06002D60 RID: 11616 RVA: 0x0011135D File Offset: 0x0010F55D
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command != this.commandName)
		{
			return;
		}
		if (!this.actionTarget.CanDoAction(item, player))
		{
			return;
		}
		this.actionTarget.DoAction(item, player);
	}

	// Token: 0x06002D61 RID: 11617 RVA: 0x0011138C File Offset: 0x0010F58C
	private void OnValidate()
	{
		if (this.actionTarget == null)
		{
			Debug.LogWarning("ItemModMenuOption: actionTarget is null!", base.gameObject);
		}
		if (string.IsNullOrEmpty(this.commandName))
		{
			Debug.LogWarning("ItemModMenuOption: commandName can't be empty!", base.gameObject);
		}
		if (this.option.icon == null)
		{
			Debug.LogWarning("No icon set for ItemModMenuOption " + base.gameObject.name, base.gameObject);
		}
	}
}
