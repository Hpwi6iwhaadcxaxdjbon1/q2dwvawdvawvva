using System;
using UnityEngine;

// Token: 0x02000718 RID: 1816
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
public class DeferredExtension : MonoBehaviour
{
	// Token: 0x04002979 RID: 10617
	public ExtendGBufferParams extendGBuffer = ExtendGBufferParams.Default;

	// Token: 0x0400297A RID: 10618
	public SubsurfaceScatteringParams subsurfaceScattering = SubsurfaceScatteringParams.Default;

	// Token: 0x0400297B RID: 10619
	public Texture2D blueNoise;

	// Token: 0x0400297C RID: 10620
	public float depthScale = 100f;

	// Token: 0x0400297D RID: 10621
	public bool debug;

	// Token: 0x0400297E RID: 10622
	public bool forceToCameraResolution;
}
