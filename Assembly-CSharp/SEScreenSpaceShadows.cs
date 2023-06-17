using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020009A0 RID: 2464
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Sonic Ether/SE Screen-Space Shadows")]
[ExecuteInEditMode]
public class SEScreenSpaceShadows : MonoBehaviour
{
	// Token: 0x0400352B RID: 13611
	private CommandBuffer blendShadowsCommandBuffer;

	// Token: 0x0400352C RID: 13612
	private CommandBuffer renderShadowsCommandBuffer;

	// Token: 0x0400352D RID: 13613
	private Camera attachedCamera;

	// Token: 0x0400352E RID: 13614
	public Light sun;

	// Token: 0x0400352F RID: 13615
	[Range(0f, 1f)]
	public float blendStrength = 1f;

	// Token: 0x04003530 RID: 13616
	[Range(0f, 1f)]
	public float accumulation = 0.9f;

	// Token: 0x04003531 RID: 13617
	[Range(0.1f, 5f)]
	public float lengthFade = 0.7f;

	// Token: 0x04003532 RID: 13618
	[Range(0.01f, 5f)]
	public float range = 0.7f;

	// Token: 0x04003533 RID: 13619
	[Range(0f, 1f)]
	public float zThickness = 0.1f;

	// Token: 0x04003534 RID: 13620
	[Range(2f, 92f)]
	public int samples = 32;

	// Token: 0x04003535 RID: 13621
	[Range(0.5f, 4f)]
	public float nearSampleQuality = 1.5f;

	// Token: 0x04003536 RID: 13622
	[Range(0f, 1f)]
	public float traceBias = 0.03f;

	// Token: 0x04003537 RID: 13623
	public bool stochasticSampling = true;

	// Token: 0x04003538 RID: 13624
	public bool leverageTemporalAA;

	// Token: 0x04003539 RID: 13625
	public bool bilateralBlur = true;

	// Token: 0x0400353A RID: 13626
	[Range(1f, 2f)]
	public int blurPasses = 1;

	// Token: 0x0400353B RID: 13627
	[Range(0.01f, 0.5f)]
	public float blurDepthTolerance = 0.1f;
}
