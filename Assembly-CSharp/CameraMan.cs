using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002A2 RID: 674
public class CameraMan : SingletonComponent<CameraMan>
{
	// Token: 0x0400160C RID: 5644
	public static string DefaultSaveName = string.Empty;

	// Token: 0x0400160D RID: 5645
	public const string SavePositionExtension = ".cam";

	// Token: 0x0400160E RID: 5646
	public const string SavePositionDirectory = "camsaves";

	// Token: 0x0400160F RID: 5647
	public bool OnlyControlWhenCursorHidden = true;

	// Token: 0x04001610 RID: 5648
	public bool NeedBothMouseButtonsToZoom;

	// Token: 0x04001611 RID: 5649
	public float LookSensitivity = 1f;

	// Token: 0x04001612 RID: 5650
	public float MoveSpeed = 1f;

	// Token: 0x04001613 RID: 5651
	public static float GuideAspect = 4f;

	// Token: 0x04001614 RID: 5652
	public static float GuideRatio = 3f;

	// Token: 0x04001615 RID: 5653
	public Canvas canvas;

	// Token: 0x04001616 RID: 5654
	public Graphic[] guides;
}
