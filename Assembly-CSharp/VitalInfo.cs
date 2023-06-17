using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008E6 RID: 2278
public class VitalInfo : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x04003299 RID: 12953
	public HudElement Element;

	// Token: 0x0400329A RID: 12954
	public Image InfoImage;

	// Token: 0x0400329B RID: 12955
	public VitalInfo.Vital VitalType;

	// Token: 0x0400329C RID: 12956
	public TextMeshProUGUI text;

	// Token: 0x02000EAB RID: 3755
	public enum Vital
	{
		// Token: 0x04004C86 RID: 19590
		BuildingBlocked,
		// Token: 0x04004C87 RID: 19591
		CanBuild,
		// Token: 0x04004C88 RID: 19592
		Crafting,
		// Token: 0x04004C89 RID: 19593
		CraftLevel1,
		// Token: 0x04004C8A RID: 19594
		CraftLevel2,
		// Token: 0x04004C8B RID: 19595
		CraftLevel3,
		// Token: 0x04004C8C RID: 19596
		DecayProtected,
		// Token: 0x04004C8D RID: 19597
		Decaying,
		// Token: 0x04004C8E RID: 19598
		SafeZone,
		// Token: 0x04004C8F RID: 19599
		Buffed,
		// Token: 0x04004C90 RID: 19600
		Pet
	}
}
