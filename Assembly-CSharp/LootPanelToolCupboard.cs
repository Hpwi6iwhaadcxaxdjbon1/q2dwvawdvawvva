using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000125 RID: 293
public class LootPanelToolCupboard : LootPanel
{
	// Token: 0x04000EBF RID: 3775
	public List<VirtualItemIcon> costIcons;

	// Token: 0x04000EC0 RID: 3776
	public Text costPerTimeText;

	// Token: 0x04000EC1 RID: 3777
	public Text protectedText;

	// Token: 0x04000EC2 RID: 3778
	public GameObject baseNotProtectedObj;

	// Token: 0x04000EC3 RID: 3779
	public GameObject baseProtectedObj;

	// Token: 0x04000EC4 RID: 3780
	public Translate.Phrase protectedPrefix;

	// Token: 0x04000EC5 RID: 3781
	public Tooltip costToolTip;

	// Token: 0x04000EC6 RID: 3782
	public Translate.Phrase blocksPhrase;
}
