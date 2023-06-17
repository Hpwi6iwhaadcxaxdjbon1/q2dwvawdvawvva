using System;
using UnityEngine;

// Token: 0x0200096A RID: 2410
public class FishingRodViewmodel : MonoBehaviour
{
	// Token: 0x040033D4 RID: 13268
	public Transform PitchTransform;

	// Token: 0x040033D5 RID: 13269
	public Transform YawTransform;

	// Token: 0x040033D6 RID: 13270
	public float YawLerpSpeed = 1f;

	// Token: 0x040033D7 RID: 13271
	public float PitchLerpSpeed = 1f;

	// Token: 0x040033D8 RID: 13272
	public Transform LineRendererStartPos;

	// Token: 0x040033D9 RID: 13273
	public ParticleSystem[] StrainParticles;

	// Token: 0x040033DA RID: 13274
	public bool ApplyTransformRotation = true;

	// Token: 0x040033DB RID: 13275
	public GameObject CatchRoot;

	// Token: 0x040033DC RID: 13276
	public Transform CatchLinePoint;

	// Token: 0x040033DD RID: 13277
	public FishingRodViewmodel.FishViewmodel[] FishViewmodels;

	// Token: 0x040033DE RID: 13278
	public float ShakeMaxScale = 0.1f;

	// Token: 0x02000EC6 RID: 3782
	[Serializable]
	public struct FishViewmodel
	{
		// Token: 0x04004CE8 RID: 19688
		public ItemDefinition Item;

		// Token: 0x04004CE9 RID: 19689
		public GameObject Root;
	}
}
