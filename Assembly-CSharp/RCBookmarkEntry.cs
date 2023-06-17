using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000106 RID: 262
public class RCBookmarkEntry : MonoBehaviour
{
	// Token: 0x04000DEC RID: 3564
	private ComputerMenu owner;

	// Token: 0x04000DED RID: 3565
	public RectTransform connectButton;

	// Token: 0x04000DEE RID: 3566
	public RectTransform disconnectButton;

	// Token: 0x04000DEF RID: 3567
	public RawImage onlineIndicator;

	// Token: 0x04000DF0 RID: 3568
	public RawImage offlineIndicator;

	// Token: 0x04000DF1 RID: 3569
	public GameObject selectedindicator;

	// Token: 0x04000DF2 RID: 3570
	public Image backgroundImage;

	// Token: 0x04000DF3 RID: 3571
	public Color selectedColor;

	// Token: 0x04000DF4 RID: 3572
	public Color activeColor;

	// Token: 0x04000DF5 RID: 3573
	public Color inactiveColor;

	// Token: 0x04000DF6 RID: 3574
	public Text nameLabel;

	// Token: 0x04000DF9 RID: 3577
	public EventTrigger eventTrigger;

	// Token: 0x170001E7 RID: 487
	// (get) Token: 0x060015AD RID: 5549 RVA: 0x000AAE20 File Offset: 0x000A9020
	// (set) Token: 0x060015AE RID: 5550 RVA: 0x000AAE28 File Offset: 0x000A9028
	public string identifier { get; private set; }

	// Token: 0x170001E8 RID: 488
	// (get) Token: 0x060015AF RID: 5551 RVA: 0x000AAE31 File Offset: 0x000A9031
	// (set) Token: 0x060015B0 RID: 5552 RVA: 0x000AAE39 File Offset: 0x000A9039
	public bool isSelected { get; private set; }

	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x060015B1 RID: 5553 RVA: 0x000AAE42 File Offset: 0x000A9042
	// (set) Token: 0x060015B2 RID: 5554 RVA: 0x000AAE4A File Offset: 0x000A904A
	public bool isControlling { get; private set; }
}
