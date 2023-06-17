using System;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class ItemSelector : PropertyAttribute
{
	// Token: 0x04002494 RID: 9364
	public ItemCategory category = ItemCategory.All;

	// Token: 0x06002CE5 RID: 11493 RVA: 0x0010FB9D File Offset: 0x0010DD9D
	public ItemSelector(ItemCategory category = ItemCategory.All)
	{
		this.category = category;
	}
}
