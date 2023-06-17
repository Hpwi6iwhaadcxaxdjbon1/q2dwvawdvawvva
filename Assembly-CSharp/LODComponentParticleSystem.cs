using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public abstract class LODComponentParticleSystem : LODComponent
{
	// Token: 0x040021F6 RID: 8694
	[Tooltip("Automatically call Play() the particle system when it's shown via LOD")]
	public bool playOnShow = true;
}
