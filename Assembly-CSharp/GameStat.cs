using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007E4 RID: 2020
public class GameStat : MonoBehaviour
{
	// Token: 0x04002D2B RID: 11563
	public float refreshTime = 5f;

	// Token: 0x04002D2C RID: 11564
	public Text title;

	// Token: 0x04002D2D RID: 11565
	public Text globalStat;

	// Token: 0x04002D2E RID: 11566
	public Text localStat;

	// Token: 0x04002D2F RID: 11567
	private long globalValue;

	// Token: 0x04002D30 RID: 11568
	private long localValue;

	// Token: 0x04002D31 RID: 11569
	private float secondsSinceRefresh;

	// Token: 0x04002D32 RID: 11570
	private float secondsUntilUpdate;

	// Token: 0x04002D33 RID: 11571
	private float secondsUntilChange;

	// Token: 0x04002D34 RID: 11572
	public GameStat.Stat[] stats;

	// Token: 0x02000E7B RID: 3707
	[Serializable]
	public struct Stat
	{
		// Token: 0x04004BDA RID: 19418
		public string statName;

		// Token: 0x04004BDB RID: 19419
		public string statTitle;
	}
}
