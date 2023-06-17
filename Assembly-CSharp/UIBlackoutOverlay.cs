using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C5 RID: 2245
public class UIBlackoutOverlay : MonoBehaviour
{
	// Token: 0x0400324D RID: 12877
	public CanvasGroup group;

	// Token: 0x0400324E RID: 12878
	public static Dictionary<UIBlackoutOverlay.blackoutType, UIBlackoutOverlay> instances;

	// Token: 0x0400324F RID: 12879
	public UIBlackoutOverlay.blackoutType overlayType = UIBlackoutOverlay.blackoutType.NONE;

	// Token: 0x02000EA8 RID: 3752
	public enum blackoutType
	{
		// Token: 0x04004C75 RID: 19573
		FULLBLACK,
		// Token: 0x04004C76 RID: 19574
		BINOCULAR,
		// Token: 0x04004C77 RID: 19575
		SCOPE,
		// Token: 0x04004C78 RID: 19576
		HELMETSLIT,
		// Token: 0x04004C79 RID: 19577
		SNORKELGOGGLE,
		// Token: 0x04004C7A RID: 19578
		NVG,
		// Token: 0x04004C7B RID: 19579
		FULLWHITE,
		// Token: 0x04004C7C RID: 19580
		SUNGLASSES,
		// Token: 0x04004C7D RID: 19581
		NONE = 64
	}
}
