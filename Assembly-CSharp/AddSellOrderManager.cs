using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200012A RID: 298
public class AddSellOrderManager : MonoBehaviour
{
	// Token: 0x04000ECD RID: 3789
	public VirtualItemIcon sellItemIcon;

	// Token: 0x04000ECE RID: 3790
	public VirtualItemIcon currencyItemIcon;

	// Token: 0x04000ECF RID: 3791
	public GameObject itemSearchParent;

	// Token: 0x04000ED0 RID: 3792
	public ItemSearchEntry itemSearchEntryPrefab;

	// Token: 0x04000ED1 RID: 3793
	public InputField sellItemInput;

	// Token: 0x04000ED2 RID: 3794
	public InputField sellItemAmount;

	// Token: 0x04000ED3 RID: 3795
	public InputField currencyItemInput;

	// Token: 0x04000ED4 RID: 3796
	public InputField currencyItemAmount;

	// Token: 0x04000ED5 RID: 3797
	public VendingPanelAdmin adminPanel;
}
