using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085D RID: 2141
public class WorkbenchPanel : LootPanel, IInventoryChanged
{
	// Token: 0x04003019 RID: 12313
	public Button experimentButton;

	// Token: 0x0400301A RID: 12314
	public Text timerText;

	// Token: 0x0400301B RID: 12315
	public Text costText;

	// Token: 0x0400301C RID: 12316
	public GameObject expermentCostParent;

	// Token: 0x0400301D RID: 12317
	public GameObject controlsParent;

	// Token: 0x0400301E RID: 12318
	public GameObject allUnlockedNotification;

	// Token: 0x0400301F RID: 12319
	public GameObject informationParent;

	// Token: 0x04003020 RID: 12320
	public GameObject cycleIcon;

	// Token: 0x04003021 RID: 12321
	public TechTreeDialog techTreeDialog;
}
