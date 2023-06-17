using System;
using UnityEngine;

// Token: 0x0200096C RID: 2412
public class IronSights : MonoBehaviour
{
	// Token: 0x040033E3 RID: 13283
	public bool Enabled;

	// Token: 0x040033E4 RID: 13284
	[Header("View Setup")]
	public IronsightAimPoint aimPoint;

	// Token: 0x040033E5 RID: 13285
	public float fieldOfViewOffset = -20f;

	// Token: 0x040033E6 RID: 13286
	public float zoomFactor = 1f;

	// Token: 0x040033E7 RID: 13287
	[Header("Animation")]
	public float introSpeed = 1f;

	// Token: 0x040033E8 RID: 13288
	public AnimationCurve introCurve = new AnimationCurve();

	// Token: 0x040033E9 RID: 13289
	public float outroSpeed = 1f;

	// Token: 0x040033EA RID: 13290
	public AnimationCurve outroCurve = new AnimationCurve();

	// Token: 0x040033EB RID: 13291
	[Header("Sounds")]
	public SoundDefinition upSound;

	// Token: 0x040033EC RID: 13292
	public SoundDefinition downSound;

	// Token: 0x040033ED RID: 13293
	[Header("Info")]
	public IronSightOverride ironsightsOverride;

	// Token: 0x040033EE RID: 13294
	public bool processUltrawideOffset;
}
