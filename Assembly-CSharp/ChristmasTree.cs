using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018A RID: 394
public class ChristmasTree : StorageContainer
{
	// Token: 0x040010A3 RID: 4259
	public GameObject[] decorations;

	// Token: 0x060017CC RID: 6092 RVA: 0x000B36C8 File Offset: 0x000B18C8
	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (item.info.GetComponent<ItemModXMasTreeDecoration>() == null)
		{
			return false;
		}
		using (List<Item>.Enumerator enumerator = base.inventory.itemList.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.info == item.info)
				{
					return false;
				}
			}
		}
		return base.ItemFilter(item, targetSlot);
	}

	// Token: 0x060017CD RID: 6093 RVA: 0x000B3750 File Offset: 0x000B1950
	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		ItemModXMasTreeDecoration component = item.info.GetComponent<ItemModXMasTreeDecoration>();
		if (component != null)
		{
			base.SetFlag((BaseEntity.Flags)component.flagsToChange, added, false, true);
		}
		base.OnItemAddedOrRemoved(item, added);
	}
}
