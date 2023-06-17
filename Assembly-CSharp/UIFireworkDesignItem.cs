using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007C6 RID: 1990
public class UIFireworkDesignItem : MonoBehaviour
{
	// Token: 0x04002C6B RID: 11371
	public static readonly Translate.Phrase EmptyPhrase = new Translate.Phrase("firework.pattern.design.empty", "Empty");

	// Token: 0x04002C6C RID: 11372
	public static readonly Translate.Phrase UntitledPhrase = new Translate.Phrase("firework.pattern.design.untitled", "Untitled");

	// Token: 0x04002C6D RID: 11373
	public RustText Title;

	// Token: 0x04002C6E RID: 11374
	public RustButton LoadButton;

	// Token: 0x04002C6F RID: 11375
	public RustButton SaveButton;

	// Token: 0x04002C70 RID: 11376
	public RustButton EraseButton;

	// Token: 0x04002C71 RID: 11377
	public UIFireworkDesigner Designer;

	// Token: 0x04002C72 RID: 11378
	public int Index;
}
