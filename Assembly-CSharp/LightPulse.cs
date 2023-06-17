using System;
using UnityEngine;

// Token: 0x02000344 RID: 836
public class LightPulse : MonoBehaviour, IClientComponent
{
	// Token: 0x0400185B RID: 6235
	public Light TargetLight;

	// Token: 0x0400185C RID: 6236
	public float PulseSpeed = 1f;

	// Token: 0x0400185D RID: 6237
	public float Lifetime = 3f;

	// Token: 0x0400185E RID: 6238
	public float MaxIntensity = 3f;

	// Token: 0x0400185F RID: 6239
	public float FadeOutSpeed = 2f;
}
