using System;
using UnityEngine;

// Token: 0x0200070C RID: 1804
[ExecuteInEditMode]
public class AdaptMeshToTerrain : MonoBehaviour
{
	// Token: 0x04002955 RID: 10581
	public LayerMask LayerMask = -1;

	// Token: 0x04002956 RID: 10582
	public float RayHeight = 10f;

	// Token: 0x04002957 RID: 10583
	public float RayMaxDistance = 20f;

	// Token: 0x04002958 RID: 10584
	public float MinDisplacement = 0.01f;

	// Token: 0x04002959 RID: 10585
	public float MaxDisplacement = 0.33f;

	// Token: 0x0400295A RID: 10586
	[Range(8f, 64f)]
	public int PlaneResolution = 24;
}
