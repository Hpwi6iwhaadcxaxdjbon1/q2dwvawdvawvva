using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008C8 RID: 2248
public class UIDeathScreen : SingletonComponent<UIDeathScreen>, IUIScreen
{
	// Token: 0x0400325D RID: 12893
	public LifeInfographic previousLifeInfographic;

	// Token: 0x0400325E RID: 12894
	public Animator screenAnimator;

	// Token: 0x0400325F RID: 12895
	public bool fadeIn;

	// Token: 0x04003260 RID: 12896
	public Button ReportCheatButton;

	// Token: 0x04003261 RID: 12897
	public MapView View;

	// Token: 0x04003262 RID: 12898
	public List<SleepingBagButton> sleepingBagButtons = new List<SleepingBagButton>();

	// Token: 0x04003263 RID: 12899
	public UIDeathScreen.RespawnColourScheme[] RespawnColourSchemes;

	// Token: 0x04003264 RID: 12900
	public GameObject RespawnScrollGradient;

	// Token: 0x04003265 RID: 12901
	public ScrollRect RespawnScrollRect;

	// Token: 0x04003266 RID: 12902
	public ExpandedLifeStats ExpandedStats;

	// Token: 0x04003267 RID: 12903
	public CanvasGroup StreamerModeContainer;

	// Token: 0x02000EA9 RID: 3753
	[Serializable]
	public struct RespawnColourScheme
	{
		// Token: 0x04004C7E RID: 19582
		public Color BackgroundColour;

		// Token: 0x04004C7F RID: 19583
		public Color CircleRimColour;

		// Token: 0x04004C80 RID: 19584
		public Color CircleFillColour;
	}
}
