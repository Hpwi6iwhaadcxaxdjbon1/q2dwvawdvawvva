using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x02000296 RID: 662
public class ClimateOverlay : MonoBehaviour
{
	// Token: 0x040015ED RID: 5613
	[Range(0f, 1f)]
	public float blendingSpeed = 1f;

	// Token: 0x040015EE RID: 5614
	public PostProcessVolume[] biomeVolumes;
}
