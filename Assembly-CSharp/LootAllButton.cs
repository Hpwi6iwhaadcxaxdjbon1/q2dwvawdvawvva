using System;
using UnityEngine;

// Token: 0x02000840 RID: 2112
public class LootAllButton : MonoBehaviour
{
	// Token: 0x04002F3C RID: 12092
	public Func<Item, bool> Filter;

	// Token: 0x04002F3D RID: 12093
	public OvenLootPanel inventoryGrid;
}
