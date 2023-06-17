using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007A0 RID: 1952
public class Crosshair : BaseMonoBehaviour
{
	// Token: 0x04002B4F RID: 11087
	public static bool Enabled = true;

	// Token: 0x04002B50 RID: 11088
	public Image Image;

	// Token: 0x04002B51 RID: 11089
	public RectTransform reticleTransform;

	// Token: 0x04002B52 RID: 11090
	public CanvasGroup reticleAlpha;

	// Token: 0x04002B53 RID: 11091
	public RectTransform hitNotifyMarker;

	// Token: 0x04002B54 RID: 11092
	public CanvasGroup hitNotifyAlpha;

	// Token: 0x04002B55 RID: 11093
	public static Crosshair instance;

	// Token: 0x04002B56 RID: 11094
	public static float lastHitTime = 0f;

	// Token: 0x04002B57 RID: 11095
	public float crosshairAlpha = 0.75f;
}
