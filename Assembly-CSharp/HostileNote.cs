using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E7 RID: 2023
public class HostileNote : MonoBehaviour, IClientComponent
{
	// Token: 0x04002D3F RID: 11583
	public CanvasGroup warnGroup;

	// Token: 0x04002D40 RID: 11584
	public CanvasGroup group;

	// Token: 0x04002D41 RID: 11585
	public CanvasGroup timerGroup;

	// Token: 0x04002D42 RID: 11586
	public CanvasGroup smallWarning;

	// Token: 0x04002D43 RID: 11587
	public Text timerText;

	// Token: 0x04002D44 RID: 11588
	public Text smallWarningText;

	// Token: 0x04002D45 RID: 11589
	public static float unhostileTime;

	// Token: 0x04002D46 RID: 11590
	public static float weaponDrawnDuration;

	// Token: 0x04002D47 RID: 11591
	public Color warnColor;

	// Token: 0x04002D48 RID: 11592
	public Color hostileColor;

	// Token: 0x04002D49 RID: 11593
	public float requireDistanceToSafeZone = 200f;
}
