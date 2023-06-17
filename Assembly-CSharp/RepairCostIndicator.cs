using System;
using UnityEngine;

// Token: 0x020008B1 RID: 2225
public class RepairCostIndicator : SingletonComponent<RepairCostIndicator>, IClientComponent
{
	// Token: 0x040031E5 RID: 12773
	public RepairCostIndicatorRow[] Rows;

	// Token: 0x040031E6 RID: 12774
	public CanvasGroup Fader;
}
