using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020008C7 RID: 2247
public class UIConversationScreen : SingletonComponent<UIConversationScreen>, IUIScreen
{
	// Token: 0x04003255 RID: 12885
	public NeedsCursor needsCursor;

	// Token: 0x04003256 RID: 12886
	public RectTransform conversationPanel;

	// Token: 0x04003257 RID: 12887
	public RustText conversationSpeechBody;

	// Token: 0x04003258 RID: 12888
	public RustText conversationProviderName;

	// Token: 0x04003259 RID: 12889
	public RustButton[] responseButtons;

	// Token: 0x0400325A RID: 12890
	public RectTransform letterBoxTop;

	// Token: 0x0400325B RID: 12891
	public RectTransform letterBoxBottom;

	// Token: 0x0400325C RID: 12892
	protected CanvasGroup canvasGroup;
}
