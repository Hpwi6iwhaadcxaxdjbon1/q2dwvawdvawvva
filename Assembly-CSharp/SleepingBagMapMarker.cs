using System;
using Rust.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007FB RID: 2043
public class SleepingBagMapMarker : MonoBehaviour
{
	// Token: 0x04002DD6 RID: 11734
	public Image MapIcon;

	// Token: 0x04002DD7 RID: 11735
	public Image SleepingBagIcon;

	// Token: 0x04002DD8 RID: 11736
	public Sprite SleepingBagSprite;

	// Token: 0x04002DD9 RID: 11737
	public Sprite BedSprite;

	// Token: 0x04002DDA RID: 11738
	public Sprite BeachTowelSprite;

	// Token: 0x04002DDB RID: 11739
	public Sprite CamperSprite;

	// Token: 0x04002DDC RID: 11740
	public Tooltip MarkerTooltip;

	// Token: 0x04002DDD RID: 11741
	public GameObject LockRoot;

	// Token: 0x04002DDE RID: 11742
	public TextMeshProUGUI LockTime;

	// Token: 0x04002DDF RID: 11743
	public GameObject OccupiedRoot;

	// Token: 0x04002DE0 RID: 11744
	public Image CircleRim;

	// Token: 0x04002DE1 RID: 11745
	public Image CircleFill;

	// Token: 0x04002DE2 RID: 11746
	public RustButton DeleteButton;

	// Token: 0x04002DE3 RID: 11747
	public Image ConfirmSlider;
}
