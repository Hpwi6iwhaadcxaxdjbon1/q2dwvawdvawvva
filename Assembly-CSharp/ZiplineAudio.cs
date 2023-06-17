using System;
using UnityEngine;

// Token: 0x020000F5 RID: 245
public class ZiplineAudio : MonoBehaviour
{
	// Token: 0x04000D8E RID: 3470
	public ZiplineMountable zipline;

	// Token: 0x04000D8F RID: 3471
	public SoundDefinition movementLoopDef;

	// Token: 0x04000D90 RID: 3472
	public SoundDefinition frictionLoopDef;

	// Token: 0x04000D91 RID: 3473
	public SoundDefinition sparksLoopDef;

	// Token: 0x04000D92 RID: 3474
	public AnimationCurve movementGainCurve;

	// Token: 0x04000D93 RID: 3475
	public AnimationCurve movementPitchCurve;

	// Token: 0x04000D94 RID: 3476
	public AnimationCurve frictionGainCurve;

	// Token: 0x04000D95 RID: 3477
	public AnimationCurve sparksGainCurve;
}
