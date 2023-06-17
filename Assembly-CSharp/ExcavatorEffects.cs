using System;
using UnityEngine;

// Token: 0x02000434 RID: 1076
public class ExcavatorEffects : MonoBehaviour
{
	// Token: 0x04001C57 RID: 7255
	public static ExcavatorEffects instance;

	// Token: 0x04001C58 RID: 7256
	public ParticleSystemContainer[] miningParticles;

	// Token: 0x04001C59 RID: 7257
	public SoundPlayer[] miningSounds;

	// Token: 0x04001C5A RID: 7258
	public SoundFollowCollider[] beltSounds;

	// Token: 0x04001C5B RID: 7259
	public SoundPlayer[] miningStartSounds;

	// Token: 0x04001C5C RID: 7260
	public GameObject[] ambientMetalRattles;

	// Token: 0x04001C5D RID: 7261
	public bool wasMining;
}
