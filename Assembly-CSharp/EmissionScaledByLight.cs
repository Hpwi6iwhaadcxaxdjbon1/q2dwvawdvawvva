using System;
using UnityEngine;

// Token: 0x0200033C RID: 828
public class EmissionScaledByLight : MonoBehaviour, IClientComponent
{
	// Token: 0x04001832 RID: 6194
	private Color emissionColor;

	// Token: 0x04001833 RID: 6195
	public Renderer[] targetRenderers;

	// Token: 0x04001834 RID: 6196
	public int materialIndex = -1;

	// Token: 0x04001835 RID: 6197
	private static MaterialPropertyBlock block;

	// Token: 0x04001836 RID: 6198
	public Light lightToFollow;

	// Token: 0x04001837 RID: 6199
	public float maxEmissionValue = 3f;
}
