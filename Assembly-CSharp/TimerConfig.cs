using System;
using UnityEngine.UI;

// Token: 0x02000132 RID: 306
public class TimerConfig : UIDialog
{
	// Token: 0x04000EF1 RID: 3825
	[NonSerialized]
	private CustomTimerSwitch timerSwitch;

	// Token: 0x04000EF2 RID: 3826
	public InputField input;

	// Token: 0x04000EF3 RID: 3827
	public static float minTime = 0.25f;

	// Token: 0x04000EF4 RID: 3828
	public float seconds;
}
