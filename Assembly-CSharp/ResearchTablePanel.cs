using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200084D RID: 2125
public class ResearchTablePanel : LootPanel
{
	// Token: 0x04002FA0 RID: 12192
	public Button researchButton;

	// Token: 0x04002FA1 RID: 12193
	public Text timerText;

	// Token: 0x04002FA2 RID: 12194
	public GameObject itemDescNoItem;

	// Token: 0x04002FA3 RID: 12195
	public GameObject itemDescTooBroken;

	// Token: 0x04002FA4 RID: 12196
	public GameObject itemDescNotResearchable;

	// Token: 0x04002FA5 RID: 12197
	public GameObject itemDescTooMany;

	// Token: 0x04002FA6 RID: 12198
	public GameObject itemTakeBlueprint;

	// Token: 0x04002FA7 RID: 12199
	public GameObject itemDescAlreadyResearched;

	// Token: 0x04002FA8 RID: 12200
	public GameObject itemDescDefaultBlueprint;

	// Token: 0x04002FA9 RID: 12201
	public Text successChanceText;

	// Token: 0x04002FAA RID: 12202
	public ItemIcon scrapIcon;

	// Token: 0x04002FAB RID: 12203
	[NonSerialized]
	public bool wasResearching;

	// Token: 0x04002FAC RID: 12204
	public GameObject[] workbenchReqs;
}
