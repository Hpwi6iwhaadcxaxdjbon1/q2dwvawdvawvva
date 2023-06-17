using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000830 RID: 2096
public class LootPanel : MonoBehaviour
{
	// Token: 0x04002EFA RID: 12026
	public Text Title;

	// Token: 0x04002EFB RID: 12027
	public RustText TitleText;

	// Token: 0x04002EFC RID: 12028
	public bool hideInvalidIcons;

	// Token: 0x04002EFD RID: 12029
	[Tooltip("Only needed if hideInvalidIcons is true")]
	public CanvasGroup canvasGroup;

	// Token: 0x02000E85 RID: 3717
	public interface IHasLootPanel
	{
		// Token: 0x170006F5 RID: 1781
		// (get) Token: 0x060052C5 RID: 21189
		Translate.Phrase LootPanelTitle { get; }
	}
}
