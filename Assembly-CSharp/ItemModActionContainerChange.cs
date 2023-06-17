using System;
using UnityEngine;

// Token: 0x020005D1 RID: 1489
public class ItemModActionContainerChange : ItemMod
{
	// Token: 0x04002497 RID: 9367
	public ItemMod[] actions;

	// Token: 0x06002CF9 RID: 11513 RVA: 0x0010FC58 File Offset: 0x0010DE58
	public override void OnParentChanged(Item item)
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

	// Token: 0x06002CFA RID: 11514 RVA: 0x0010FCA0 File Offset: 0x0010DEA0
	private void OnValidate()
	{
		if (this.actions == null)
		{
			Debug.LogWarning("ItemModMenuOption: actions is null!", base.gameObject);
		}
	}
}
