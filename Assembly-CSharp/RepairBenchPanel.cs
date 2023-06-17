using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200084C RID: 2124
public class RepairBenchPanel : LootPanel
{
	// Token: 0x04002F94 RID: 12180
	public Text infoText;

	// Token: 0x04002F95 RID: 12181
	public Button repairButton;

	// Token: 0x04002F96 RID: 12182
	public Color gotColor;

	// Token: 0x04002F97 RID: 12183
	public Color notGotColor;

	// Token: 0x04002F98 RID: 12184
	public Translate.Phrase phraseEmpty;

	// Token: 0x04002F99 RID: 12185
	public Translate.Phrase phraseNotRepairable;

	// Token: 0x04002F9A RID: 12186
	public Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x04002F9B RID: 12187
	public Translate.Phrase phraseNoBlueprint;

	// Token: 0x04002F9C RID: 12188
	public GameObject skinsPanel;

	// Token: 0x04002F9D RID: 12189
	public GameObject changeSkinDialog;

	// Token: 0x04002F9E RID: 12190
	public IconSkinPicker picker;

	// Token: 0x04002F9F RID: 12191
	public GameObject attachmentSkinBlocker;
}
