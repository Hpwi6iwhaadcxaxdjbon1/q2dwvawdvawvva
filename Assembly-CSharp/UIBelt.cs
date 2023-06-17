using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000851 RID: 2129
public class UIBelt : SingletonComponent<UIBelt>
{
	// Token: 0x04002FCA RID: 12234
	public List<ItemIcon> ItemIcons;

	// Token: 0x06003620 RID: 13856 RVA: 0x00148F15 File Offset: 0x00147115
	protected override void Awake()
	{
		this.ItemIcons = (from s in base.GetComponentsInChildren<ItemIcon>()
		orderby s.slot
		select s).ToList<ItemIcon>();
	}

	// Token: 0x06003621 RID: 13857 RVA: 0x00148F4C File Offset: 0x0014714C
	public ItemIcon GetItemIconAtSlot(int slot)
	{
		if (slot < 0 || slot >= this.ItemIcons.Count)
		{
			return null;
		}
		return this.ItemIcons[slot];
	}
}
