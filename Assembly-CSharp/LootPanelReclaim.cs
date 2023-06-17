using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200083B RID: 2107
public class LootPanelReclaim : LootPanel
{
	// Token: 0x04002F20 RID: 12064
	public int oldOverflow = -1;

	// Token: 0x04002F21 RID: 12065
	public Text overflowText;

	// Token: 0x04002F22 RID: 12066
	public GameObject overflowObject;

	// Token: 0x04002F23 RID: 12067
	public static readonly Translate.Phrase MorePhrase = new Translate.Phrase("reclaim.more", "additional items...");
}
