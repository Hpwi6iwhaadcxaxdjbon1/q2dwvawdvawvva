using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007C9 RID: 1993
public class KeyCodeEntry : UIDialog
{
	// Token: 0x04002C7A RID: 11386
	public Text textDisplay;

	// Token: 0x04002C7B RID: 11387
	public Action<string> onCodeEntered;

	// Token: 0x04002C7C RID: 11388
	public Action onClosed;

	// Token: 0x04002C7D RID: 11389
	public Text typeDisplay;

	// Token: 0x04002C7E RID: 11390
	public Translate.Phrase masterCodePhrase;

	// Token: 0x04002C7F RID: 11391
	public Translate.Phrase guestCodePhrase;

	// Token: 0x04002C80 RID: 11392
	public GameObject memoryKeycodeButton;
}
