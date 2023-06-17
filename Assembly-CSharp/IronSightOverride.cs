using System;
using UnityEngine;

// Token: 0x0200096B RID: 2411
public class IronSightOverride : MonoBehaviour
{
	// Token: 0x040033DF RID: 13279
	public IronsightAimPoint aimPoint;

	// Token: 0x040033E0 RID: 13280
	public float fieldOfViewOffset = -20f;

	// Token: 0x040033E1 RID: 13281
	public float zoomFactor = -1f;

	// Token: 0x040033E2 RID: 13282
	[Tooltip("If set to 1, the FOV is set to what this override is set to. If set to 0.5 it's half way between the weapon iconsights default and this scope.")]
	public float fovBias = 0.5f;
}
