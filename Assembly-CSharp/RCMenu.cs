using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// Token: 0x02000108 RID: 264
public class RCMenu : ComputerMenu
{
	// Token: 0x04000DFA RID: 3578
	public Image backgroundOpaque;

	// Token: 0x04000DFB RID: 3579
	public InputField newBookmarkEntryField;

	// Token: 0x04000DFC RID: 3580
	public NeedsCursor needsCursor;

	// Token: 0x04000DFD RID: 3581
	public float hiddenOffset = -256f;

	// Token: 0x04000DFE RID: 3582
	public RectTransform devicesPanel;

	// Token: 0x04000DFF RID: 3583
	private Vector3 initialDevicesPosition;

	// Token: 0x04000E00 RID: 3584
	public static bool isControllingCamera;

	// Token: 0x04000E01 RID: 3585
	public CanvasGroup overExposure;

	// Token: 0x04000E02 RID: 3586
	public CanvasGroup interference;

	// Token: 0x04000E03 RID: 3587
	public float interferenceFadeDuration = 0.2f;

	// Token: 0x04000E04 RID: 3588
	public float rangeInterferenceScale = 10000f;

	// Token: 0x04000E05 RID: 3589
	public Text timeText;

	// Token: 0x04000E06 RID: 3590
	public Text watchedDurationText;

	// Token: 0x04000E07 RID: 3591
	public Text deviceNameText;

	// Token: 0x04000E08 RID: 3592
	public Text noSignalText;

	// Token: 0x04000E09 RID: 3593
	public Text healthText;

	// Token: 0x04000E0A RID: 3594
	public GameObject healthBarParent;

	// Token: 0x04000E0B RID: 3595
	public RectTransform healthBarBackground;

	// Token: 0x04000E0C RID: 3596
	public RectTransform healthBarFill;

	// Token: 0x04000E0D RID: 3597
	public SoundDefinition bookmarkPressedSoundDef;

	// Token: 0x04000E0E RID: 3598
	public GameObject[] hideIfStatic;

	// Token: 0x04000E0F RID: 3599
	public GameObject readOnlyIndicator;

	// Token: 0x04000E10 RID: 3600
	[FormerlySerializedAs("crosshair")]
	public GameObject aimCrosshair;

	// Token: 0x04000E11 RID: 3601
	public GameObject generalCrosshair;

	// Token: 0x04000E12 RID: 3602
	public float fogOverrideDensity = 0.1f;

	// Token: 0x04000E13 RID: 3603
	public float autoTurretFogDistance = 30f;

	// Token: 0x04000E14 RID: 3604
	public float autoTurretDotBaseScale = 2f;

	// Token: 0x04000E15 RID: 3605
	public float autoTurretDotGrowScale = 4f;

	// Token: 0x04000E16 RID: 3606
	public PingManager PingManager;

	// Token: 0x04000E17 RID: 3607
	public ScrollRectSettable scrollRect;
}
