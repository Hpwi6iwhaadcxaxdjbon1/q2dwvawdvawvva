using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008E8 RID: 2280
public class VitalNoteOxygen : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x040032A0 RID: 12960
	[SerializeField]
	private float refreshTime = 1f;

	// Token: 0x040032A1 RID: 12961
	[SerializeField]
	private TextMeshProUGUI valueText;

	// Token: 0x040032A2 RID: 12962
	[SerializeField]
	private Animator animator;

	// Token: 0x040032A3 RID: 12963
	[SerializeField]
	private Image airIcon;

	// Token: 0x040032A4 RID: 12964
	[SerializeField]
	private RectTransform airIconTr;

	// Token: 0x040032A5 RID: 12965
	[SerializeField]
	private Image backgroundImage;

	// Token: 0x040032A6 RID: 12966
	[SerializeField]
	private Color baseColour;

	// Token: 0x040032A7 RID: 12967
	[SerializeField]
	private Color badColour;

	// Token: 0x040032A8 RID: 12968
	[SerializeField]
	private Image iconImage;

	// Token: 0x040032A9 RID: 12969
	[SerializeField]
	private Color iconBaseColour;

	// Token: 0x040032AA RID: 12970
	[SerializeField]
	private Color iconBadColour;

	// Token: 0x040032AB RID: 12971
	protected bool show = true;
}
