using System;
using UnityEngine;

// Token: 0x02000141 RID: 321
public class FirecrackerRepeater : BaseMonoBehaviour, IClientComponent
{
	// Token: 0x04000F43 RID: 3907
	public GameObjectRef singleExplosionEffect;

	// Token: 0x04000F44 RID: 3908
	public Transform[] parts;

	// Token: 0x04000F45 RID: 3909
	public float partWidth = 0.2f;

	// Token: 0x04000F46 RID: 3910
	public float partLength = 0.1f;

	// Token: 0x04000F47 RID: 3911
	public Quaternion[] targetRotations;

	// Token: 0x04000F48 RID: 3912
	public Quaternion[] initialRotations;

	// Token: 0x04000F49 RID: 3913
	public Renderer[] renderers;

	// Token: 0x04000F4A RID: 3914
	public Material materialSource;

	// Token: 0x04000F4B RID: 3915
	public float explodeRepeatMin = 0.05f;

	// Token: 0x04000F4C RID: 3916
	public float explodeRepeatMax = 0.15f;

	// Token: 0x04000F4D RID: 3917
	public float explodeLerpSpeed = 30f;

	// Token: 0x04000F4E RID: 3918
	public Vector3 twistAmount;

	// Token: 0x04000F4F RID: 3919
	public float fuseLength = 3f;

	// Token: 0x04000F50 RID: 3920
	public float explodeStrength = 10f;

	// Token: 0x04000F51 RID: 3921
	public float explodeDirBlend = 0.5f;

	// Token: 0x04000F52 RID: 3922
	public float duration = 10f;

	// Token: 0x04000F53 RID: 3923
	public ParticleSystemContainer smokeParticle;
}
