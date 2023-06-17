using System;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class EyeController : MonoBehaviour
{
	// Token: 0x04001847 RID: 6215
	public const float MaxLookDot = 0.8f;

	// Token: 0x04001848 RID: 6216
	public bool debug;

	// Token: 0x04001849 RID: 6217
	public Transform LeftEye;

	// Token: 0x0400184A RID: 6218
	public Transform RightEye;

	// Token: 0x0400184B RID: 6219
	public Transform EyeTransform;

	// Token: 0x0400184C RID: 6220
	public Vector3 Fudge = new Vector3(0f, 90f, 0f);

	// Token: 0x0400184D RID: 6221
	public Vector3 FlickerRange;

	// Token: 0x0400184E RID: 6222
	private Transform Focus;

	// Token: 0x0400184F RID: 6223
	private float FocusUpdateTime;
}
