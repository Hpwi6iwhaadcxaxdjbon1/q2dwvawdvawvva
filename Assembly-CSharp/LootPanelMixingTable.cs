using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000838 RID: 2104
public class LootPanelMixingTable : LootPanel, IInventoryChanged
{
	// Token: 0x04002F18 RID: 12056
	public GameObject controlsOn;

	// Token: 0x04002F19 RID: 12057
	public GameObject controlsOff;

	// Token: 0x04002F1A RID: 12058
	public Button StartMixingButton;

	// Token: 0x04002F1B RID: 12059
	public InfoBar ProgressBar;

	// Token: 0x04002F1C RID: 12060
	public GameObject recipeItemPrefab;

	// Token: 0x04002F1D RID: 12061
	public RectTransform recipeContentRect;
}
