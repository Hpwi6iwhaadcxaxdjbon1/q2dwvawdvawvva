using System;
using UnityEngine;

// Token: 0x0200028B RID: 651
public class BlendShapeController : MonoBehaviour
{
	// Token: 0x040015BD RID: 5565
	public SkinnedMeshRenderer TargetRenderer;

	// Token: 0x040015BE RID: 5566
	public BlendShapeController.BlendState[] States;

	// Token: 0x040015BF RID: 5567
	public float LerpSpeed = 0.25f;

	// Token: 0x040015C0 RID: 5568
	public BlendShapeController.BlendMode CurrentMode;

	// Token: 0x02000C8C RID: 3212
	public enum BlendMode
	{
		// Token: 0x040043D1 RID: 17361
		Idle,
		// Token: 0x040043D2 RID: 17362
		Happy,
		// Token: 0x040043D3 RID: 17363
		Angry
	}

	// Token: 0x02000C8D RID: 3213
	[Serializable]
	public struct BlendState
	{
		// Token: 0x040043D4 RID: 17364
		[Range(0f, 100f)]
		public float[] States;

		// Token: 0x040043D5 RID: 17365
		public BlendShapeController.BlendMode Mode;
	}
}
