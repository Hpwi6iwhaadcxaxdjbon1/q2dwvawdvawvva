using System;
using UnityEngine;

// Token: 0x020005A8 RID: 1448
public abstract class WeatherEffect : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x0400238C RID: 9100
	public ParticleSystem[] emitOnStart;

	// Token: 0x0400238D RID: 9101
	public ParticleSystem[] emitOnStop;

	// Token: 0x0400238E RID: 9102
	public ParticleSystem[] emitOnLoop;
}
