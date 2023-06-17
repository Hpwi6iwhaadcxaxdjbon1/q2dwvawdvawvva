using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200031A RID: 794
public class DevDressPlayer : MonoBehaviour
{
	// Token: 0x040017DE RID: 6110
	public bool DressRandomly;

	// Token: 0x040017DF RID: 6111
	public List<ItemAmount> clothesToWear;

	// Token: 0x06001ECB RID: 7883 RVA: 0x000D1BA8 File Offset: 0x000CFDA8
	private void ServerInitComponent()
	{
		BasePlayer component = base.GetComponent<BasePlayer>();
		if (this.DressRandomly)
		{
			this.DoRandomClothes(component);
		}
		foreach (ItemAmount itemAmount in this.clothesToWear)
		{
			if (!(itemAmount.itemDef == null))
			{
				ItemManager.Create(itemAmount.itemDef, 1, 0UL).MoveToContainer(component.inventory.containerWear, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06001ECC RID: 7884 RVA: 0x000D1C3C File Offset: 0x000CFE3C
	private void DoRandomClothes(BasePlayer player)
	{
		string text = "";
		foreach (ItemDefinition itemDefinition in (from x in ItemManager.GetItemDefinitions()
		where x.GetComponent<ItemModWearable>()
		orderby Guid.NewGuid()
		select x).Take(UnityEngine.Random.Range(0, 4)))
		{
			ItemManager.Create(itemDefinition, 1, 0UL).MoveToContainer(player.inventory.containerWear, -1, true, false, null, true);
			text = text + itemDefinition.shortname + " ";
		}
		text = text.Trim();
		if (text == "")
		{
			text = "naked";
		}
		player.displayName = text;
	}
}
