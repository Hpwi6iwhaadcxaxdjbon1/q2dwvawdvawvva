using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000868 RID: 2152
public class LookatTooltip : MonoBehaviour
{
	// Token: 0x04003077 RID: 12407
	public static bool Enabled = true;

	// Token: 0x04003078 RID: 12408
	[NonSerialized]
	public BaseEntity currentlyLookingAt;

	// Token: 0x04003079 RID: 12409
	public RustText textLabel;

	// Token: 0x0400307A RID: 12410
	public Image icon;

	// Token: 0x0400307B RID: 12411
	public CanvasGroup canvasGroup;

	// Token: 0x0400307C RID: 12412
	public CanvasGroup infoGroup;

	// Token: 0x0400307D RID: 12413
	public CanvasGroup minimiseGroup;
}
