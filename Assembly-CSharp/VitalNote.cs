using System;
using TMPro;
using UnityEngine;

// Token: 0x020008E7 RID: 2279
public class VitalNote : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x0400329D RID: 12957
	public VitalNote.Vital VitalType;

	// Token: 0x0400329E RID: 12958
	public FloatConditions showIf;

	// Token: 0x0400329F RID: 12959
	public TextMeshProUGUI valueText;

	// Token: 0x02000EAC RID: 3756
	public enum Vital
	{
		// Token: 0x04004C92 RID: 19602
		Comfort,
		// Token: 0x04004C93 RID: 19603
		Radiation,
		// Token: 0x04004C94 RID: 19604
		Poison,
		// Token: 0x04004C95 RID: 19605
		Cold,
		// Token: 0x04004C96 RID: 19606
		Bleeding,
		// Token: 0x04004C97 RID: 19607
		Hot,
		// Token: 0x04004C98 RID: 19608
		Oxygen,
		// Token: 0x04004C99 RID: 19609
		Wet,
		// Token: 0x04004C9A RID: 19610
		Hygiene,
		// Token: 0x04004C9B RID: 19611
		Starving,
		// Token: 0x04004C9C RID: 19612
		Dehydration
	}
}
