using System;
using UnityEngine;

// Token: 0x0200012F RID: 303
public class LootPanelVendingMachine : LootPanel, IVendingMachineInterface
{
	// Token: 0x04000EE4 RID: 3812
	public GameObjectRef sellOrderPrefab;

	// Token: 0x04000EE5 RID: 3813
	public GameObject sellOrderContainer;

	// Token: 0x04000EE6 RID: 3814
	public GameObject busyOverlayPrefab;

	// Token: 0x04000EE7 RID: 3815
	private GameObject busyOverlayInstance;
}
