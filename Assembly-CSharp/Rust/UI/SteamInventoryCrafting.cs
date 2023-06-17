using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	// Token: 0x02000B10 RID: 2832
	public class SteamInventoryCrafting : MonoBehaviour
	{
		// Token: 0x04003D44 RID: 15684
		public GameObject Container;

		// Token: 0x04003D45 RID: 15685
		public ToggleGroup ToggleGroup;

		// Token: 0x04003D46 RID: 15686
		public Button ConvertToItem;

		// Token: 0x04003D47 RID: 15687
		public TextMeshProUGUI WoodAmount;

		// Token: 0x04003D48 RID: 15688
		public TextMeshProUGUI ClothAmount;

		// Token: 0x04003D49 RID: 15689
		public TextMeshProUGUI MetalAmount;

		// Token: 0x04003D4A RID: 15690
		public TextMeshProUGUI InfoText;

		// Token: 0x04003D4D RID: 15693
		public SteamInventoryCrateOpen CraftModal;

		// Token: 0x04003D4E RID: 15694
		public GameObject CraftingContainer;

		// Token: 0x04003D4F RID: 15695
		public GameObject CraftingButton;

		// Token: 0x04003D50 RID: 15696
		public SteamInventoryNewItem NewItemModal;

		// Token: 0x1700063E RID: 1598
		// (get) Token: 0x06004505 RID: 17669 RVA: 0x00194ECA File Offset: 0x001930CA
		// (set) Token: 0x06004506 RID: 17670 RVA: 0x00194ED2 File Offset: 0x001930D2
		public IPlayerItemDefinition ResultItem { get; private set; }

		// Token: 0x1700063F RID: 1599
		// (get) Token: 0x06004507 RID: 17671 RVA: 0x00194EDB File Offset: 0x001930DB
		// (set) Token: 0x06004508 RID: 17672 RVA: 0x00194EE3 File Offset: 0x001930E3
		public Coroutine MarketCoroutine { get; private set; }
	}
}
