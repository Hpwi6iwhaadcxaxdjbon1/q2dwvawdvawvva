using System;
using UnityEngine;

// Token: 0x0200033B RID: 827
public class EffectScaleWithCameraDistance : MonoBehaviour, IEffect
{
	// Token: 0x0400182E RID: 6190
	public float minScale = 1f;

	// Token: 0x0400182F RID: 6191
	public float maxScale = 2.5f;

	// Token: 0x04001830 RID: 6192
	public float scaleStartDistance = 50f;

	// Token: 0x04001831 RID: 6193
	public float scaleEndDistance = 150f;
}
