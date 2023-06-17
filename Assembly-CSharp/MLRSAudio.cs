using System;
using UnityEngine;

// Token: 0x02000486 RID: 1158
public class MLRSAudio : MonoBehaviour
{
	// Token: 0x04001E64 RID: 7780
	[SerializeField]
	private MLRS mlrs;

	// Token: 0x04001E65 RID: 7781
	[SerializeField]
	private Transform pitchTransform;

	// Token: 0x04001E66 RID: 7782
	[SerializeField]
	private Transform yawTransform;

	// Token: 0x04001E67 RID: 7783
	[SerializeField]
	private float pitchDeltaSmoothRate = 5f;

	// Token: 0x04001E68 RID: 7784
	[SerializeField]
	private float yawDeltaSmoothRate = 5f;

	// Token: 0x04001E69 RID: 7785
	[SerializeField]
	private float pitchDeltaThreshold = 0.5f;

	// Token: 0x04001E6A RID: 7786
	[SerializeField]
	private float yawDeltaThreshold = 0.5f;

	// Token: 0x04001E6B RID: 7787
	private float lastPitch;

	// Token: 0x04001E6C RID: 7788
	private float lastYaw;

	// Token: 0x04001E6D RID: 7789
	private float pitchDelta;

	// Token: 0x04001E6E RID: 7790
	private float yawDelta;

	// Token: 0x04001E6F RID: 7791
	public SoundDefinition turretMovementStartDef;

	// Token: 0x04001E70 RID: 7792
	public SoundDefinition turretMovementLoopDef;

	// Token: 0x04001E71 RID: 7793
	public SoundDefinition turretMovementStopDef;

	// Token: 0x04001E72 RID: 7794
	private Sound turretMovementLoop;
}
