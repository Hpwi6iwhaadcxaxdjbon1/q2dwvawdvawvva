using System;
using UnityEngine;

// Token: 0x02000331 RID: 817
public class BucketVMFluidSim : MonoBehaviour
{
	// Token: 0x04001810 RID: 6160
	public Animator waterbucketAnim;

	// Token: 0x04001811 RID: 6161
	public ParticleSystem waterPour;

	// Token: 0x04001812 RID: 6162
	public ParticleSystem waterTurbulence;

	// Token: 0x04001813 RID: 6163
	public ParticleSystem waterFill;

	// Token: 0x04001814 RID: 6164
	public float waterLevel;

	// Token: 0x04001815 RID: 6165
	public float targetWaterLevel;

	// Token: 0x04001816 RID: 6166
	public AudioSource waterSpill;

	// Token: 0x04001817 RID: 6167
	private float PlayerEyePitch;

	// Token: 0x04001818 RID: 6168
	private float turb_forward;

	// Token: 0x04001819 RID: 6169
	private float turb_side;

	// Token: 0x0400181A RID: 6170
	private Vector3 lastPosition;

	// Token: 0x0400181B RID: 6171
	protected Vector3 groundSpeedLast;

	// Token: 0x0400181C RID: 6172
	private Vector3 lastAngle;

	// Token: 0x0400181D RID: 6173
	protected Vector3 vecAngleSpeedLast;

	// Token: 0x0400181E RID: 6174
	private Vector3 initialPosition;
}
