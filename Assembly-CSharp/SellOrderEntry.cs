using System;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class SellOrderEntry : MonoBehaviour, IInventoryChanged
{
	// Token: 0x04000EE8 RID: 3816
	public VirtualItemIcon MerchandiseIcon;

	// Token: 0x04000EE9 RID: 3817
	public VirtualItemIcon CurrencyIcon;

	// Token: 0x04000EEA RID: 3818
	private ItemDefinition merchandiseInfo;

	// Token: 0x04000EEB RID: 3819
	private ItemDefinition currencyInfo;

	// Token: 0x04000EEC RID: 3820
	public GameObject buyButton;

	// Token: 0x04000EED RID: 3821
	public GameObject cantaffordNotification;

	// Token: 0x04000EEE RID: 3822
	public GameObject outOfStockNotification;

	// Token: 0x04000EEF RID: 3823
	private IVendingMachineInterface vendingPanel;

	// Token: 0x04000EF0 RID: 3824
	public UIIntegerEntry intEntry;
}
