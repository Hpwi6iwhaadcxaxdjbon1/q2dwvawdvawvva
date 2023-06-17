using System;
using System.Linq;
using UnityEngine;

// Token: 0x020005DD RID: 1501
public class ItemModConditionHasContents : ItemMod
{
	// Token: 0x040024B0 RID: 9392
	[Tooltip("Can be null to mean any item")]
	public ItemDefinition itemDef;

	// Token: 0x040024B1 RID: 9393
	public bool requiredState;

	// Token: 0x06002D1F RID: 11551 RVA: 0x00110260 File Offset: 0x0010E460
	public override bool Passes(Item item)
	{
		if (item.contents == null)
		{
			return !this.requiredState;
		}
		if (item.contents.itemList.Count == 0)
		{
			return !this.requiredState;
		}
		if (this.itemDef && !item.contents.itemList.Any((Item x) => x.info == this.itemDef))
		{
			return !this.requiredState;
		}
		return this.requiredState;
	}
}
