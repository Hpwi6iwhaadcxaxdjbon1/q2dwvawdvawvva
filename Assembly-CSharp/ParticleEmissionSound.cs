using System;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class ParticleEmissionSound : FacepunchBehaviour, IClientComponent, ILOD
{
	// Token: 0x0400186D RID: 6253
	public ParticleSystem particleSystem;

	// Token: 0x0400186E RID: 6254
	public SoundDefinition soundDefinition;

	// Token: 0x0400186F RID: 6255
	public float soundCooldown;
}
