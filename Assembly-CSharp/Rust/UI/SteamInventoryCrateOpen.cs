using System;
using TMPro;
using UnityEngine;

namespace Rust.UI
{
	// Token: 0x02000B11 RID: 2833
	public class SteamInventoryCrateOpen : MonoBehaviour
	{
		// Token: 0x04003D51 RID: 15697
		public TextMeshProUGUI Name;

		// Token: 0x04003D52 RID: 15698
		public TextMeshProUGUI Requirements;

		// Token: 0x04003D53 RID: 15699
		public TextMeshProUGUI Label;

		// Token: 0x04003D54 RID: 15700
		public HttpImage IconImage;

		// Token: 0x04003D55 RID: 15701
		public GameObject ErrorPanel;

		// Token: 0x04003D56 RID: 15702
		public TextMeshProUGUI ErrorText;

		// Token: 0x04003D57 RID: 15703
		public GameObject CraftButton;

		// Token: 0x04003D58 RID: 15704
		public GameObject ProgressPanel;

		// Token: 0x04003D59 RID: 15705
		public SteamInventoryNewItem NewItemModal;
	}
}
