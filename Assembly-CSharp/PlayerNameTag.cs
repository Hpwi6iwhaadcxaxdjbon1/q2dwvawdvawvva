using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200044A RID: 1098
public class PlayerNameTag : MonoBehaviour
{
	// Token: 0x04001D05 RID: 7429
	public CanvasGroup canvasGroup;

	// Token: 0x04001D06 RID: 7430
	public Text text;

	// Token: 0x04001D07 RID: 7431
	public Gradient color;

	// Token: 0x04001D08 RID: 7432
	public float minDistance = 3f;

	// Token: 0x04001D09 RID: 7433
	public float maxDistance = 10f;

	// Token: 0x04001D0A RID: 7434
	public Vector3 positionOffset;

	// Token: 0x04001D0B RID: 7435
	public Transform parentBone;
}
