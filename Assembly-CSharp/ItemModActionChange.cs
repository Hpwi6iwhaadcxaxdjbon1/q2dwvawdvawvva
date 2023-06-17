using System;
using UnityEngine;

// Token: 0x020005D0 RID: 1488
public class ItemModActionChange : ItemMod
{
	// Token: 0x04002496 RID: 9366
	public ItemMod[] actions;

	// Token: 0x06002CF6 RID: 11510 RVA: 0x0010FBF4 File Offset: 0x0010DDF4
	public override void OnChanged(Item item)
	{
		if (!item.isServer)
		{
			return;
		}
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		foreach (ItemMod itemMod in this.actions)
		{
			if (itemMod.CanDoAction(item, ownerPlayer))
			{
				itemMod.DoAction(item, ownerPlayer);
			}
		}
	}

	// Token: 0x06002CF7 RID: 11511 RVA: 0x0010FC3C File Offset: 0x0010DE3C
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}
}
