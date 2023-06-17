using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Token: 0x0200029C RID: 668
public class OxygenOverlay : MonoBehaviour
{
	// Token: 0x040015FE RID: 5630
	[SerializeField]
	private PostProcessVolume postProcessVolume;

	// Token: 0x040015FF RID: 5631
	[SerializeField]
	private float smoothTime = 1f;

	// Token: 0x04001600 RID: 5632
	[Tooltip("If true, only show this effect when the player is mounted in a submarine.")]
	[SerializeField]
	private bool submarinesOnly;
}
