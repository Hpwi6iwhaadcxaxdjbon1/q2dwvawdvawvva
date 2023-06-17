using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B14 RID: 2836
	public class SteamInventoryManager : SingletonComponent<SteamInventoryManager>
	{
		// Token: 0x04003D5C RID: 15708
		public GameObject inventoryItemPrefab;

		// Token: 0x04003D5D RID: 15709
		public GameObject inventoryCanvas;

		// Token: 0x04003D5E RID: 15710
		public GameObject missingItems;

		// Token: 0x04003D5F RID: 15711
		public SteamInventoryCrafting CraftControl;

		// Token: 0x04003D60 RID: 15712
		public List<GameObject> items;

		// Token: 0x04003D61 RID: 15713
		public GameObject LoadingOverlay;
	}
}
