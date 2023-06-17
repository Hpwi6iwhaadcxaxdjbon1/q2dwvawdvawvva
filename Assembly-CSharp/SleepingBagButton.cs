using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008B6 RID: 2230
public class SleepingBagButton : MonoBehaviour
{
	// Token: 0x040031FF RID: 12799
	public GameObject TimeLockRoot;

	// Token: 0x04003200 RID: 12800
	public GameObject LockRoot;

	// Token: 0x04003201 RID: 12801
	public GameObject OccupiedRoot;

	// Token: 0x04003202 RID: 12802
	public Button ClickButton;

	// Token: 0x04003203 RID: 12803
	public TextMeshProUGUI BagName;

	// Token: 0x04003204 RID: 12804
	public TextMeshProUGUI LockTime;

	// Token: 0x04003205 RID: 12805
	public Image Icon;

	// Token: 0x04003206 RID: 12806
	public Sprite SleepingBagSprite;

	// Token: 0x04003207 RID: 12807
	public Sprite BedSprite;

	// Token: 0x04003208 RID: 12808
	public Sprite BeachTowelSprite;

	// Token: 0x04003209 RID: 12809
	public Sprite CamperSprite;

	// Token: 0x0400320A RID: 12810
	public Image CircleRim;

	// Token: 0x0400320B RID: 12811
	public Image CircleFill;

	// Token: 0x0400320C RID: 12812
	public Image Background;

	// Token: 0x0400320D RID: 12813
	public RustButton DeleteButton;

	// Token: 0x0400320E RID: 12814
	public Image ConfirmSlider;

	// Token: 0x0400320F RID: 12815
	public static Translate.Phrase toastHoldToUnclaimBag = new Translate.Phrase("hold_unclaim_bag", "Hold down the delete button to unclaim a sleeping bag");
}
